<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="inprogress.aspx.cs" Inherits="canteen_sign_up.inprogress" Theme="defaultTheme" MasterPageFile="~/user.Master" %>

<asp:Content ID="cttPage" runat="server" ContentPlaceHolderID="cphUser">
    <div id="message">
        <asp:Label runat="server" Font-Names="Gadugi, Arial" Font-Size="Small" ForeColor="White" ID="lblMessage"/>
    </div>
    
    <div id="formTable">
        <asp:Label ID="txtAccountOwner" runat="server" Font-Names="Gadugi, Arial" Font-Size="Medium"/>
        <br />
        <asp:Label ID="txtZipCodeCity" runat="server" Font-Names="Gadugi, Arial" Font-Size="Medium"/>
        <br />
        <asp:Label ID="txtStreetHouseNr" runat="server" Font-Names="Gadugi, Arial" Font-Size="Medium"/>
        <br />
        <asp:Label ID="txtIBAN" runat="server" Font-Names="Gadugi, Arial" Font-Size="Medium"/>
        <br />
        <asp:Label ID="txtBIC" runat="server" Font-Names="Gadugi, Arial" Font-Size="Medium" />
    </div>

    <div id="checkActions">
        <asp:Button ID="btnEdit" runat="server" Text="Bearbeiten" OnClick="btnEdit_Click" CssClass="submitButton" />
    </div>
</asp:Content>