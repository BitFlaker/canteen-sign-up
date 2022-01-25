<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="confirmed.aspx.cs" MasterPageFile="~/admin.Master" Inherits="canteen_sign_up_admin.confirmed" %>

<asp:Content ID="cttPage" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <asp:GridView ID="gvStudentsData" runat="server" OnRowDataBound="GridViewStudentsData_RowDataBound">
    </asp:GridView>
</asp:Content>