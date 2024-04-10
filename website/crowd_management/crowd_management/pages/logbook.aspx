<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="logbook.aspx.cs" Inherits="crowd_management.pages.logbook" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <link rel="shortcut icon" href="../image/favicon.png" type="image/x-icon" />
    <title>Crowd management</title>

    <script src="https://kit.fontawesome.com/08c8f3812a.js" crossorigin="anonymous"></script>

    <!-- CSS -->
    <link rel="stylesheet" href="../css/bulma.css" />
    <link rel="stylesheet" href="../css/style.css" />
    <link rel="stylesheet" href="../css/bulma-tooltip.css" />
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

            <a href="logbook.aspx" class="navbar-item">logboek
            </a>    
        </div>
        </nav>
            <div class="page-content">
                <div class="tile is-ancestor tile-padding page-content">
                    <div class="tile is-vertical">
                        <div class="tile">
                            <div class="tile is-parent">
                                <article class="tile is-child box">
                                    <p class="subtitle"><b>Logboek</b></p>
                                        <table class="table" style="width:100%">
                                            <thead>
                                                <tr>
                                                    <th>Tijd</th>
                                                    <th>Categorie</th>
                                                    <th>Gebruiker</th>
                                                    <th>Beschrijving</th>
                                                </tr>
                                            </thead>
                                            
                                            <tbody>
                                                <div id="divLogbookList" runat="server" class="logbook-list">

                                                </div>
                                        </table>
                                </article>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="footer has-background-white ">
                Ontworpen door Warre Willeme & Jesse UijtdeHaag
            </div>
    </form>
</body>
</html>
