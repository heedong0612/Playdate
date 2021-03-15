<%@ Page Title="LOGIN" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Playdate._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="preconnect" href="https://fonts.gstatic.com">
    <link href="https://fonts.googleapis.com/css2?family=Pacifico&family=Raleway&display=swap" rel="stylesheet">
    <%--<link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap@4.6.0/dist/css/bootstrap.min.css'>--%>
    
    <script src='https://code.jquery.com/jquery-3.5.1.slim.min.js'></script>
    <script src='https://cdn.jsdelivr.net/npm/bootstrap@4.6.0/dist/js/bootstrap.bundle.min.js'></script>

    <div class="body">
        <br />
        <img class="LOGOPIC" src="https://playdate.blob.core.windows.net/profilepictures/LOGO.png" runat="server" />
        <div class="applabel">
            <asp:Label class="AppName" runat="server" Text="PLAYDATE"></asp:Label>
        </div>
        <br />
        <div class="jumbotron">
          
            <img src="https://playdate.blob.core.windows.net/profilepictures/mainImg.jpg" class="d-block w-100 bannerpic" alt="mainpic">
            <p class="landingContainer">
                Message. Chat. Playdate. It’s easy and fun to find other pet friends on Playdate. Make your pet's profile stand out with your best pic of them and a little something about them to share the happiness. Once messaged, you will be able to connect with other pets all around the States for a fun day playday!

Playdate is the biggest and flyest party in the world for pets — it’s about time you showed up.
            </p>
        </div>
        <hr />
        
        
        <% if (!Request.IsAuthenticated)
            { %>
        <h3 class="landingContainer2>
            
            <asp:Label ID="Label1" runat="server" Text="<b>Note: </b> If this is your first time visiting Playdate, <br />choose <i>Use another account</i> and <i>Create one!</i> upon clicking this button <br /> <br />" Font-Size="Medium" ForeColor="DarkGray" ></asp:Label>
            <asp:Button class="signinbutton" ID="SignIn" runat="server" Text="Sign In or Sign Up" OnClick="SignIn_Click" ForeColor="Black" Height="42px" />
            &nbsp;<asp:Image class="MSLOGO" src="https://playdate.blob.core.windows.net/profilepictures/MS.png" runat="server" />
            &nbsp;<asp:Image class="GGLOGO" src="https://playdate.blob.core.windows.net/profilepictures/Google.png" runat="server" />
        </h3>
        <h3 class="landingContainer2">
            &nbsp;</h3>
        <% }
            else
            { %>
        <% if (newAcc)
            {%>
        <h4>
            <asp:Label ID="EnterName" runat="server" Text="Enter Pet Name:"></asp:Label>
            <asp:TextBox ID="PetNameTextbox" runat="server"></asp:TextBox>
            <asp:Button ID="Submit" runat="server" Text="Submit" OnClick="Submit_Click" />
            <br />
        </h4>
        <h4>
            <b class="warning">NOTICE:</b> You cannot change this later
        </h4>
        <% }
            else
            {%>
        <h3>Logged in as <% getPetName(); %>
            <asp:Label ID="PetName" runat="server" Text=""></asp:Label>
        </h3>
        <% }
            } %>
        <%-- <asp:TextBox ID="PetNameTextbox" runat="server"></asp:TextBox>
    <asp:Label ID="PetName" runat="server" Text=""></asp:Label>--%>
        <asp:Label ID="Error" runat="server" Text=""></asp:Label>
        <br />
    </div>
</asp:Content>
