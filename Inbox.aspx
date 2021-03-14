<%@ Page Title=" " Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Inbox.aspx.cs" Inherits="Playdate.Inbox" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        &nbsp;</h2>
<h2>
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
                
                <asp:Button ID="GoToChat_Button" runat="server" Text='<%#: "Message " + Eval("PetName")%>' Style="margin-left: auto; display: block;" OnClick="Message_Button_Click"/>
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
        <asp:SqlDataSource ID="PlaydateDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:PlaydateDBConnectionString %>" SelectCommand="SELECT [ReceiverID], (SELECT PetName FROM Person WHERE (PersonID = Chat.ReceiverID)) AS 'PetName', (SELECT Email FROM Person WHERE (PersonID = Chat.ReceiverID)) AS 'ReceiverEmail', [Content] FROM Chat WHERE (SenderID IN (SELECT Chat.SenderID FROM Person AS Person_2 WHERE (Email = @Email) AND (PetName = 'Umu'))) AND (TimeSent IN (SELECT MAX(TimeSent) AS latest_timesent FROM Chat AS Chat_1 WHERE (SenderID IN (SELECT Chat_1.SenderID FROM Person AS Person_1 WHERE (Email = 'heedong@uw.edu') AND (PetName = 'Umu'))) GROUP BY ChatRoomID))">
         <SelectParameters>
                <asp:Parameter  DefaultValue="heedong@uw.edu" Name="Email" DbType="String" />
            </SelectParameters>
        </asp:SqlDataSource>
    <asp:Button ID="Back_Button" runat="server" OnClick="Back_Button_Click" Text="Back To Main" />
    </h2>
    <p>&nbsp;</p>

</asp:Content>
