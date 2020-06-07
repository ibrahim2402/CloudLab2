using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebUI
{
    public partial class Payment : System.Web.UI.Page
    {

        private string accountName = "ibrastorage";
        private string accountKey = "aCPhgjl5QfoKQnHo9IBdn0XzJukE76Vongk2thLIiirFPsvOTu+cAMwcBefMHyQu1kd33yA4d5XaqcrPDwSDkQ==";     // Azure storage account 
        CloudQueue queue, queue1, queue2, queue3;
        string myrecfkey;
        string[] keys = { "trswebqueue" , "offerwebqueue","crswebqueue","hrswebqueue" };


        protected void Page_Load(object sender, EventArgs e)
        {

            myrecfkey = Session["MyRefKey"].ToString();

            Console.WriteLine(myrecfkey);
        
            //Page uiPage = (Page)Context.Handler;
            //Label_offer.Text = ((TextBox)uiPage.FindControl("textPrice")).Text;
        

    }

        protected void get_result(object sender, EventArgs e)
        {
            StorageCredentials creds = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount storageAccount = new CloudStorageAccount(creds, useHttps: true);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
       
            // Retrieve a reference to a queue
            queue = queueClient.GetQueueReference("trswebqueue");
            queue1 = queueClient.GetQueueReference("offerwebqueue");
            queue2 = queueClient.GetQueueReference("crswebqueue");
            queue3 = queueClient.GetQueueReference("hrswebqueue");

            CloudQueue[] myques = { queue, queue1, queue2, queue3 };

            
            string[] readmsg = new string[4];
            try
            {
                // Create the queue if it doesn't already exist
                queue.CreateIfNotExists();
                queue1.CreateIfNotExists();
                queue2.CreateIfNotExists();
                queue3.CreateIfNotExists();

                // retrieve the next message
               // CloudQueueMessage readMessage = queue.GetMessage();
                for (int i = 0; i < 4; i++)
                {
                    if (myrecfkey == keys[i]){
                        CloudQueueMessage temp = myques[i].GetMessage();
                        // Display message (populate the textbox with the message you just retrieved.
                        Label_offer.Text = temp.AsString;
                    }
                }

                //Delete the message just read to avoid reading it over and over again
                queue.DeleteMessage(queue.GetMessage());
            }
            catch (NullReferenceException ee)
            {
                Debug.WriteLine("Problem reading from queue");
                error.Text = ee.ToString();
            }
        }
        protected void btn_pay(object sender, EventArgs e)
        {
            //Response.Redirect("~/DefaultTest.aspx?UserName=" + TextBox1.Text + "&UserEmail=" + TextBox3 + "&UserPrice"+ Label_offer);

            Response.Cookies["User"]["Value1"] = TextBox1.Text;
            Response.Cookies["User"]["Value2"] = TextBox3.Text;
            Response.Cookies["User"]["Value3"] = Label_offer.Text;
            Response.Cookies["User"].Expires = DateTime.Now.AddDays(1d);

            if (string.IsNullOrEmpty(TextBox1.Text) 
                || string.IsNullOrEmpty(TextBox3.Text) || string.IsNullOrEmpty(Label_offer.Text))
            {
                order_error.Text = "Enter correct details";
            }
            else
            {
                 Response.Redirect("DefaultTest.aspx");
            }
        }

    }
}