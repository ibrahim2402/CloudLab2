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
using System.Windows;
using System.Windows.Forms;
using TextBox = System.Windows.Forms.TextBox;

namespace WebUI
{
    public partial class DefaultTest : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            labelName.Text = Request.Cookies["User"]["Value1"].ToString();
            labelEmail.Text = Request.Cookies["User"]["Value2"].ToString();
            labelPrice.Text = Request.Cookies["User"]["Value3"].ToString();

            /*   labelName.Text = Request.QueryString["UserName"];
                labelEmail.Text = Request.QueryString["UserEmail"];
                labelPrice.Text = Request.QueryString["UserPrice"]; */

            //string name = labelName.Text;
            //string email = labelEmail.Text;

            /*
            if (Request.Cookies["User"] == null && !(string.IsNullOrEmpty(name) && string.IsNullOrEmpty(email)))
            {
                order_error.Text = "Failed Payment Process. Enter Name and Email correctly!";

            }
            else
            {
               
            }*/


        }
      
    }
}