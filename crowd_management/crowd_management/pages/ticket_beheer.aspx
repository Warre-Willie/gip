<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ticket_beheer.aspx.cs" Inherits="crowd_management.pages.ticket_beheer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    <link rel="shortcut icon" href="../image/favicon.png" type="image/x-icon"/>
    <title>Ticket beheer</title>

    <script src="https://kit.fontawesome.com/08c8f3812a.js" crossorigin="anonymous"></script>

    <!-- CSS -->
    <link rel="stylesheet" href="../css/bulma.css">
    <link rel="stylesheet" href="../css/style.css">
    <link rel="stylesheet" href="../css/bulma-tooltip.css">
</head>
<body>
    <form id="form1" runat="server">
        <nav class="navbar is-link" role="navigation" aria-label="main navigation">
            <div class="navbar-brand">
                <a class="navbar-item navbar-brand-container">
                    <img class="navbar-brand-img" src="../image/logo_navbar.png">
                </a>

                <a role="button" class="navbar-burger" aria-label="menu" aria-expanded="false" data-target="navbarBasicExample">
                    <span aria-hidden="true"></span>
                    <span aria-hidden="true"></span>
                    <span aria-hidden="true"></span>
                </a>
            </div>
            <div id="navbar" class="navbar-menu">
                <div class="navbar-start">
                    <a href="index.aspx" class="navbar-item">Crowd management
                    </a>

                    <a href="ticket_beheer.aspx" class="navbar-item">Ticket beheer
                    </a>

                    <a href="rapport.aspx" class="navbar-item">Rapport maken
                    </a>
                </div>

                <!-- <div class="navbar-end">
            <div class="navbar-item">
                <div class="buttons">
                    <a class="button is-warning">
                        <strong>Login</strong>
                    </a>
                </div>
            </div>
        </div> -->
            </div>
        </nav>
        <div class="page-content">
            <div id="modal-js-example" class="modal">
                <div class="modal-background"></div>
                <div class="modal-card ticket-connect-modal">
                    <header class="modal-card-head">
                        <p class="modal-card-title">
                            <span class="icon-text">
                                <span class="icon">
                                    <i class="fa-solid fa-link"></i>
                                </span>
                                <span>Badge koppelen</span>
                            </span>
                        </p>
                    </header>
                    <section class="modal-card-body">
                        <figure class="image is-128x128 ticket-barcode-center">
                            <img src="https://bulma.io/images/placeholders/128x128.png">
                        </figure>
                    </section>
                    <footer class="modal-card-foot">
                        <button class="button is-link">Gereed</button>
                        <button class="button">Anuleer</button>
                    </footer>
                </div>
            </div>
            <div class="tile is-ancestor tile-padding">
                <div class="tile is-vertical is-8 fit-tile-content">
                    <div class="tile is-parent">
                        <article class="tile is-child box">
                            <p class="subtitle"><b>Ticket teller</b></p>
                            <div class="content">
                                <progress class="progress is-link" value="50" max="100"></progress>
                                500/1000
                            </div>
                        </article>
                    </div>
                    <div class="tile is-parent">
                        <article class="tile is-child box">
                            <div class="content">
                                <nav class="panel">
                                    <span class="panel-heading ticket-panel-container">Tickets zoeken
                                        <div class="select is-small ticket-dropdown">
                                            <select>
                                                <option>Barcode</option>
                                                <option>Badgerechten</option>
                                            </select>
                                        </div>
                                    </span>
                                    <div class="panel-block">
                                        <p class="control has-icons-left">
                                            <asp:TextBox ID="tbTicketFilter" runat="server" class="input"  placeholder="Ticket zoeken" onkeydown="return (event.keyCode!=13);" onkeyup="searchTicket();"></asp:TextBox>
                                            <span class="icon is-left">
                                                <i class="fas fa-search" aria-hidden="true"></i>
                                            </span>
                                        </p>
                                    </div>
                                    <div id="divTicketList" class="ticket-list">
                                        <a class="panel-block is-active">
                                            <span class="panel-icon">
                                                <i class="fa-solid fa-ticket"></i>
                                            </span>
                                            1000002556
                                        </a>
                                        <a class="panel-block is-active">
                                            <span class="panel-icon">
                                                <i class="fa-solid fa-ticket"></i>
                                            </span>
                                            1000002557
                                        </a>
                                        <a class="panel-block is-active">
                                            <span class="panel-icon">
                                                <i class="fa-solid fa-ticket"></i>
                                            </span>
                                            1000002558
                                        </a>
                                        <a class="panel-block is-active">
                                            <span class="panel-icon">
                                                <i class="fa-solid fa-ticket"></i>
                                            </span>
                                            1000002559
                                        </a>
                                        <a class="panel-block is-active">
                                            <span class="panel-icon">
                                                <i class="fa-solid fa-ticket"></i>
                                            </span>
                                            1000002561
                                        </a>
                                        <a class="panel-block is-active">
                                            <span class="panel-icon">
                                                <i class="fa-solid fa-ticket"></i>
                                            </span>
                                            1000002562
                                        </a>
                                        <a class="panel-block is-active">
                                            <span class="panel-icon">
                                                <i class="fa-solid fa-ticket"></i>
                                            </span>
                                            1000002563
                                        </a>
                                        <a class="panel-block is-active">
                                            <span class="panel-icon">
                                                <i class="fa-solid fa-ticket"></i>
                                            </span>
                                            1000002564
                                        </a>
                                        <a class="panel-block is-active">
                                            <span class="panel-icon">
                                                <i class="fa-solid fa-ticket"></i>
                                            </span>
                                            1000002565
                                        </a>
                                    </div>
                                    <div class="panel-block">
                                        <asp:Button ID="btnClearFilter" runat="server" class="button is-link is-outlined is-fullwidth" Text="Verwijder filter" OnClientClick="clearInput(); return false;"/>
                                    </div>
                                </nav>
                            </div>
                        </article>
                    </div>
                </div>
                <div class="tile is-parent fit-tile-content">
                    <article class="tile is-child box">
                        <div class="content">
                            <p class="subtitle">
                                <b>Ticket instellingen</b>
                            </p>
                            <div class="content">
                                <div class="card">
                                    <header class="card-header">
                                        <p class="card-header-title">
                                            <span class="icon-text ">
                                                <span class="icon">
                                                    <i class="fa-solid fa-arrows-rotate"></i>
                                                </span>
                                            </span>
                                            <span>Opnieuw koppelen
                                            </span>
                                        </p>
                                    </header>
                                    <div class="card-content">
                                        <div class="content">
                                            <asp:Button ID="btnConnectTicket" runat="server" Text="Opnieuw koppelen" class="button is-danger js-modal-trigger" data-target="modal-js-example"  OnClientClick="return false;"/>
                                        </div>
                                    </div>
                                </div>
                                <br>
                                <div class="card">
                                    <header class="card-header">
                                        <p class="card-header-title">
                                            <span class="icon-text ">
                                                <span class="icon">
                                                    <i class="fa-regular fa-id-badge"></i>
                                                </span>
                                            </span>
                                            <span>Badgerechten
                                            </span>
                                        </p>
                                    </header>
                                    <div class="card-content">
                                        <table class="table">
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        <label class="checkbox">
                                                            <asp:CheckBox ID="cbBadgeRight01" runat="server" />
                                                            Kamping
                                                        </label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <label class="checkbox">
                                                            <asp:CheckBox ID="cbBadgeRight02" runat="server" />
                                                            VIP
                                                        </label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <label class="checkbox">
                                                            <asp:CheckBox ID="cbBadgeRight03" runat="server" />
                                                            Backstage
                                                        </label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <label class="checkbox">
                                                            <asp:CheckBox ID="cbBadgeRight04" runat="server" />
                                                            Artiest
                                                        </label>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                    </article>
                </div>
            </div>
            </article>
        </div>

        <div class="footer has-background-white ">
            Ontworpen door Warre Willeme & Jesse UijtdeHaag
        </div>

        <!-- Loading JavaScript at the end of the page for better preformance-->
        <script src="../js/navbar.js"></script>
        <script src="../js/settings_panel.js"></script>
        <script src="../js/search_ticket.js"></script>
        <script src="../js/ticket_modal.js"></script>
    </form>
</body>
</html>
