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
            <div id="headingContainer">
                <div id="gradientOverlay">
                    <asp:Image runat="server" CssClass="htlLogo" ImageUrl="~/images/HTLVB_Logo.png" Height="75px" />
                    <asp:Image runat="server" ImageUrl="~/images/MensaLogo.png" Height="100px" />
                    <h1 id="overallHeading">MENSA - Anmeldung</h1>
                </div>
            </div>
            <asp:Image runat="server" CssClass="progressImage" ImageUrl="~/images/ProgressSymbol_Progress1.svg" />
            <div id="message">
                <asp:Label runat="server" ForeColor="White" Text="Hallo --FANME-- --LNAME--,<br /><br />Fülle die nachfolgenden Daten aus, um dich bei der Mensa zu registrieren. Nach dem Absenden des Formulars muss eien Bestätigung gedruckt, unterschrieben und abschließend abgegeben werden."/>
            </div>
            <div id="formTable">
                <div class="formDiv" style="display:flex; width: 100%;">
                    <asp:TextBox CssClass="formField" ID="txtFirstname" runat="server" Placeholder="Vorname des Kontoinhabers" Width="100%" SkinID="inputField"/>
                    <asp:TextBox CssClass="formField" ID="txtLastname" runat="server" Placeholder="Nachname des Kontoinhabers" Width="100%" SkinID="inputField"/>
                </div>
                <div class="formDiv" style="display:flex; width: 100%;">
                    <asp:TextBox CssClass="formField" ID="txtStreet" runat="server" Placeholder="Straße" Width="100%" SkinID="inputField"/>
                    <asp:TextBox CssClass="formField" ID="txtHouseNumber" runat="server" Placeholder="Hausnummer" Width="30%" SkinID="inputField"/>
                </div>
                <div class="formDiv" style="display:flex; width: 100%;">
                    <asp:TextBox CssClass="formField" ID="txtZipCode" runat="server" Placeholder="Postleitzahl" Width="30%" SkinID="inputField"/>
                    <asp:TextBox CssClass="formField" ID="txtCity" runat="server" Placeholder="Stadt" Width="100%" SkinID="inputField"/>
                </div>
                <div class="formDiv" style="display:flex; width: 100%;">
                    <asp:TextBox CssClass="formField" ID="txtIban" runat="server" Placeholder="IBAN" Width="100%" SkinID="inputField"/>
                </div>
                <div class="formDiv" style="display:flex; width: 100%; text-align:left;">
                    <asp:TextBox CssClass="formField" ID="txtBic" runat="server" Placeholder="BIC (optional)" Width="30%" SkinID="inputField"/>
                </div>
            </div>
            <div id="submitContainer">
                <asp:Button runat="server" Text="Absenden und drucken" CssClass="submitButton" ID="btnSend" OnClick="btnSend_Click"/>
            </div>
            <asp:Label ID="lblInfo" runat="server" ForeColor="Orange"></asp:Label>
        </div>
    </form>
</body>
</html>
