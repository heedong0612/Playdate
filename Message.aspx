﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Message.aspx.cs" Inherits="Playdate.Message" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Message</h2>
    <p></p>
    <div style="width: 1200px; display:block">
        <asp:Panel ID="Panel1" runat="server">
        </asp:Panel>
        <div class="chatpanel" id="MAINPANEL" runat="server"></div>
        <p>&nbsp;</p>
        <p>&nbsp;</p>
        <asp:TextBox ID="MessageBox" runat="server" Height="28px" Width="1200px"></asp:TextBox>
&nbsp;
        &nbsp;<asp:Button Width="400px"  ID="SendButton" runat="server" Height="34px" OnClick="SendButton_Click" Text="Send A Message!"/>
    
    </div>
    
    <h2>
    </h2>   
    <p>
        <asp:Button  ID="Back_Button" runat="server" OnClick="Back_Button_Click" Text="Back to Inbox" />
    </p>
    <p>&nbsp;</p>
</asp:Content>

