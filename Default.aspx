<%@ Page Title="LOGIN" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Playdate._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
     <div class="body">
        <h2>Login</h2>
        <script src="./script.js"></script>


        <style>
            article, aside, figure, footer, header, hgroup,
            menu, nav, section {
                display: block;
            }
        </style>

        <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap@4.6.0/dist/css/bootstrap.min.css'>
        <link rel="stylesheet" href="./style.css">
        <link rel="preconnect" href="https://fonts.gstatic.com">
        <link href="https://fonts.googleapis.com/css2?family=Comfortaa:wght@300&display=swap" rel="stylesheet">
        <div class="container">

            <img class="LOGO" src="LOGO.png" runat="server" />
            <br />
            <asp:Label class="applabel" ID="AppName" runat="server" Text="PLAYDATE"></asp:Label>

            <hr />
            <br />


            <br />
   
    <% if(!Request.IsAuthenticated) { %>
        <h3>
            <asp:Button ID="SignIn" runat="server" Text="Sign In" OnClick="SignIn_Click" />
        </h3>
    <% } else { %>
            <% if (newAcc) {%>
                <h4>
                    <asp:Label ID="EnterName" runat="server" Text="Enter Pet Name:"></asp:Label>
                    <asp:TextBox ID="PetNameTextbox" runat="server"></asp:TextBox>
                    <asp:Button ID="Submit" runat="server" Text="Submit" OnClick="Submit_Click" />
                    <br />
                    <b>NOTICE:</b> You cannot change this later
                </h4>                
            <% } else {%>
                 <h3>Logged in as <% getPetName(); %>
                 <asp:Label ID="PetName" runat="server" Text=""></asp:Label>
                 </h3>
            <% } 
       } %>
       <asp:Label ID="Error" runat="server" Text=""></asp:Label>
</asp:Content>
