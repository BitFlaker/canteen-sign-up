<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="canteen_sign_up._default" Theme="defaultTheme" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="styles/styles.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div style="overflow: hidden;">
            <asp:Image runat="server" CssClass="htlLogo" ImageUrl="~/images/HTLVB_Logo.png" Height="75px" />
            <asp:Image runat="server" ImageUrl="~/images/MensaLogo.png" Height="100px" />
            <h1 id="overallHeading">MENSA - Anmeldung</h1>
            <asp:Label ID="lblInfo" runat="server" ForeColor="Orange"></asp:Label>
            <!-- Progress icons -->
            <!-- Info message -->
            <div id="formTable">
                <div class="formDiv" style="display:flex; width: 100%;">
                    <asp:TextBox CssClass="formField" ID="txtFirstname" runat="server" Width="100%" SkinID="inputField"/>
                    <asp:TextBox CssClass="formField" ID="txtLastname" runat="server" Width="100%" SkinID="inputField"/>
                </div>
                <div class="formDiv" style="display:flex; width: 100%;">
                    <asp:TextBox CssClass="formField" ID="txtStreet" runat="server" Width="100%" SkinID="inputField"/>
                    <asp:TextBox CssClass="formField" ID="txtHouseNumber" runat="server" Width="30%" SkinID="inputField"/>
                </div>
                <div class="formDiv" style="display:flex; width: 100%;">
                    <asp:TextBox CssClass="formField" ID="txtZipCode" runat="server" Width="30%" SkinID="inputField"/>
                    <asp:TextBox CssClass="formField" ID="txtCity" runat="server" Width="100%" SkinID="inputField"/>
                </div>
                <div class="formDiv" style="display:flex; width: 100%;">
                    <asp:TextBox CssClass="formField" ID="txtIban" runat="server" Width="100%" SkinID="inputField"/>
                </div>
                <div class="formDiv" style="display:flex; width: 100%; text-align:left;">
                    <asp:TextBox CssClass="formField" ID="txtBic" runat="server" Width="100%" SkinID="inputField"/>
                </div>
            </div>
            <div id="submitContainer">
                <asp:Button runat="server" Text="Absenden und drucken" CssClass="submitButton" ID="btnSend" OnClick="btnSend_Click"/>
            </div>
        </div>
    </form>
</body>
</html>
