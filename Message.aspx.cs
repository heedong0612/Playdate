﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos.Table;
using System.IO;
using System.Web.UI.WebControls;
using System.Security.Claims;

namespace Playdate
{
    
    public partial class Message : Page

    {   private string chatRoomID;

        // DEBUG PURPOSE
        string senderEmail;
        string senderPetname; // = "Umu";

        string receiverEmail; 
        string receiverPetname;
        
        static IConfigurationRoot GetConfiguration()
            => new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
            .AddJsonFile("appsettings.json")
            .Build();

        private static IConfigurationRoot config = GetConfiguration();
        private static CloudTable tableClient;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                Response.Redirect("Default.aspx");
                return;
            }

            ConnectToTable();
            senderEmail = ClaimsPrincipal.Current.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
            senderPetname = getPetName();
            receiverEmail = Request["receiverEmail"];
            receiverPetname = Request["receiverPetname"];

            //Debug.WriteLine("In Message");
            //Debug.WriteLine(receiverEmail);
            //Debug.WriteLine(receiverPetname);
            //Debug.WriteLine(senderEmail);
            //Debug.WriteLine(senderPetname);

            displayPreviousMessages();
        }

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
                // Label2.Text = "ERROR: " + e.Message;
            }
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
        public string getPetName()
        {
            try
            {
                TableQuery userEntry = new TableQuery().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Format(senderEmail)));
                var retrievedResult = tableClient.ExecuteQuery(userEntry);
                return retrievedResult.ElementAt(0).RowKey;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return "";
        }

        private void displayPreviousMessages()
        {
            // get Person ID from Person Table
            int senderID = getPersonID(senderEmail, senderPetname);
            int receiverID = getPersonID(receiverEmail, receiverPetname);

            // find chatRoomID
            chatRoomID = Math.Min(senderID, receiverID).ToString() + "+" + Math.Max(senderID, receiverID).ToString();

            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                builder.DataSource = config.GetValue<string>("PlaydateDB:DataSource");
                builder.UserID = config.GetValue<string>("PlaydateDB:AdminID");
                builder.Password = config.GetValue<string>("PlaydateDB:AdminPWD");
                builder.InitialCatalog = config.GetValue<string>("PlaydateDB:Catalog");

                SqlConnection connection = new SqlConnection(builder.ConnectionString);
                String sqlQuery = "Select Content, SenderID, ReceiverID, Timesent from Chat where ChatRoomID = @chatRoomID Order By Timesent";

                connection.Open();
                SqlCommand sql = new SqlCommand(sqlQuery, connection);
                sql.Parameters.Add("@chatRoomID", SqlDbType.VarChar).Value = chatRoomID;

                SqlDataReader dataReader = sql.ExecuteReader();

                List<string> chatHistory = new List<string>();

                // string test = "";
                Panel1.Controls.Clear();
                while (dataReader.Read())
                {
                    string message_content = (string)dataReader.GetValue(0);
                    int message_sender = (int)dataReader.GetValue(1);
                    int message_receiver = (int)dataReader.GetValue(2);
                    DateTime message_timesent = (DateTime)dataReader.GetValue(3);

                    Label l = new Label();
                    
                    if (message_sender == senderID)
                    {
                        l.Text = "<br /><br />[" + senderPetname + "]: "  + message_content + "&emsp;&emsp;" + message_timesent;
                        l.CssClass = "right_align";
                        
                    } else
                    {
                        l.Text = "<br /><br /> [" + receiverPetname + "]: " + message_content + "&emsp;&emsp;" + message_timesent;
                        l.CssClass = "left_align";
                    }
                    Debug.WriteLine("width: " + l.Width.ToString()); 
                    Debug.WriteLine(l.CssClass.ToString());
                    
                    l.BorderColor = System.Drawing.Color.Red;
                    
                    Panel1.Controls.Add(l);

                    chatHistory.Add(message_content);
                }
                Panel1.BorderColor = System.Drawing.Color.Black;
                // Label1.Text = test;

                sql.Dispose();
                connection.Close();
            }
            catch (SqlException error)
            {
                Debug.WriteLine(error.ToString());
            }

        }
        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect("Message.aspx");
        }

        private int getPersonID(string Email, string Petname)
        {
            // connect to sql db 
            int Output = -1;
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                builder.DataSource = config.GetValue<string>("PlaydateDB:DataSource");
                builder.UserID = config.GetValue<string>("PlaydateDB:AdminID");
                builder.Password = config.GetValue<string>("PlaydateDB:AdminPWD");
                builder.InitialCatalog = config.GetValue<string>("PlaydateDB:Catalog");

                SqlConnection connection = new SqlConnection(builder.ConnectionString);
                String sqlQuery = "Select PersonID from Person where Email = @Email AND PetName = @Petname";

                connection.Open();
                SqlCommand sql = new SqlCommand(sqlQuery, connection);
                sql.Parameters.Add("@Email", SqlDbType.VarChar).Value = Email;
                sql.Parameters.Add("@Petname", SqlDbType.VarChar).Value = Petname;

                SqlDataReader dataReader = sql.ExecuteReader();

                if (dataReader.Read())
                {
                    Output = (int)dataReader.GetValue(0);
                }
                Debug.WriteLine("PersonID (-1 if match not found)");
                Debug.WriteLine(Output);

                sql.Dispose();
                connection.Close();
            }
            catch (SqlException error)
            {
                Debug.WriteLine(error.ToString());
            }
            return Output;
        }

        private void saveChatToDB(string senderID, string receiverID, string chatRoomID, string content, DateTime timesent)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                builder.DataSource = config.GetValue<string>("PlaydateDB:DataSource");
                builder.UserID = config.GetValue<string>("PlaydateDB:AdminID");
                builder.Password = config.GetValue<string>("PlaydateDB:AdminPWD");
                builder.InitialCatalog = config.GetValue<string>("PlaydateDB:Catalog");

                SqlConnection connection = new SqlConnection(builder.ConnectionString);
                String sqlQuery = "Insert into Chat(ChatRoomID, SenderID, ReceiverID, Content, TimeSent) values(@chatRoomID, @senderID, @receiverID, @content, @timesent)";

                connection.Open();
                SqlCommand sql = new SqlCommand(sqlQuery, connection);
                sql.Parameters.Add("@senderID", SqlDbType.VarChar).Value = senderID;
                sql.Parameters.Add("@receiverID", SqlDbType.VarChar).Value = receiverID;
                sql.Parameters.Add("@content", SqlDbType.VarChar).Value = content;
                sql.Parameters.Add("@timesent", SqlDbType.DateTime).Value = timesent;
                sql.Parameters.Add("@chatRoomID", SqlDbType.VarChar).Value = chatRoomID;

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.InsertCommand = sql;
                adapter.InsertCommand.ExecuteNonQuery();

                sql.Dispose();
                connection.Close();
            }
            catch (SqlException error)
            {
                Debug.WriteLine(error.ToString());
            }
        }

        protected void SendButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Text == "")
            {
                return;
            }
            string content = MessageBox.Text;
            DateTime timesent = DateTime.Now;

            // get Person ID from Person Table
            int senderID = getPersonID(senderEmail, senderPetname);
            int receiverID = getPersonID(receiverEmail, receiverPetname);

            // find chatRoomID
            string chatRoomID = Math.Min(senderID, receiverID).ToString() + "+" + Math.Max(senderID, receiverID).ToString();

            // save message to Chat Table
            saveChatToDB(senderID.ToString(), receiverID.ToString(), chatRoomID, content, timesent);

            // wipe the text
            MessageBox.Text = "";
            displayPreviousMessages();
        }

        protected void Back_Button_Click(object sender, EventArgs e)
        {
            Response.Redirect("Inbox.aspx");
        }
    }
}