<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/admin.Master" CodeBehind="confirmed.aspx.cs" Inherits="canteen_sign_up_admin.confirmed" Theme="ControlTheme" %>

<asp:Content ID="cttPage" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <div style="width:fit-content;margin-left:auto;margin-right:auto;margin-top:40px;">
        <asp:Label ID="lblDataHeading" runat="server" CssClass="dataHeading" Text="Bestätigte Registrierungen"/>
        <div style="display:flex;justify-content:space-between;">
            <div>
                <asp:Button runat="server" ID="btnActivate" Text="Aktiv setzen" OnClick="btnActivate_Click" CssClass="ActionButton"/>
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
        <asp:GridView runat="server" ID="grdData" AutoGenerateColumns="true" AutoGenerateSelectButton="true" CssClass="DynTable" BackColor="White"/>
        <asp:Label runat="server" ID="lblPageInfo" CssClass="PageInfo" />
        <asp:Label ID="lblInfo" runat="server" />
    </div>
</asp:Content>
