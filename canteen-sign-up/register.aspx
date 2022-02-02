<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="register.aspx.cs" Inherits="canteen_sign_up._default" Theme="defaultTheme" MasterPageFile="~/user.Master" %>

<asp:Content ID="cttPage" runat="server" ContentPlaceHolderID="cphUser">
    <div id="message">
        <asp:Label runat="server" Font-Names="Gadugi, Arial" Font-Size="Small" ForeColor="White" ID="lblMessage"/>
    </div>
    <div id="formTable">
        <div class="formDiv" style="display:flex; width: 100%;">
            <asp:TextBox CssClass="formField" ID="txtFirstname" runat="server" Placeholder="Vorname des Kontoinhabers" Width="100%" SkinID="inputField"/>
            <asp:TextBox CssClass="formField" ID="txtLastname" runat="server" Placeholder="Nachname des Kontoinhabers" Width="100%" SkinID="inputField"/>
        </div>
        <div class="formDiv" style="display:flex; width: 100%;">
            <asp:TextBox CssClass="formField" ID="txtZipCode" runat="server" Placeholder="Postleitzahl" Width="30%" SkinID="inputField"/>
            <asp:TextBox CssClass="formField" ID="txtCity" runat="server" Placeholder="Ort" Width="100%" SkinID="inputField"/>
        </div>
        <div class="formDiv" style="display:flex; width: 100%;">
            <asp:TextBox CssClass="formField" ID="txtStreet" runat="server" Placeholder="Straße" Width="100%" SkinID="inputField"/>
            <asp:TextBox CssClass="formField" ID="txtHouseNumber" runat="server" Placeholder="Hausnummer" Width="30%" SkinID="inputField"/>
        </div>
        <div class="formDiv" style="display:flex; width: 100%;">
            <asp:TextBox CssClass="formField" ID="txtIban" runat="server" Placeholder="IBAN" Width="100%" SkinID="inputField"/>
        </div>
        <div class="formDiv" style="display:flex; width: 100%; text-align:left;">
            <asp:TextBox CssClass="formField" ID="txtBic" runat="server" Placeholder="BIC (optional)" Width="30%" SkinID="inputField"/>
        </div>
    </div>
    <div id="submitContainer">
        <asp:Button runat="server" Text="Absenden und drucken" CssClass="submitButton" ID="btnSendAndPrint" OnClick="btnSendAndPrint_Click"/>
    </div>
    <asp:Label ID="lblInfo" runat="server" ForeColor="Orange" Visible="false"></asp:Label>
</asp:Content>
