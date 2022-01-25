<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pending.aspx.cs" Inherits="canteen_sign_up_admin._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div> <table>  
            <tr>  
                <td>  
                    <asp:FileUpload ID="btnPdfBrowse" runat="server" ToolTip="Wähle nur PDF-Dokumente" /> </td>  
                <td>  
                    <asp:Button ID="btnPdfUpload" runat="server" Text="Hochladen" onclick="btnPdfUpload_Click" /> </td>  
            </tr>  
        </table>  
            <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label>
            <br />
            <asp:GridView ID="gvStudentsData" runat="server" OnRowDataBound="GridViewStudentsData_RowDataBound">
            </asp:GridView>
            <asp:Label ID="lblInfo" runat="server"></asp:Label>
            <br />
            <asp:Button ID="Button1" runat="server" Text="Button" />
        </div>
    </form>
</body>
</html>
