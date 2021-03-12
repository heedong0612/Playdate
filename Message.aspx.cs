using System;
using System.Web.UI;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
//using Microsoft.WindowsAzure.StorageClient;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;

namespace Playdate
{
    public partial class Message : Page
    {
        //sets up the credentials without hard-coding into the cs file
        static IConfigurationRoot GetConfiguration()
            => new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
            .AddJsonFile("appsettings.json")
            .Build();

        private static IConfigurationRoot config = GetConfiguration();
        private static CloudTable tableClient;
        //private static BlobBaseClient containerClient;
        List<Pet> petList = new List<Pet>();
        private static string email = "";

        /*
         * Gets the user email from the table to display for "Logged in as"
         */
        public string getEmail()
        {
            if (Request.IsAuthenticated)
            {
                if (ClaimsPrincipal.Current.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress") != null)
                    email = ClaimsPrincipal.Current.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
                return email;
            }
            return null;
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            /*try
            {
                string name = getPetName();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    NameTextBox.Text += name + "<br>";
                    Home.Visible = true;
                }
                else
                {
                    Home.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Label2.Text = "ERROR: " + ex.Message;
            }*/
        }

       
        /* ConnectToTable --  instantiate a CloudTableClient object to interact with Azure Table Service
        * precondition: credentials are set up in appsetting.json
        */
        private void ConnectToTable()
        {
            try
            {
                //Label2.Text = "";
                var storageAccount = Microsoft.Azure.Cosmos.Table.CloudStorageAccount.Parse(config["AzureStorage:ConnectionString"]);

                var _tableClient = storageAccount.CreateCloudTableClient();

                tableClient = _tableClient.GetTableReference(config["AzureStorage:Table"]);
            }
            catch (Exception e)
            {
                //Label2.Text = "ERROR: " + e.Message;
            }
        }
        /* Format -- formats all input text to capitalize only the first letter of input string 

         * postcondition: returns empty string if input is null or only whitespace
         */
        private static string Format(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return "";
            }
            s = s.ToLower();
            s = s[0].ToString().ToUpper() + s.Substring(1);
            return s;
        }

        /* AddToTable -- helper method to add Person object to Azure Table
        * precondition: Person input is not null object
        * postcondition:  if the fname and lname are same, table will override the old value and update with the one thats added the latest;
        *                 return false if Person object passed in is null
        */
        private Boolean AddToTable(Pet p)
        {
            try
            {
               // Label2.Text = "";
                if (p == null) { return false; }

                var batch = new TableBatchOperation();
                batch.InsertOrReplace(p); //stage insert
                tableClient.ExecuteBatch(batch); //commit insert
            }
            catch (Exception e)
            {
                //Label2.Text = "ERROR: " + e.Message;
            }
            return true;
        }

        //redirect to main
        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect("Inbox.aspx");
        }

    }
}