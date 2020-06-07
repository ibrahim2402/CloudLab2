using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private string accountName = "ibrastorage";
        private string accountKey = "aCPhgjl5QfoKQnHo9IBdn0XzJukE76Vongk2thLIiirFPsvOTu+cAMwcBefMHyQu1kd33yA4d5XaqcrPDwSDkQ==";   // Write your Azure storage account key here "YOUR_ACCOUNT_KEY";     
        private StorageCredentials creds;
        private CloudStorageAccount storageAccount;
        private CloudQueueClient queueClient;
        private CloudQueue inqueue, outqueue;
        private CloudQueueMessage inMessage, outMessage;
        private double price;

        private string[] airportCodes = { "STO", "CPH", "CDG", "LHR", "FRA" };
        private string[] airportNames = { "Stockholm", "Copenhagen", "Paris", "London", "Frankfurt" };
        private double[] latitudes = { 59.6519, 55.6181, 49.0097, 51.4707, 50.1167 };
        private double[] longitudes = { 17.9186, 12.6561, 2.5478, -0.4543, 8.6833 };

        //Gets instances of the queues and creates them if they don't exist 

        private void InitQueue()
        {


            creds = new StorageCredentials(accountName, accountKey);
            storageAccount = new CloudStorageAccount(creds, useHttps: true);

            // Create the queue client
            queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue

            inqueue = queueClient.GetQueueReference("trsworkerqueue");

            // Create the queue if it doesn't already exist
            inqueue.CreateIfNotExists();

            // Retrieve a reference to a queue
            outqueue = queueClient.GetQueueReference("trswebqueue");

            // Create the queue if it doesn't already exist
            outqueue.CreateIfNotExists();
        }

        public override void Run()
        {
            Trace.TraceInformation("WorkerRole1 is running");



            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            catch (AggregateException a)
            {

            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("WorkerRole1 has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole1 is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole1 has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            InitQueue();        //call the queue initialization method

            while (!cancellationToken.IsCancellationRequested)
            {
                // Async dequeue (read) the message
                inMessage = await inqueue.GetMessageAsync();
                // Console.WriteLine("Retrieved message with content '{0}'", inMessage.AsString);  //Show the received message in the development console
                if (inMessage != null)
                {
                    string s = inMessage.AsString;

                    //Splits message by information
                    string[] info = s.Split('*');

                    string from = (info[0]);
                    string to = (info[1]);
                    int nbrInfant = int.Parse(info[2]);
                    int nbrChild = int.Parse(info[3]);
                    int nbrAdult = int.Parse(info[4]);
                    int nbrSenior = int.Parse(info[5]);
                    double distance = getFlightDistance(from, to);


                    //sends info to calculate price method
                    double offer = calculatedPrice(nbrInfant, nbrChild, nbrAdult, nbrSenior, distance);

                    Trace.TraceInformation("***** Worker Received " + s);

                    // Async delete the message
                    await inqueue.DeleteMessageAsync(inMessage);

                    // Create a message and add it to the queue.
                    outMessage = new CloudQueueMessage(offer.ToString());
                    outqueue.AddMessage(outMessage);

                    Trace.TraceInformation("Working");
                    await Task.Delay(1000);
                }
                //convert the message to string

            }
        }

        private double calculatedPrice(int nbrInfant, int nbrChild, int nbrAdult, int nbrSenior, double distance)
        {
            double baseprice = 500.0;
            double amount;
        
            if (nbrSenior > 2)
            {
                amount = (baseprice * nbrSenior) * 0.5;
            }
            else
            {
                amount = (baseprice * nbrSenior) * 0.2;
            }

      
            return amount;
        }

        public double getFlightDistance(string origin, string destination)
        {
            double earthRadius = 6371.0;
           
            double latitudeFrom, longitudeFrom, latitudeTo, longitudeTo;
            latitudeFrom = getLatLongitude(origin, 'L');
            longitudeFrom = getLatLongitude(origin, 'G');
            latitudeTo = getLatLongitude(destination, 'L');
            longitudeTo = getLatLongitude(destination, 'G');
            if ((latitudeFrom < -999) || (longitudeFrom < -999) || (latitudeTo < -999) || (longitudeTo < -999)) return -1;
            //            if ((latitudeFrom < 0) || (longitudeFrom < 0) || (latitudeTo < 0) || (longitudeTo < 0))  return -1;

            double x1 = degreeToRadians(latitudeFrom);
            double y1 = degreeToRadians(longitudeFrom);
            double x2 = degreeToRadians(latitudeTo);
            double y2 = degreeToRadians(longitudeTo);
            // great circle distance in radians
            double centralAngle = Math.Acos(Math.Sin(x1) * Math.Sin(x2) + Math.Cos(x1) * Math.Cos(x2) * Math.Cos(y1 - y2));
            double distanceXY = earthRadius * centralAngle;
            return distanceXY;
        }

        private double degreeToRadians(double angleDegrees)
        {
            return (Math.PI / 180) * angleDegrees;
        }

        private double getLatLongitude(string s, char latorlong)
        {
            int i;
            for (i = 0; i < 5; i++)
            {
                if (s == airportCodes[i]) break;
            }
            if (i >= 5)
            {
                return (double)-1000.0;
            }
            else
            {
                if (latorlong == 'L') return latitudes[i];
                return longitudes[i];
            }
        }

    }
}
