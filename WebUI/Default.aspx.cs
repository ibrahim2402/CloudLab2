using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.UI;
using System.Windows;
using System.Windows.Forms;

namespace WebRole1
{
    public partial class _Default : Page

    {
        private string key;
        List<string> Places = new List<string>();
        private string accountName = "ibrastorage";
        private string accountKey = "aCPhgjl5QfoKQnHo9IBdn0XzJukE76Vongk2thLIiirFPsvOTu+cAMwcBefMHyQu1kd33yA4d5XaqcrPDwSDkQ==";

        StorageCredentials creds;
        CloudStorageAccount storageAccount;

        CloudQueueClient queueClient;
        CloudQueue queueFlight, queueOffer, queueCar, queueHotel;

        string from;
        string to;
        string nbrOfInfant;
        string nbrOfChildren;
        string nbrOfAdult;
        string nbrOfSenior;
        string numberOfDays;
        bool carType;
        string carResult;
        string numberOfNight;
        bool room;
        string hotelResult;

        public string Key { get => key; set => key = value; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Places.Add("STO");
            Places.Add("CPH");
            Places.Add("CDG");
            Places.Add("LHR");
            Places.Add("FRA");

            DropDown_from.DataSource = Places;
            DropDown_to.DataSource = Places;

            DropDown_from.DataBind();
            DropDown_to.DataBind();

        }
        // To checkout flight booking request
        protected void submit_btn_flight(object sender, EventArgs e)
        {
            // Using a state management logic to hold queue reference
            key = "trswebqueue";
            Session["MyRefKey"] = key;

            TextBox textPrice = new TextBox();
            textPrice.Visible = false;
            
            if ((DropDown_from.SelectedItem == null || DropDown_to.SelectedItem == null)
              || (TextBox_infant.Text == "" && TextBox_children.Text == "" && TextBox_adult.Text == "" && TextBox_senior.Text == ""))
            {
                confirmation_label.Text = "Error. Please enter all required data.";
            }
            else
            {
                confirmation_label.Text = "";
                from = DropDown_from.SelectedItem.Value;
                to = DropDown_to.SelectedItem.Value;
                nbrOfInfant = TextBox_infant.Text;
                nbrOfChildren = TextBox_children.Text;
                nbrOfAdult = TextBox_adult.Text;
                nbrOfSenior = TextBox_senior.Text;

                string result_f = from + "*" + to + "*" + nbrOfInfant + "*" + nbrOfChildren + "*" + nbrOfAdult + "*" + nbrOfSenior;

                Console.WriteLine(result_f);
                confirmation_label.Text = "Error. Please enter all required data." + result_f;

                try
                {
                    creds = new StorageCredentials(accountName, accountKey);
                    storageAccount = new CloudStorageAccount(creds, useHttps: true);

                    queueClient = storageAccount.CreateCloudQueueClient();

                    // Retrieve a reference to a specific queue
                    queueFlight = queueClient.GetQueueReference("trsworkerqueue");
                   

                    // Create the queue if it doesn't already exist
                    queueFlight.CreateIfNotExists();
                   

                    //remove any existing messages (just in case)
                    queueFlight.Clear();
                   
                    // Create a message and add it to the queue.
                    CloudQueueMessage messageFlight = new CloudQueueMessage(result_f);
                    queueFlight.AddMessage(messageFlight);
 
                    //Show in the console that some activity is going on in the Web Role
                    Debug.WriteLine("Message '" + messageFlight + "'stored in Queue");

                    dialogbox();
                }
                catch (Exception ee)
                {
                    confirmation_label.Text = ee.ToString();
                }
            }
        }

        // To checkout hotel booking request
        protected void submit_btn_hotel(object sender, EventArgs e)
        {
            // Using a state management logic to hold queue reference
            key = "hrswebqueue";
            Session["MyRefKey"] = key;

            if (rbn_hotel.Checked == true)
            {
                if ((single_room.Checked == true || double_room.Checked == true) && !string.IsNullOrEmpty(TextBox_night.Text))
                {

                    numberOfNight = TextBox_night.Text;

                    if (single_room.Checked)
                    {
                        room = true;
                    }
                    else
                    {
                        room = false;
                    }

                    hotelResult = numberOfNight + "*" + room;
                    try
                    {

                        creds = new StorageCredentials(accountName, accountKey);
                        storageAccount = new CloudStorageAccount(creds, useHttps: true);

                        queueClient = storageAccount.CreateCloudQueueClient();

                        // Retrieve a reference to a specific queue
                        queueHotel = queueClient.GetQueueReference("hrsworkerqueue");

                        // Create the queue if it doesn't already exist
                        queueHotel.CreateIfNotExists();

                        //remove any existing messages (just in case)
                        queueHotel.Clear();

                        // Create a message and add it to the queue.
                        CloudQueueMessage messageHotel = new CloudQueueMessage(hotelResult);
                        queueHotel.AddMessage(messageHotel);

                        //Show in the console that some activity is going on in the Web Role
                        Debug.WriteLine("Message '" + messageHotel + "'stored in Queue");

                        dialogbox();
                    }
                    catch (Exception ee)
                    {
                        confirmation_label.Text = ee.ToString();
                    }
                }
                else
                {
                    confirmation_label.Text = "Transaction can not be processed";
                }

            }
            else
            {
                confirmation_label.Text = "Error. Please select correct options in the categories.";
            }
        }
        // to checkout car hire request
        protected void submit_btn_car(object sender, EventArgs e)
        {

            // Using a state management logic to hold queue reference
            key = "crswebqueue";
            
            Session["MyRefKey"] = key;

            if (rbn_rent_car.Checked == true)
            {
                if ((rbn_car.Checked == true || rbn_bus.Checked == true) && !string.IsNullOrEmpty(TextBox_car_days.Text))
                {
                    numberOfDays = TextBox_car_days.Text;

                    if (single_room.Checked)
                    {
                        carType = true;
                    }
                    else
                    {
                        carType = false;
                    }

                    carResult = numberOfDays + "*" + carType;

                    try
                    {

                        //double carChoice = carOption();
                        //string carResult = carChoice.ToString();

                        // carOption();

                        creds = new StorageCredentials(accountName, accountKey);
                        storageAccount = new CloudStorageAccount(creds, useHttps: true);

                        queueClient = storageAccount.CreateCloudQueueClient();

                        // Retrieve a reference to a specific queue
                        queueCar = queueClient.GetQueueReference("crsworkerqueue");

                        // Create the queue if it doesn't already exist
                        queueCar.CreateIfNotExists();

                        //remove any existing messages (just in case)
                        queueCar.Clear();

                        // Create a message and add it to the queue.
                        CloudQueueMessage messageCar = new CloudQueueMessage(carResult);
                        queueCar.AddMessage(messageCar);

                        //Show in the console that some activity is going on in the Web Role
                        Debug.WriteLine("Message '" + messageCar + "'stored in Queue");


                        dialogbox();
                    }
                    catch (Exception ee)
                    {
                        confirmation_label.Text = ee.ToString();
                    }
                }
                else
                {
                    confirmation_label.Text = "Transaction can not be processed";
                }
            }
            else
            {
                confirmation_label.Text = "Error. Please select options.";
            }
        }
        protected void get_booking_offer(object sender, EventArgs e)
        {
            // Using a state management logic to hold queue reference
            key = "offerwebqueue";
            
            Session["MyRefKey"] = key;

            if ((DropDown_from.SelectedItem == null || DropDown_to.SelectedItem == null)
                || (TextBox_infant.Text == "" && TextBox_children.Text == "" && TextBox_adult.Text == "" && TextBox_senior.Text == ""))
            {
                confirmation_label.Text = "Error. Please enter all required data.";

            }
            else
            {

                if (rbn_hotel.Checked == true || rbn_rent_car.Checked == true)
                {
                    confirmation_label.Text = "";
                    from = DropDown_from.SelectedItem.Value;
                    to = DropDown_to.SelectedItem.Value;
                    nbrOfInfant = TextBox_infant.Text;
                    nbrOfChildren = TextBox_children.Text;
                    nbrOfAdult = TextBox_adult.Text;
                    nbrOfSenior = TextBox_senior.Text;

                    numberOfDays = TextBox_car_days.Text;

                    if (rbn_car.Checked)
                    {
                        carType = true;
                    }
                    else
                    {
                        carType = false;
                    }

                    numberOfNight = TextBox_night.Text;

                    if (single_room.Checked)
                    {
                        room = true;
                    }
                    else
                    {
                        room = false;
                    }

                    hotelResult = numberOfNight + "*" + room;

                    carResult = numberOfDays + "*" + carType;

                    //all--> for all 
                    //car--> for only car
                    //hotel--> hotell
                    //ticket --> only ticket
                    //carhotel--> car and hotel
                    string result ="all"+"*"+ from + "*" + to + "*" + nbrOfInfant + "*" + nbrOfChildren + "*" + nbrOfAdult
                       + "*" + nbrOfSenior + "*" + numberOfNight + "*" + numberOfDays + "*" + carType + "*" + room;

                    /*string result = from + "*" + to + "*" + nbrOfInfant + "*" + nbrOfChildren + "*" + nbrOfAdult
                        + "*" + nbrOfSenior;*/

                    Console.WriteLine(carResult);
                    confirmation_label.Text = "Error. Please enter all required data." + carResult;

                    try
                    {
                        creds = new StorageCredentials(accountName, accountKey);
                        storageAccount = new CloudStorageAccount(creds, useHttps: true);

                        queueClient = storageAccount.CreateCloudQueueClient();

                        // Retrieve a reference to a specific queue
                        queueOffer = queueClient.GetQueueReference("offerworkerqueue");

                        // Create the queue if it doesn't already exist
                        queueOffer.CreateIfNotExists();

                        //remove any existing messages (just in case)
                        queueOffer.Clear();

                        // Create a message and add it to the queue.
                        CloudQueueMessage message = new CloudQueueMessage(result);
                        queueOffer.AddMessage(message);

                        //Show in the console that some activity is going on in the Web Role
                        Debug.WriteLine("Message '" + message + "'stored in Queue");
                    }
                    catch (Exception ee)
                    {
                        confirmation_label.Text = ee.ToString();
                    }

                    dialogbox();
                }
                else
                {
                    confirmation_label.Text = "You can ONLY check out here if you choosed at least 2 services!";
                }
            }
        } 
        //Dialogbox to either continue or cancel the order
        public void dialogbox()
        {
            string myref = Session["MyRefKey"].ToString();

            Console.WriteLine(myref);

            //This pop for user to confirm further operation or decline order
            DialogResult dialogResult = (DialogResult)System.Windows.MessageBox.Show("1: Click YES to continue order? \n 2: Click NO to cancel the order", "CONFIRMATION PAGE", MessageBoxButton.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Session["MyRefKey"] = myref;
                confirmation_label.Text = " Total price: ";
                Response.Redirect("~/Payment.aspx"); //link to payment page
            }

            if (dialogResult == DialogResult.No)
            {
                Response.Redirect("~/Default.aspx");
                confirmation_label.Text = " Order has been cancel!";
            }
        }
    }
}