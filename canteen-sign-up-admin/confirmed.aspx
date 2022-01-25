<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="confirmed.aspx.cs" MasterPageFile="~/admin.Master" Inherits="canteen_sign_up_admin.confirmed" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
        <asp:GridView ID="gvStudentsData" runat="server" OnRowDataBound="GridViewStudentsData_RowDataBound">
            </asp:GridView>
    </form>
</body>
</html>
