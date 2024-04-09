<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="rapport.aspx.cs" Inherits="crowd_management.pages.Rapport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Rapport maken</title>

    <link rel="shortcut icon" href="../image/favicon.png" type="image/x-icon" />
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
                <a class="navbar-item navbar-brand-container" href="index.aspx">
                    <img class="navbar-brand-img" src="../image/logo_navbar.png" />
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

                    <a href="ticket_beheer.aspx" class="navbar-item">Ticketbeheer
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

        <div class="tile is-ancestor tile-padding">
            <div class="tile is-4 is-vertical is-parent fit-tile-content">
                <div class="tile is-child box">
                    <p class="subtitle"><b>Rapport instellingen</b></p>
                    <div class="filter-table">
                        <table class="table is-fullwidth">
                            <tbody>
                                <tr>
                                    <td>
                                        <label class="checkbox">
                                            <asp:CheckBox ID="cbRapport01" runat="server" />
                                            <span class="has-tooltip-multiline" data-tooltip="Een lijngrafiek met het aantal mensen op evenement gedurende een bepaalde periode">Verspreiding tussen tijden</span>
                                        </label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label class="checkbox">
                                            <asp:CheckBox ID="cbRapport02" runat="server" />
                                            <span class="has-tooltip-multiline" data-tooltip="Een lijngrafiek met het aantal mensen op evenement gedurende een bepaalde periode">Verspreiding tussen zones </span>
                                        </label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label class="checkbox">
                                            <asp:CheckBox ID="cbRapport03" runat="server" />
                                            Aantal tickets
                                        </label>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <asp:Button ID="btnGenRapport" runat="server" Text="Genereer PDF" class="button is-success is-fullwidth" OnClick="btnGenRapport_Click" />
                </div>
                <div class="tile is-child box">
                    <nav class="panel" style="height: 400px; overflow: auto;">
                        <p class="panel-heading">Oude rapporten</p>
                        <div class="panel-block">
                            <p class="control has-icons-left">
                                <input class="input" type="text" placeholder="Search" />
                                <span class="icon is-left">
                                    <i class="fas fa-search" aria-hidden="true"></i>
                                </span>
                            </p>
                        </div>
                        <div id="divPdfList" runat="server">
                        </div>
                        <div class="panel-block">
                            <button class="button is-link is-outlined is-fullwidth">
                                Reset all filters
                            </button>
                        </div>
                    </nav>
                </div>
            </div>
            <div class="tile is-vertical is-parent fit-tile-content">
                <div id="pdfContainer" runat="server" class="tile is-child box">
                    <p class="subtitle"><b>Voorstelling</b></p>
                    <div class="is-hidden-touch">
                        <object
                            data="../eventHandlers/report.ashx?filename=print_rapport.pdf"
                            type="application/pdf"
                            width="100%"
                            height="700px">
                            <p>
                                Unable to display PDF file.
                <a href="../eventHandlers/report.ashx?filename=print_rapport.pdf">Download</a> instead.
                            </p>
                        </object>
                    </div>
                    <div class="is-hidden-desktop">
                        <p>
                            Open a PDF file
              <a href="../eventHandlers/report.ashx?filename=print_rapport.pdf" target="_blank">example</a>.
                        </p>
                    </div>
                </div>
            </div>
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
