using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Host.SystemWeb;
using System.Security.Claims;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using System.Security.Principal;


namespace Playdate
{
    public partial class _Default : Page
    {
        string redirectUri = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];
        protected bool newAcc = true;
        private static CloudTable tableClient;
        private static IConfigurationRoot config = GetConfiguration();
        private string email = "";

        //sets up the credentials without hard-coding into the cs file
        static IConfigurationRoot GetConfiguration()
            => new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
            .AddJsonFile("appsettings.json")
            .Build();

        protected void Page_Load(object sender, EventArgs e)
        {
            ConnectToTable();
            if (Request.IsAuthenticated)
            {
                email = ClaimsPrincipal.Current.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
                TableQuery emailCheck = new TableQuery().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Format(email)));
                var retrievedResult = tableClient.ExecuteQuery(emailCheck);
                // if the query returned anything, we already registered this user
                if (retrievedResult.Any())
                {
                    newAcc = false;
                    Response.Redirect("Main.aspx");
                }
            }
        }

        protected void SignIn_Click(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.Current.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties { RedirectUri = redirectUri },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        protected void Submit_Click(object sender, EventArgs e)
        {
            string email = ClaimsPrincipal.Current.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
            string name = PetNameTextbox.Text;
            if (ComparePet(Format(email), Format(name)))
            {
                AddToTable(Format(email), Format(name));
                newAcc = false;
                Response.Redirect("Profile.aspx");
            }
            else
            {
                Error.Text = "ERROR: The account for this pet already exists. Please try again. <br>";
            }
            
        }

        /*
         * Gets the pet name from the table to display for "Logged in as"
         */
        protected void getPetName()
        {
            TableQuery userEntry = new TableQuery().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Format(email)));
            var retrievedResult = tableClient.ExecuteQuery(userEntry);
            PetName.Text = retrievedResult.ElementAt(0).RowKey;
        }

        /* ConnectToTable --  instantiate a CloudTableClient object to interact with Azure Table Service
        * precondition: credentials are set up in appsetting.json
        */
        private void ConnectToTable()
        {
            try
            {
                Error.Text = "";
                var storageAccount = Microsoft.Azure.Cosmos.Table.CloudStorageAccount.Parse(config["AzureStorage:ConnectionString"]);

                var _tableClient = storageAccount.CreateCloudTableClient();

                tableClient = _tableClient.GetTableReference(config["AzureStorage:Table"]);
            }
            catch (Exception e)
            {
                Error.Text = "ERROR: " + e.Message;
            }
        }

        /* AddToTable -- helper method to add Person object to Azure Table
        * precondition: Person input is not null object
        * postcondition:  if the fname and lname are same, table will override the old value and update with the one thats added the latest;
        *                 return false if Person object passed in is null
        */
        private Boolean AddToTable(string email, string name)
        {
            try
            {
                Error.Text = "";
                if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(name)) { return false; }

                var batch = new TableBatchOperation();
                var entity = new TableEntity();
                entity.PartitionKey = email;
                entity.RowKey = name;
                batch.InsertOrReplace(entity); //stage insert
                tableClient.ExecuteBatch(batch); //commit insert
            }
            catch (Exception e)
            {
                Error.Text = "ERROR: " + e.Message;
            }
            return true;
        }

        // true = not a match; false = table contains already
        private static bool ComparePet(string email, string name)
        {
            System.Collections.Generic.IEnumerable<Pet> itemlist = null;

            TableQuery<Pet> CustomerQuery = new TableQuery<Pet>().Where(TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, email),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, name)));
            itemlist = tableClient.ExecuteQuery(CustomerQuery);

            if (itemlist.Count() > 0)
            {
                return false; //there shouldn't be 2 pets with same name and same email

            }

            return true;
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

        protected void SignOut_Click(object sender, EventArgs e)
        {
            HttpContext.Current.GetOwinContext().Authentication.SignOut(
                    new AuthenticationProperties { RedirectUri = redirectUri },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType,
                    CookieAuthenticationDefaults.AuthenticationType);
        }
    }
}