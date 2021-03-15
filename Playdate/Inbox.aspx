<%@ Page Title=" " Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Inbox.aspx.cs" Inherits="Playdate.Inbox" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="PLAYDATE">
        Messages </h2>
    <h2>
    <br />
        <asp:ListView ID="ListView1" runat="server" DataSourceID="PlaydateDataSource" OnSelectedIndexChanged="ListView1_SelectedIndexChanged" style="font-size: small">
            
            <ItemTemplate>
                <span style=" ">
                
                <img style="float: left; vertical-align: middle" src="https://playdate.blob.core.windows.net/profilepictures/<%# Eval("ReceiverEmail") %>+<%# Eval("PetName") %>.jpg"
                 width="150" height="105" />
                <div style="display: block; width: 700px; padding-left: 165px">
                    <asp:Label ID="PetNameLabel" runat="server" Text='<%# Eval("PetName") %>' Font-Bold='false' Font-Size="Large"/>
                </div>
                
                <div style="display: block; width: 700px; padding-left: 165px">
                    <asp:Label ID="ContentLabel" runat="server" Text='<%# Eval("Content") %>' Font-Bold='false' Font-Size="Medium" ForeColor="DarkGray"/>
                </div>
                
                <asp:Button ID="GoToChat_Button" runat="server" Text='<%#: "Message " + Eval("PetName")%>' Style="margin-left: auto; display: block;" OnClick="Message_Button_Click" CommandArgument='<%#Eval("ReceiverEmail")+","+ Eval("PetName")%>' Font-Size="Large"/>
            <br /></span>
            </ItemTemplate>
            <LayoutTemplate>
                <div id="itemPlaceholderContainer" runat="server" style="">
                    <span runat="server" id="itemPlaceholder" />
                </div>
                <div style="">
                </div>
            </LayoutTemplate>
            <SelectedItemTemplate>
                <span style="">PetName:
                <asp:Label ID="PetNameLabel" runat="server" Text='<%# Eval("PetName") %>' />
                <br />
                Content:
                <asp:Label ID="ContentLabel" runat="server" Text='<%# Eval("Content") %>' />
                <br />
                <br />
                </span>
            </SelectedItemTemplate>
        </asp:ListView>
        <asp:SqlDataSource ID="PlaydateDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:PlaydateDBConnectionString %>">
        </asp:SqlDataSource>
    <asp:Button ID="Back_Button" runat="server" OnClick="Back_Button_Click" Text="Back To Main" Height="32px" Width="125px" Font-Size="Large"/>
    </h2>
    <p>&nbsp;</p>

</asp:Content>
