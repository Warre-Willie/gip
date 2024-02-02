<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dummy-teller.aspx.cs" Inherits="Dummy_gip.Pages.dummy_teller" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>dummy-teller</title>
    <link href="../CSS/Style-dummypage.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Image ID="Image1" runat="server" Height="152px" ImageUrl="images/logo_print.png" Width="232px" />
            <asp:Button ID="btnTeller" runat="server" OnClick="btnTeller_Click" Text="Teller" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search" />
            <br />
            <br />
            <br />
            <h1>Welkom bij de teller</h1>
            <p>
                <asp:Label ID="Label2" runat="server" Text="Give the zone ID"></asp:Label>
&nbsp;&nbsp;
                <asp:TextBox ID="tbId" runat="server" Width="130px" OnTextChanged="tbId_TextChanged"></asp:TextBox>
            </p>
        </div>
        <p>
            <asp:Label ID="Label1" runat="server" Text="Select the input:"></asp:Label>
        </p>
&nbsp;
        <asp:Button ID="btnIn" runat="server" OnClick="btnIn_Click" Text="IN" Width="81px" />
&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnOut" runat="server" OnClick="btnOut_Click" Text="Out" Width="81px" />
&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnTime" runat="server" OnClick="btnTime_Click" Text="Update logbook" Width="146px" />
        <br />
        <br />
        <asp:Label ID="lblCount" runat="server"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Label ID="lblError" runat="server"></asp:Label>
    </form>
</body>
</html>
