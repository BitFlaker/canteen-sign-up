<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/admin.Master" CodeBehind="pending.aspx.cs" Inherits="canteen_sign_up_admin.pending" Theme="ControlTheme" %>

<asp:Content ID="cttPage" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <!--<div id="dTop">
        <asp:Button runat="server" ID="btnUploadFileYY" OnClick="btnUploadFile_Click" Text="Anmeldeformulare hochladen" SkinID="controlButton" />
    </div>-->
    <asp:Label ID="lblDataHeading" runat="server" CssClass="dataHeading" Text="Ausstehende Bestätigungen"/>
    <asp:Label ID="lblDataInfo" runat="server" CssClass="dataInfo" Text="467 von 10000 Einträgen werden aufgelistet"/>
    <div id="gridContainer">
        <div id="dTop">
            <asp:Button runat="server" ID="btnUploadFile" OnClick="btnUploadFile_Click" Text="Anmeldeformulare hochladen" SkinId="controlButton" />
            <asp:Button runat="server" ID="btnNextEntries" OnClick="btnNextEntries_Click" Text="Nächste Einträge"/>
        </div>
        <asp:GridView ID="gvStudentsData" runat="server" OnRowDataBound="GridViewStudentsData_RowDataBound" HeaderStyle-Wrap="true" RowStyle-Wrap="true" SkinID="controlGridView" CellPadding="4" ForeColor="#333333" GridLines="None">
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
    </div>
    <asp:Label ID="lblInfo" runat="server" />
</asp:Content>
