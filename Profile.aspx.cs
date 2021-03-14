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
using global::Microsoft.WindowsAzure.Storage;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;

namespace Playdate
{

    public partial class Profile : Page
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

        /*
         * Gets the pet name from the table to display for "Logged in as"
         */
        public string getPetName()
        {
            try
            {
                TableQuery userEntry = new TableQuery().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Format(getEmail())));
                var retrievedResult = tableClient.ExecuteQuery(userEntry);
                return retrievedResult.ElementAt(0).RowKey;
            }
            catch (Exception ex)
            {
                Label2.Text = "ERROR: " + ex.Message;
            }
            return null;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ConnectToTable();
            if (!Request.IsAuthenticated)
            {
                Response.Redirect("Default.aspx");
                return;
            }
            try
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
            }
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            try
            {
                ConnectToTable();
                if (
                    string.IsNullOrWhiteSpace(AnimalTextBox.Text) ||
                    string.IsNullOrWhiteSpace(AgeTextBox.Text) || string.IsNullOrWhiteSpace(CityTextBox.Text) ||
                    string.IsNullOrWhiteSpace(StateTextBox.Text))
                {

                    Label2.Text = "ERROR: Please fill in all required fields.";
                    return;
                }

                //check if age is a number
                int n;
                bool isNumeric = int.TryParse(AgeTextBox.Text, out n);
                if (!isNumeric)
                {
                    Label2.Text += "ERROR: Please enter only numbers for age. <br>";
                    return;
                }

                if (n < 0)
                {
                    Label2.Text += "ERROR: Please enter only numbers more than 0 for age. <br>";
                    return;
                }

                if (!isStateAbbreviation(StateTextBox.Text))
                {
                    Label2.Text += "ERROR: Please enter appropriate state abbreviation. Example: Enter \"WA\" for Washington. <br>";
                    return;
                }

                if (!VerifyBio(BioTextBox.Text))
                {
                    Label2.Text += "ERROR: Please enter 20 words or lower for Bio. <br>";
                    return;
                }


                string picID = Format(getEmail()) + "+" + Format(getPetName()) + ".jpg"; //TO BE OBTAINED FROM AUTHENTICATION
                                                                                         //Format(EmailTextBox.Text) + "+" + Format(NameTextBox.Text) + ".jpg";
                if (!UploadPic(picID))
                {
                    return;
                }

                Pet p = new Pet(//Format(NameTextBox.Text), 
                    Format(getEmail()), Format(getPetName()), Format(AgeTextBox.Text), Format(AnimalTextBox.Text), Format(CityTextBox.Text), Format(StateTextBox.Text),
                    //Format(EmailTextBox.Text), 
                    Format(BioTextBox.Text), picID);

                if (!AddToTable(p))
                {
                    Label2.Text += "ERROR: Unable to create account. Please try again. <br>";
                    return;
                }
            }
            catch (Exception ex)
            {
                Label2.Text = "ERROR: " + ex.Message;
            }

            Label2.Text = "Account Updated!";
            Label1.Text = "";
            Home.Visible = true;
        }

        // check if the input state is of appropriate form
        private static bool isStateAbbreviation(string state)
        {
            string states = "|AL|AK|AS|AZ|AR|CA|CO|CT|DE|DC|FM|FL|GA|GU|HI|ID|IL|IN|IA|KS|KY|LA|ME|MH|MD|MA|MI|MN|MS|MO|MT|NE|NV|NH|NJ|NM|NY|NC|ND|MP|OH|OK|OR|PW|PA|PR|RI|SC|SD|TN|TX|UT|VT|VI|VA|WA|WV|WI|WY|";

            return state.Length == 2 && states.IndexOf(state) > 0;
        }

        /* check if the bio has 20 or less words
         */
        private static bool VerifyBio(string bio)
        {
            string[] text = bio.Split(new char[0], StringSplitOptions.RemoveEmptyEntries); //split line by whitespace
            return text.Length <= 20;
        }

        /* ConnectToTable --  instantiate a CloudTableClient object to interact with Azure Table Service
        * precondition: credentials are set up in appsetting.json
        */
        private void ConnectToTable()
        {
            try
            {
                Label2.Text = "";
                var storageAccount = Microsoft.Azure.Cosmos.Table.CloudStorageAccount.Parse(config["AzureStorage:ConnectionString"]);

                var _tableClient = storageAccount.CreateCloudTableClient();

                tableClient = _tableClient.GetTableReference(config["AzureStorage:Table"]);
            }
            catch (Exception e)
            {
                Label2.Text = "ERROR: " + e.Message;
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
                Label2.Text = "";
                if (p == null) { return false; }

                var batch = new TableBatchOperation();
                batch.InsertOrReplace(p); //stage insert
                tableClient.ExecuteBatch(batch); //commit insert
            }
            catch (Exception e)
            {
                Label2.Text = "ERROR: " + e.Message;
            }
            return true;
        }

        //redirect to main
        protected void Main_Click(object sender, EventArgs e)
        {
            try
            {
                Server.Transfer("Main.aspx");

            }
            catch (Exception ex)
            {
                Label2.Text = "ERROR: " + ex.Message;
            }

        }

        /*
         * TODO: Load in the profile data from the database so that all the user's info is loaded for them already 
         * when they want to edit
         */
        private void Load_Profile(string email, string name)
        {

        }

        /* reads in picture user uploaded from their file and upload to blob with their email+petname as key
         */
        private bool UploadPic(string picID)
        {
            try
            {
                if (PhotoUpload.HasFile)
                {
                    Stream photoStream = PhotoUpload.PostedFile.InputStream;
                    int photoLength = PhotoUpload.PostedFile.ContentLength;
                    string photoMime = PhotoUpload.PostedFile.ContentType;
                    string photoName = Path.GetFileName(PhotoUpload.PostedFile.FileName);
                    byte[] photoData = new byte[photoLength];
                    photoStream.Read(photoData, 0, photoLength);

                    var StorageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(config["AzureStorage:ConnectionString"]);
                    var BlobClient = StorageAccount.CreateCloudBlobClient();
                    var Container = BlobClient.GetContainerReference(config["AzureStorage:Container"]);
                    Container.CreateIfNotExists();

                    var BlobBlock = Container.GetBlockBlobReference(picID);
                    BlobBlock.Properties.ContentType = "image/jpg";
                    var fileContent = photoData;
                    BlobBlock.UploadFromByteArray(fileContent, 0, fileContent.Length);
                    return true;
                }
                else
                {
                    Label1.Text = "ERROR: No picture uploaded. Please upload a profile picture in jpg or png form.";
                }
            }
            catch (Exception ex)
            {
                Label2.Text = "ERROR: " + ex.Message;
                Label2.Text += "<br>ERROR: Unable to upload profile picture. Please try again.";

            }
            return false;
        }

    }
}