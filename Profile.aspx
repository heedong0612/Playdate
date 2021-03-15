<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="Playdate.Profile" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="./style.css">
    <link rel="preconnect" href="https://fonts.gstatic.com">
    <link href="https://fonts.googleapis.com/css2?family=Pacifico&family=Raleway&display=swap" rel="stylesheet">
    <%--  <script src='https://code.jquery.com/jquery-3.5.1.slim.min.js'></script>
    <script src='https://cdn.jsdelivr.net/npm/bootstrap@4.6.0/dist/js/bootstrap.bundle.min.js'></script>--%>

    <style>
        article, aside, figure, footer, header, hgroup,
        menu, nav, section {
            display: block;
        }
    </style>
    <%--<script src="./script.js" type="text/javascript"></script>--%>
    <script>
        // JavaScript source code
        function readURL(input) {
            $('#ProfilePic').attr('src', "#");
            if (input.files && input.files[0]) {
                var reader = new FileReader();

                reader.onload = function (e) {
                    $('#ProfilePic')
                        .attr('src', e.target.result)
                        .width(150)
                        .height(200);
                };
                // document.getElementById("Label2").Text = "LOADED JAVASCRIPT";
                reader.readAsDataURL(input.files[0]);
            }
        }
    </script>
    <h2>Profile Page</h2>
    <div class="body">

        <div class="container">

            <img class="LOGO" src="https://playdate.blob.core.windows.net/profilepictures/LOGO.png" runat="server" />
            <br />
            <asp:Label class="applabel" ID="AppName" runat="server" Text="PLAYDATE"></asp:Label>

            <hr />
            <br />
            <br />

            <div class="savebutton">
                <asp:Label class="left_align" ID="NameTextBox" runat="server" Text="Pet Name: "></asp:Label>
            </div>
            <%--  <asp:TextBox ID="NameTextBox" runat="server"></asp:TextBox>--%>
            <br />
            <br />

            <table>
                <tr>
                    <td>
                        <asp:Label ID="Age" runat="server" Text="Age:"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="AgeTextBox" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Animal" runat="server" Text="Animal Type:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="AnimalTextBox" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="City" runat="server" Text="City:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="CityTextBox" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="State" runat="server" Text="State:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="StateTextBox" runat="server"></asp:TextBox>
                    </td>
                </tr>


            </table>

            <br />
            <asp:Label ID="Bio" runat="server" Text="Upload your Bio:"></asp:Label>
            <br />
            <asp:TextBox class="BioTextBox" TextMode="MultiLine" ID="BioTextBox" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="Limit" runat="server" Text="Word Limit: 20 words"></asp:Label>

            <br />
            <hr />
            <br />

            <p>Upload your Profile Picture Here:</p>
            <asp:FileUpload ID="PhotoUpload" runat="server" Text="Upload Picture" onchange="readURL(this);" accept="image/jpeg, image/png" />
            <br />
            <img id="ProfilePic" src="#" runat="server" alt="Your Pet Profile Picture" />
            <br />
            <br />
            <asp:Label class="error" ID="Label1" runat="server" Text=""></asp:Label>

            <br />

            <br />

            <div class="savebutton">
                <asp:Button ID="Save" runat="server" Text="Save" OnClick="Save_Click" />
            </div>
            <br />

            <br />

            <%--error label--%>
            <asp:Label class="error" ID="Label2" runat="server" Text=""></asp:Label>


            <br />
            <br />


            <br />

            <img class="HomePic" src="https://playdate.blob.core.windows.net/profilepictures/HOME.png" runat="server" />
            <asp:Button class="signinbutton" ID="Home" runat="server" Text="Back To Main" OnClick="Main_Click" />


        </div>
        <!--end container-->

    </div>
</asp:Content>
