<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dummy-search.aspx.cs" Inherits="Dummy_gip.Pages.dummy_search" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">

h1{
    color: black;
}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Image ID="Image1" runat="server" Height="152px" ImageUrl="~/Images/logo_print.png" Width="232px" />
            &nbsp;
            <asp:Button ID="btnTeller" runat="server" OnClick="btnTeller_Click" Text="Teller" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnZoekenPG" runat="server" OnClick="btnZoekenPG_Click" Text="Search" />
            <br />
            <br />
            <h1>Welkom bij de ticket zoeker</h1>
            <p>&nbsp;</p>
            <p>
                <asp:Label ID="lblTicket" runat="server" Text="Ticket number: "></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:TextBox ID="tbTknb" runat="server" Width="238px"></asp:TextBox>
            </p>
            <p>&nbsp;
                <asp:Button ID="bttSearch" runat="server" OnClick="bttSearch_Click" Text="Search" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="lblError" runat="server"></asp:Label>
            </p>
            <p>
                <asp:TextBox ID="tbOutput" runat="server" Height="142px" TextMode="MultiLine" Width="726px"></asp:TextBox>
            </p>
        </div>
    </form>
</body>
</html>
