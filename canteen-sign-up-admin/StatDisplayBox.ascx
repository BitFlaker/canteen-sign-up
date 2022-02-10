<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StatDisplayBox.ascx.cs" Inherits="canteen_sign_up_admin.StatDisplayBox" %>

<asp:Panel ID="pnlBackground" runat="server" CssClass="card green">
    <asp:Label ID="lblHeading" runat="server" CssClass="cardSecondaryText">Aktuelle Einträge</asp:Label>
    <asp:Label ID="lblContent" runat="server" CssClass="cardPrimaryText">1237</asp:Label>
    <asp:Image runat="server" ImageUrl="~/images/Celtic.png" CssClass="cardOverlayImage"/>
</asp:Panel>