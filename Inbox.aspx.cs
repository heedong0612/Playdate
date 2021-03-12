using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Playdate
{
    public partial class Inbox : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Message_Click(object sender, EventArgs e) 
        {
            Response.Redirect("Message.aspx");
        }
    }
}