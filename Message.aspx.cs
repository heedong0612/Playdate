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
using System.Net.Mail;
using System.Configuration;
using Microsoft.Azure.Cosmos.Table;
using System.Security.Claims;

namespace Playdate
{

    public partial class Message : Page

    {
        private string chatRoomID;

        // DEBUG PURPOSE
        string senderEmail;
        string senderPetname;
        string receiverEmail;
        string receiverPetname;

        static IConfigurationRoot GetConfiguration()
            => new Microsoft.Extensions.Configuration.ConfigurationBuilder()
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

                // string test = "";
                //Panel1.Controls.Clear();
                MAINPANEL.InnerHtml = "";
                while (dataReader.Read())
                {
                    string message_content = (string)dataReader.GetValue(0);
                    int message_sender = (int)dataReader.GetValue(1);
                    int message_receiver = (int)dataReader.GetValue(2);
                    DateTime message_timesent = (DateTime)dataReader.GetValue(3);

                    if (message_sender == senderID)
                    {
                        var pic = "https://playdate.blob.core.windows.net/profilepictures/" + Format(senderEmail) + "+" + Format(senderPetname) + ".jpg";
                        MAINPANEL.InnerHtml += "<br /><div width=\"100%\" class = \"right_align\">" + message_content + "&emsp;&emsp;" + message_timesent + "&nbsp;&nbsp; <img class = \"chatlogo\" src=\"" + pic + "\" alt=\"Sender's Profile Pic\"></div>";


                    }
                    else
                    {
                        var pic = "https://playdate.blob.core.windows.net/profilepictures/" + Format(receiverEmail) + "+" + Format(receiverPetname) + ".jpg"; ;
                        MAINPANEL.InnerHtml += "<br /><div width=\"100%\" class = \"left_align\"><img class = \"chatlogo\" src=\"" + pic + "\" alt=\"Receiver's Profile Pic\">&nbsp;&nbsp; " + message_timesent + "&emsp;&emsp;" + message_content + "</div>";
                    }

                }
                Panel1.BorderColor = System.Drawing.Color.Black;

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
            Response.Redirect("Inbox.aspx");
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

            //send email notif to recipient
            string send = ConfigurationManager.AppSettings["SendEmail"];
            if (send.ToLower() == "true")
            {
                SendEmail(MessageBox.Text);
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


        //send notif to the other recipient
        private void SendEmail(string body)
        {
            //receiverEmail
            MailMessage mailmsg = new MailMessage(config["SendEmail:senderEmail"], receiverEmail);
            mailmsg.Subject = "You received a message from " + senderPetname + " from PlayDate!";
            mailmsg.Body = senderPetname + " messaged: \"" + body + "\".";
            mailmsg.IsBodyHtml = true;
            mailmsg.Body += "<br>Log in to <a href=\"http://playdate4pets.azurewebsites.net/\">PlayDate</a> to reply!<br><br>-The PlayDate Team :)";

            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(config["SendEmail:senderEmail"], config["SendEmail:pass"]);
                smtp.EnableSsl = true;
                smtp.Send(mailmsg);
            }
        }
        protected void Back_Button_Click(object sender, EventArgs e)
        {
            Response.Redirect("Inbox.aspx");
        }

    }
}