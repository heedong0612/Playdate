<%@ Page Title="Playdate" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="Playdate.Main" %>


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
                <h2 class="PLAYDATE">
                    <asp:Label ID="playdate_label" runat="server" Text="Playdate"></asp:Label>
                    
                </h2>
            </hgroup>
            <br />

            <div class="checklist" style="float: left; width: 150px; font-size:medium;">
                <asp:Label class="calibri" ID="Label1" runat="server" Text="Animal Types" Font-Bold="true" Font-Size="Large"></asp:Label>
                <asp:CheckBoxList class="calibri" ID="CheckBoxList1" runat="server" OnSelectedIndexChanged="CheckBoxList1_SelectedIndexChanged1" AutoPostBack="True" Font-Bold="false" RepeatColumns="4" Width="500">
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
                <asp:Image ID="Profile_Image" runat="server" Style="margin-left: auto; display: block;" ImageUrl="https://playdate.blob.core.windows.net/profilepictures/temp_profile_picture.jpg" Width="170" Height="120"/>
                <asp:Label ID="Label2" runat="server" Text="Note for CSS 436 classmates:" ForeColor="#fdb65c" Font-Bold="true"></asp:Label>
                <asp:Label ID="Temp_Label" runat="server" Text="<br /> It will be more fun if you message the accounts that don't have [Fake account] tag in bio. Enjoy! :D - Donghee, Kaity & Jessica"></asp:Label>
                <br />
                <asp:Button class="signinbutton" ID="Profile_Button" runat="server" OnClick="Profile_Button_Click" Text="My Profile" Style="margin-left: auto; display: block;" Height="34px" Width="95px" />
                <br />
                <asp:Button ID="SignOut_Button" runat="server" OnClick="SignOut_Button_Click" Text="Sign out" Style="margin-left: auto; display: block;" Height="33px" Width="95px" />
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
                                        <img style="float: left; vertical-align: middle;" src="https://playdate.blob.core.windows.net/profilepictures/<%#:Item.PicID%>"
                                            width="200" height="145" /></a>

                                    <span style="vertical-align: top; padding-left: 15px; font-size:17px">
                                        <b><%#:Item.RowKey%></b><span style="color: darkgrey">&emsp;<i class="fas fa-map-marker-alt"></i>&nbsp; <%#:Item.City%>, <%#:Item.State%> 
                                           
                                    </span>
                                    
                                    <br /><br />
                                    <div style="margin-right: auto; display: block; width: 1100px; padding-left: 215px; font-size:17px; font-family:Calibri"><%#:Item.Bio%></div>
                                     
                                    <asp:Button ID="Message_Button" runat="server" Style="margin-left: auto; text-align:right; display: block;" Text='<%#: "Message " + Item.RowKey%>' OnClick="Message_Button_Clicked" CommandArgument='<%#: Item.PartitionKey +","+ Item.RowKey%>' />
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