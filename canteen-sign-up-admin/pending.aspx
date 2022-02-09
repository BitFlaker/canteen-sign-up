<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/admin.Master" CodeBehind="pending.aspx.cs" Inherits="canteen_sign_up_admin.pending" Theme="ControlTheme" %>

<asp:Content ID="cttPage" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <div style="width:fit-content;margin-left:auto;margin-right:auto;margin-top:40px;">
        <div class="cardHolder">
            <div class="card yellow">
                <p class="cardPrimaryText">1237</p>
                <p class="cardSecondaryText">Personen ausstehend</p>
                <asp:Image runat="server" ImageUrl="~/images/Celtic.png" CssClass="cardOverlayImage"/>
            </div>
            <div class="card green">
                <p class="cardPrimaryText">51</p>
                <p class="cardSecondaryText">Personen aktiv</p>
                <asp:Image runat="server" ImageUrl="~/images/Celtic.png" CssClass="cardOverlayImage"/>
            </div>
            <div class="card red">
                <p class="cardPrimaryText">8395</p>
                <p class="cardSecondaryText">Personen deaktiviert</p>
                <asp:Image runat="server" ImageUrl="~/images/Celtic.png" CssClass="cardOverlayImage"/>
            </div>
        </div>
        <div style="display:flex;justify-content:space-between;margin-top:50px;">
            <div>
                <asp:Button runat="server" ID="btnUploadFile" Text="Anmeldeformulare hochladen" OnClick="btnUploadFile_Click" CssClass="ActionButton" />
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
