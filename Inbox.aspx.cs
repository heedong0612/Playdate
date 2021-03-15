using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Security.Claims;
using Microsoft.Azure.Cosmos.Table;
using System.Web.UI.WebControls;

namespace Playdate
{
    public partial class Inbox : Page
    {
        // DEBUG PURPOSE
        private string senderEmail;
        private static CloudTable table;


        static IConfigurationRoot GetConfiguration()
            => new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
            .AddJsonFile("appsettings.json")
            .Build();

        private static IConfigurationRoot config = GetConfiguration();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                Response.Redirect("Default.aspx");
                return;
            }

            senderEmail = ClaimsPrincipal.Current.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
            display_Inbox();
        }

        public string getEmail() { return senderEmail; }
        public string getPet() 
        {
            ConnectToTable();
            TableQuery userEntry = new TableQuery().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Format(senderEmail)));
            var retrievedResult = table.ExecuteQuery(userEntry);
            return retrievedResult.ElementAt(0).RowKey;
        }

        private void display_Inbox()
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                builder.DataSource = config.GetValue<string>("PlaydateDB:DataSource");
                builder.UserID = config.GetValue<string>("PlaydateDB:AdminID");
                builder.Password = config.GetValue<string>("PlaydateDB:AdminPWD");
                builder.InitialCatalog = config.GetValue<string>("PlaydateDB:Catalog");

                SqlConnection connection = new SqlConnection(builder.ConnectionString);
                string sqlQuery = "select Content, (select Email from Person where PersonID = receiverID) as ReceiverEmail, (select Petname from Person where PersonID = receiverID) as PetName from[dbo].[Chat] as main right join (select chatRoomID, max(timesent) as ts from[dbo].[Chat] where senderID in (select PersonID from[dbo].[Person] where Email = '"+getEmail()+"') or receiverID in (select PersonID from[dbo].[Person] where Email = '"+getEmail()+"') group by ChatRoomID, senderID, receiverID) subq on main.chatroomID = subq.chatroomID and main.timesent = subq.ts";

                connection.Open();
                SqlCommand sql = new SqlCommand(sqlQuery, connection);
                sql.Parameters.AddWithValue("Email", getEmail());
                sql.Parameters.AddWithValue("Pet", getPet());
                PlaydateDataSource.SelectCommand = sqlQuery;

                SqlDataReader dataReader = sql.ExecuteReader();
                
                string test = "";
                string receiverName;
                string lastMessage;

                while (dataReader.Read())
                {              
                    if (dataReader.GetValue(0) != System.DBNull.Value)
                    {

                        receiverName = (string)dataReader.GetValue(0);
                        lastMessage = (string)dataReader.GetValue(1);
                        test += "<br /><br />" + receiverName + "<br />" + lastMessage;
                    } 
                }
                
                sql.Dispose();
                connection.Close();
            }
            catch (SqlException error)
            {
                Debug.WriteLine(error.ToString());
            }
        }

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

        protected void Message_Button_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string[] receiverInfo = btn.CommandArgument.ToString().Split(new char[] { ',' });
            Response.Redirect($"Message.aspx?receiverEmail={receiverInfo[0]}&receiverPetname={receiverInfo[1]}");
        }

        protected void Back_Button_Click(object sender, EventArgs e)
        {
            Response.Redirect("Main.aspx");
        }

        protected void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            display_Inbox();
        }
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
    }

}