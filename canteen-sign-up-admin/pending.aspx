<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/admin.Master" CodeBehind="pending.aspx.cs" Inherits="canteen_sign_up_admin.pending" Theme="ControlTheme" %>

<asp:Content ID="cttPage" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <div style="width:fit-content;margin-left:auto;margin-right:auto;margin-top:40px;">
        <asp:Label ID="lblDataHeading" runat="server" CssClass="dataHeading" Text="Ausstehende Bestätigungen"/>
        <asp:Label ID="lblDataInfo" runat="server" CssClass="dataInfo" Text="467 von 10000 Einträgen werden aufgelistet"/>
        <asp:Button runat="server" ID="btnUploadFile" OnClick="btnUploadFile_Click" Text="Anmeldeformulare hochladen" SkinId="controlButton" />
        <div style="display:flex;justify-content:space-between;">
            <div>
                <asp:Button runat="server" ID="btnActivate" Text="Aktiv setzen" OnClick="btnAvtivate_Click" CssClass="ActionButton"/>
            </div>
            <div>
                <asp:DropDownList runat="server" ID="ddlEntriesPerPage" AutoPostBack="True" OnSelectedIndexChanged="ddlEntriesPerPage_SelectedIndexChanged" CssClass="ActionSelect">
                    <asp:ListItem Value="10"></asp:ListItem>
                    <asp:ListItem Selected="True" Value="50"></asp:ListItem>
                    <asp:ListItem Value="100"></asp:ListItem>
                    <asp:ListItem Value="150"></asp:ListItem>
                    <asp:ListItem Value="200"></asp:ListItem>
                    <asp:ListItem Value="500"></asp:ListItem>
                </asp:DropDownList>
                <asp:Button runat="server" ID="btnPrev" Text="Vorherige Seite" Enabled="false" OnClick="btnPrev_Click" CssClass="ActionButton"/>
                <asp:Button runat="server" ID="btnNext" Text="Nächste Seite" OnClick="btnNext_Click" CssClass="ActionButton"/>
            </div>
        </div>
        <asp:GridView runat="server" ID="grdData" AutoGenerateColumns="true" AutoGenerateSelectButton="true" CssClass="DynTable"/>
        <asp:Label runat="server" ID="lblPageInfo" CssClass="PageInfo" />
        <asp:Label ID="lblInfo" runat="server" />
    </div>
</asp:Content>
