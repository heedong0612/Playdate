<%@ Page Title="Ugly Main Feed (help)" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="Playdate.Main" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section>
        <div>
            <script src="https://kit.fontawesome.com/962ba28c29.js"></script>
            <hgroup>
                <h2><%: Title %></h2>
            </hgroup>

            <br />

            <div style="float: left; width: 150px">
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
                <asp:Image ID="Profile_Image" runat="server" Style="margin-left:auto; display:block;" ImageUrl="https://playdate.blob.core.windows.net/profilepictures/temp_profile_picture.jpg" Width="150" Height="150"/>
                <br />
                <asp:Button ID="Profile_Button" runat="server" OnClick="Profile_Button_Click" Text="My Profile" Style="margin-left:auto; display:block;" />
                <br />
            </div>
            <asp:ListView ID="PetList" runat="server" 
                DataKeyNames="PartitionKey" GroupItemCount="1"
                ItemType="Playdate.Pet" SelectMethod="GetPets">
                <EmptyDataTemplate>
                    <table>
                        <tr>
                            <td> No match found </td>
                        </tr>
                    </table>
                </EmptyDataTemplate>  
                <EmptyItemTemplate>
                    <td><td/>
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
                                        <img style="float:left; vertical-align:middle" src="https://playdate.blob.core.windows.net/profilepictures/<%#:Item.PicID%>"
                                            width="150" height="105" /></a>

                                            <span style="vertical-align:top; padding-left:15px">
                                                <b><%#:Item.RowKey%></b><span style="color:darkgrey">&emsp;<i class="fas fa-map-marker-alt"></i>&nbsp; <%#:Item.City%>, <%#:Item.State%> </span>
                                            </span>
                                                <br />
                                            
                                    <div Style="margin-right:auto; display:block; width:950px; padding-left:165px"><%#:Item.Bio%></div>
                                             
                                    <asp:Button ID="Message_Button" runat="server" Style="margin-left:auto; display:block;" Text=<%#: "Message " + Item.RowKey%> OnClick="Message_Button_Clicked" /> 
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
                    <table style="width:100%;">
                        <tbody>
                            <tr>
                                <td>
                                    <table id="groupPlaceholderContainer" runat="server" style="width:100%">
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
