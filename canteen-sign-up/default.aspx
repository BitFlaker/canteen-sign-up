<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="canteen_sign_up._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Button ID="btnSend" runat="server" OnClick="btnSend_Click" Text="Button" />
            <br />
            <asp:Label ID="lblInfo" runat="server" ForeColor="Red"></asp:Label>
        </div>
    </form>
</body>
</html>
