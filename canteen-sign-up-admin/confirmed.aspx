<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="confirmed.aspx.cs" MasterPageFile="~/admin.Master" Inherits="canteen_sign_up_admin.confirmed" Theme="ControlTheme" %>

<asp:Content ID="cttPage" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <asp:Label ID="lblDataHeading" runat="server" CssClass="dataHeading" Text="Angemeldeten und Inaktivliste"/>
    <asp:Label ID="lblDataInfo" runat="server" CssClass="dataInfo" Text="67 von 1000 Einträgen werden aufgelistet"/>
    <asp:GridView ID="gvStudentsData" runat="server" OnRowDataBound="GridViewStudentsData_RowDataBound" HeaderStyle-Wrap="true" RowStyle-Wrap="true" CellPadding="4" ForeColor="#333333" GridLines="None" SkinID="controlGridView">
        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        <EditRowStyle BackColor="#999999" />
        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
        <SortedAscendingCellStyle BackColor="#E9E7E2" />
        <SortedAscendingHeaderStyle BackColor="#506C8C" />
        <SortedDescendingCellStyle BackColor="#FFFDF8" />
        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
    </asp:GridView>
</asp:Content>