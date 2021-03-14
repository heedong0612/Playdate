using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.ModelBinding;
using Microsoft.Azure.Cosmos.Table;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Playdate
{
    public partial class Main : Page
    {

        //sets up the credentials without hard-coding into the cs file
        static IConfigurationRoot GetConfiguration()
            => new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
            .AddJsonFile("appsettings.json")
            .Build();

        private static IConfigurationRoot config = GetConfiguration();
        private static CloudTable table;

        private List<string> AnimalTypes = new List<string> { 
            "Dog",      // 0
            "Cat",      // 1
            "Rabbit",   // 2
            "Hamster",  // 3
            "Monkey",   // 4
            "Bird",     // 5
            "Snake"     // 6
        };

        // by default, show all animals
        TableQuery<Pet> query = new TableQuery<Pet>().Where(
                    TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.NotEqual, ""),
                        TableOperators.And, TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.NotEqual, "")));

        /* ConnectToTable --  instantiate a CloudTableClient object to interact with Azure Table Service
       * precondition: credentials are set up in appsetting.json
       */
        private void ConnectToTable()
        {
            try
            {
                var storageAccount = Microsoft.Azure.Cosmos.Table.CloudStorageAccount.Parse(config["AzureStorage:ConnectionString"]);

                var _tableClient = storageAccount.CreateCloudTableClient();

                table = _tableClient.GetTableReference(config["AzureStorage:Table"]);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!Request.IsAuthenticated)
            //{
            //    Response.Redirect("Default.aspx");
            //    return;
            //}

            for (int i = 0; i < AnimalTypes.Count; i++)  {
                CheckBoxList1.Items[i].Text = "&nbsp;" + AnimalTypes[i];
            }

        }

        protected void Message_Button_Clicked(object sender, EventArgs e)
        {

        }

        public IQueryable<Pet> GetPets()
        {
            ConnectToTable();
            Debug.WriteLine("get pets called");
            var itemlist = table.ExecuteQuery(query);
            return itemlist.AsQueryable();
        }

        protected void CheckBoxList1_SelectedIndexChanged1(object sender, EventArgs e)
        {
            // ConnectToTable();
            string queryFilters = TableQuery.GenerateFilterCondition("PrimaryKey", QueryComparisons.Equal, ""); // impossible condition
            bool anyBoxChecked = false;

            for (int i = 0; i < CheckBoxList1.Items.Count; i++)
            {
                if (CheckBoxList1.Items[i].Selected)
                {
                    anyBoxChecked = true;
                    queryFilters = TableQuery.CombineFilters(queryFilters, TableOperators.Or, TableQuery.GenerateFilterCondition("Animal", QueryComparisons.Equal, AnimalTypes[i]));
                }
            }

            if (anyBoxChecked)
            {
                query = new TableQuery<Pet>().Where(queryFilters);

            }

            // update the ListView
            PetList.DataBind();

        }

        protected void Profile_Button_Click(object sender, EventArgs e)
        {
            Response.Redirect("Profile.aspx");
        }
    }

    public class Pet : TableEntity
    {
        public string Age { get; set; }
        public string Animal { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string PicID { get; set; }

        public Pet() { }
        // public Pet(string name, string age, string animal, string city, string state, string email, string bio, string picID)
        public Pet(string email, string name, string age, string animal, string city, string state, string bio, string picID)
        {
            PartitionKey = email;
            RowKey = name;
            this.Age = age;
            this.Animal = animal;
            this.City = city;
            this.State = state;
            this.Email = email;
            this.Bio = bio;
            this.PicID = picID;
        }
    }
}