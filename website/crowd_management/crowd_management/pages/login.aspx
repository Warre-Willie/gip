<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="crowd_management.pages.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Login</title>

    <link rel="shortcut icon" href="../image/favicon.png" type="image/x-icon" />
    <script src="https://kit.fontawesome.com/08c8f3812a.js" crossorigin="anonymous"></script>
    <script src="../js/preventBack.js"></script>
    <script src="../js/preventBackLogin.js"></script>

    <!-- CSS -->
    <link rel="stylesheet" href="../css/bulma.css">
    <link rel="stylesheet" href="../css/style.css">
    <link rel="stylesheet" href="../css/bulma-tooltip.css">
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <div class="image-container">
                <img src="../image/logo_print.png" />
            </div>
            <div class="box">
                <div class="field">
                    <p class="control has-icons-right">
                        <asp:TextBox ID="tbEmail" CssClass="input" runat="server" placeholder="Email"></asp:TextBox>
                        <span class="icon is-small is-right">
                            <i class="fas fa-envelope"></i>
                        </span>
                    </p>
                </div>
                <div class="field">
                    <p class="control has-icons-right">
                        <asp:TextBox ID="tbWW" CssClass="input" runat="server" placeholder="Wachtwoord" type="password"></asp:TextBox>
                        <span class="icon is-small is-right">
                            <i class="fas fa-lock"></i>
                        </span>
                    </p>
                </div>
                <div class="field">
                    <p class="control">
                        <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="button is-warning" OnClick="btnLogin_Click" />
                    </p>
                </div>
                <asp:Label ID="lbError" runat="server" Text="Ongeldige login poging" CssClass="has-text-danger" Visible="false"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>
