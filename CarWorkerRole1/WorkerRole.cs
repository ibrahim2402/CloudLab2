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

namespace CarWorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private string accountName = "ibrastorage";
        private string accountKey = "aCPhgjl5QfoKQnHo9IBdn0XzJukE76Vongk2thLIiirFPsvOTu+cAMwcBefMHyQu1kd33yA4d5XaqcrPDwSDkQ==";     // Azure storage account key here "YOUR_ACCOUNT_KEY";     
        private StorageCredentials creds;
        private CloudStorageAccount storageAccount;
        private CloudQueueClient queueClient;
        private CloudQueue inqueue, outqueue;
        private CloudQueueMessage inMessage, outMessage;

        double amount;

        private void initQueue()
        {


            creds = new StorageCredentials(accountName, accountKey);
            storageAccount = new CloudStorageAccount(creds, useHttps: true);

            // Create the queue client
            queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            //I tried to give the queue another name but it didn't work,
            //maybe because I have to run VStudio as administrator
            inqueue = queueClient.GetQueueReference("crsworkerqueue");

            // Create the queue if it doesn't already exist
            inqueue.CreateIfNotExists();

            // Retrieve a reference to a queue
            outqueue = queueClient.GetQueueReference("crswebqueue");

            // Create the queue if it doesn't already exist
            outqueue.CreateIfNotExists();
        }


        public override void Run()
        {
            Trace.TraceInformation("CarWorkerRole1 is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            catch (AggregateException e)
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

            Trace.TraceInformation("CarWorkerRole1 has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("CarWorkerRole1 is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("CarWorkerRole1 has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            initQueue();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    inMessage = await inqueue.GetMessageAsync();
                    //Console.WriteLine("Retrieved message with content '{0}'", inMessage.AsString);  //Show the received message in the development console

                    if (inMessage != null)
                    {
                        string s = inMessage.AsString;
                        if (s != null)
                        {
                            //Splits message by information
                            string[] msg = s.Split('*');
                            int days = int.Parse(msg[0]);
                            bool car;
                            if (msg[1] == "True")
                                car = true;
                            else
                                car = false;

                            double discount = calculateHotel(days, car);

                            Trace.TraceInformation("***** Worker Received " + s);

                            // Async delete the message
                            await inqueue.DeleteMessageAsync(inMessage);

                            // Create a message and add it to the queue.
                            outMessage = new CloudQueueMessage(discount.ToString());
                            outqueue.AddMessage(outMessage);

                            Trace.TraceInformation("Working");
                            await Task.Delay(1000);

                        }

                        
                    }

                }
                catch (NullReferenceException e)
                {

                }
                
               
            }
        }

        private double calculateHotel(int days, bool car)
        {
            amount = 0.0;
            if (car)
            {
                amount = 500 * days;
            }
            else
            {
                amount = 800 * days;
            }
            return amount;
        }
    }
}
