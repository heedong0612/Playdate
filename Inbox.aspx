<%@ Page Title="Message Inbox" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Inbox.aspx.cs" Inherits="Playdate.Inbox" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %></h2>
    <asp:Button ID="MessageTest" runat="server" Text="Go To Message" OnClick="Message_Click" />
</asp:Content>
