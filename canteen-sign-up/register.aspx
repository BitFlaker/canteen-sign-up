<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="register.aspx.cs" Inherits="canteen_sign_up._default" Theme="defaultTheme" MasterPageFile="~/user.Master" %>

<asp:Content ID="cttPage" runat="server" ContentPlaceHolderID="cphUser">
    <div id="message">
        <asp:Label runat="server" Font-Names="Gadugi, Arial" Font-Size="Small" ForeColor="White" ID="lblMessage"/>
    </div>
    <div id="formTable">
        <div class="formDiv" style="display:flex; width: 100%;">
            <asp:TextBox CssClass="formField" ID="txtFirstname" runat="server" Placeholder="Vorname des Kontoinhabers" Width="100%" SkinID="inputField"/>
            <asp:RequiredFieldValidator ID="reqFldValFirstname" runat="server" Text="" ControlToValidate="txtFirstname" EnableClientScript="False" ErrorMessage="Kontobesitzer Vorname benötigt" Display="None"></asp:RequiredFieldValidator>
            <asp:TextBox CssClass="formField" ID="txtLastname" runat="server" Placeholder="Nachname des Kontoinhabers" Width="100%" SkinID="inputField"/>
            <asp:RequiredFieldValidator ID="reqFldValLastname" runat="server" Text=" " ControlToValidate="txtLastname" EnableClientScript="False" ErrorMessage="Kontobesitzer Nachname benötigt" Display="None"></asp:RequiredFieldValidator>
        </div>
        <div class="formDiv" style="display:flex; width: 100%;">
            <asp:TextBox CssClass="formField" ID="txtZipCode" runat="server" Placeholder="Postleitzahl" Width="30%" SkinID="inputField"/>
            <asp:RequiredFieldValidator ID="reqFldValZipCode" runat="server" Text="" ControlToValidate="txtZipCode" EnableClientScript="False" ErrorMessage="PLZ benötigt" Display="None"></asp:RequiredFieldValidator>
            <asp:TextBox CssClass="formField" ID="txtCity" runat="server" Placeholder="Ort" Width="100%" SkinID="inputField"/>
             <asp:RequiredFieldValidator ID="redFldValCity" runat="server" Text="" ControlToValidate="txtCity" EnableClientScript="False" ErrorMessage="Wohnort benötigt" Display="None"></asp:RequiredFieldValidator>
        </div>
        <div class="formDiv" style="display:flex; width: 100%;">
            <asp:TextBox CssClass="formField" ID="txtStreet" runat="server" Placeholder="Straße" Width="100%" SkinID="inputField"/>
             <asp:RequiredFieldValidator ID="reqFldValStreet" runat="server" Text="" ControlToValidate="txtStreet" EnableClientScript="False" ErrorMessage="Straße benötigt" Display="None"></asp:RequiredFieldValidator>
            <asp:TextBox CssClass="formField" ID="txtHouseNumber" runat="server" Placeholder="Hausnummer" Width="30%" SkinID="inputField"/>
             <asp:RequiredFieldValidator ID="reqFldValHouseNumber" runat="server" Text="" ControlToValidate="txtHouseNumber" EnableClientScript="False" ErrorMessage="Hausnummer benötigt" Display="None"></asp:RequiredFieldValidator>
        </div>
        <div class="formDiv" style="display:flex; width: 100%;">
            <asp:TextBox CssClass="formField" ID="txtIban" runat="server" Placeholder="IBAN" Width="100%" SkinID="inputField"/>
             <asp:RequiredFieldValidator ID="reqFldValIban" runat="server" Text="" ControlToValidate="txtIban" EnableClientScript="False" ErrorMessage="Iban benötigt" Display="None"></asp:RequiredFieldValidator>
            <asp:CustomValidator ID="custValIban" runat="server" Text="" ControlToValidate="txtIban" EnableClientScript="false" ErrorMessage="Iban ist nicht gültig" Display="None" OnServerValidate="custValIban_ServerValidate"></asp:CustomValidator>
        </div>
        <div class="formDiv" style="display:flex; width: 100%; text-align:left;">
            <asp:TextBox CssClass="formField" ID="txtBic" runat="server" Placeholder="BIC (optional)" Width="30%" SkinID="inputField"/>
        </div>
    </div>
    <div id="ValidationSummary">
        <asp:ValidationSummary id="validationSummary" runat="server" CssClass="validationSummary" DisplayMode="List" EnableClientScript="False"/>
    </div>
    <div id="submitContainer">
        <asp:Button runat="server" Text="Absenden und drucken" CssClass="submitButton" ID="btnSendAndPrint" OnClick="btnSendAndPrint_Click"/>
    </div>
    <asp:Label ID="lblInfo" runat="server" ForeColor="Orange" Visible="false"></asp:Label>
</asp:Content>