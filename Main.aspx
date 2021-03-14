<%@ Page Title="Ugly Main Feed (help)" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="Playdate.Main" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="preconnect" href="https://fonts.gstatic.com">
    <link href="https://fonts.googleapis.com/css2?family=Pacifico&family=Raleway&display=swap" rel="stylesheet">
    <%--<link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap@4.6.0/dist/css/bootstrap.min.css'>--%>

    <script src='https://code.jquery.com/jquery-3.5.1.slim.min.js'></script>
    <script src='https://cdn.jsdelivr.net/npm/bootstrap@4.6.0/dist/js/bootstrap.bundle.min.js'></script>
    <section>
        <div class="body">
            <script src="https://kit.fontawesome.com/962ba28c29.js"></script>
            <hgroup>
                <h2><%: Title %></h2>
            </hgroup>

            <br />

            <div class="checklist" style="float: left; width: 150px">
                &emsp;<asp:Label ID="Label1" runat="server" Text="Animal Types" Font-Bold="true"></asp:Label>
                <asp:CheckBoxList ID="CheckBoxList1" runat="server" OnSelectedIndexChanged="CheckBoxList1_SelectedIndexChanged1" AutoPostBack="True" Font-Bold="false" RepeatColumns="4" Width="500">
                    <asp:ListItem></asp:ListItem>
                    <asp:ListItem></asp:ListItem>
                    <asp:ListItem></asp:ListItem>
                    <asp:ListItem></asp:ListItem>
                    <asp:ListItem></asp:ListItem>
                    <asp:ListItem></asp:ListItem>
                    <asp:ListItem></asp:ListItem>
                </asp:CheckBoxList>
            </div>
            <div>
                <asp:Image ID="Profile_Image" runat="server" Style="margin-left: auto; display: block;" ImageUrl="https://playdate.blob.core.windows.net/profilepictures/temp_profile_picture.jpg" Width="150" Height="150" />
                <br />
                <asp:Button class="signinbutton" ID="Profile_Button" runat="server" OnClick="Profile_Button_Click" Text="My Profile" Style="margin-left: auto; display: block;" />
                <asp:Button ID="SignOut_Button" runat="server" OnClick="SignOut_Button_Click" Text="Sign out" Style="margin-left: auto; display: block;" />
            </div>


            <hr />
            <asp:ListView ID="PetList" runat="server"
                DataKeyNames="PartitionKey" GroupItemCount="1"
                ItemType="Playdate.Pet" SelectMethod="GetPets">
                <EmptyDataTemplate>
                    <table>
                        <tr>
                            <td>No match found </td>
                        </tr>
                    </table>
                </EmptyDataTemplate>
                <EmptyItemTemplate>
                    <td>
                    <td />
                </EmptyItemTemplate>
                <GroupTemplate>
                    <tr id="itemPlaceholderContainer" runat="server">
                        <td id="itemPlaceholder" runat="server"></td>
                    </tr>
                </GroupTemplate>
                <ItemTemplate>
                    <td runat="server">
                        <table>
                            <tr>
                                <td>
                                    <a href="https://playdate.blob.core.windows.net/profilepictures/<%#:Item.PicID%>">
                                        <img style="float: left; vertical-align: middle" src="https://playdate.blob.core.windows.net/profilepictures/<%#:Item.PicID%>"
                                            width="150" height="105" /></a>

                                    <span style="vertical-align: top; padding-left: 15px">
                                        <b><%#:Item.RowKey%></b><span style="color: darkgrey">&emsp;<i class="fas fa-map-marker-alt"></i>&nbsp; <%#:Item.City%>, <%#:Item.State%> </span>
                                    </span>
                                    <br />

                                    <div style="margin-right: auto; display: block; width: 950px; padding-left: 165px"><%#:Item.Bio%></div>

                                    <asp:Button ID="Message_Button" runat="server" Style="margin-left: auto; display: block;" Text='<%#: "Message " + Item.RowKey%>' OnClick="Message_Button_Clicked" />
                                </td>

                            </tr>

                            <tr>
                                <td>&nbsp;</td>
                            </tr>
                        </table>
                        </p>
                    </td>
                </ItemTemplate>
                <LayoutTemplate>
                    <table style="width: 100%;">
                        <tbody>
                            <tr>
                                <td>
                                    <table id="groupPlaceholderContainer" runat="server" style="width: 100%">
                                        <tr id="groupPlaceholder"></tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                            </tr>
                            <tr></tr>
                        </tbody>
                    </table>
                </LayoutTemplate>
            </asp:ListView>
        </div>
    </section>
</asp:Content>