<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DialogBox.ascx.cs" Inherits="canteen_sign_up_admin.DialogBox" %>

<link href="styles/default.css" rel="stylesheet" type="text/css" />

<div id="background">
    <div id="dialogWindow" style="text-align: center;">
        <asp:Label runat="server" ID="txtTitle" Font-Bold="True" Font-Size="Large">Title</asp:Label>
        <div id="dialogContent" runat="server" style="margin-top: 30px;margin-bottom: 30px; display: grid; justify-items: center;"></div>
        <div style="display: block;">
            <asp:Button ID="btnOk" runat="server" Text="Ok" OnClick="btnOk_Click" CssClass="cornerRadius" Height="25" />
            <asp:Button ID="btnCancel" runat="server" Text="Abbrechen" OnClick="btnCancel_Click" CssClass="cornerRadius" Height="25"/>
        </div>
    </div>
</div>