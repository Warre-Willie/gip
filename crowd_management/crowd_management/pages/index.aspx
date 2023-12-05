<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="crowd_management.pages.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="https://kit.fontawesome.com/08c8f3812a.js" crossorigin="anonymous"></script>

    <!-- CSS -->
    <link rel="stylesheet" href="../css/bulma.css">
    <link rel="stylesheet" href="../css/style.css">
    <link rel="stylesheet" href="../css/bulma-tooltip.css">

    <title></title>
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
            <div class="tile is-ancestor tile-padding page-content">
                <div class="tile is-vertical is-8 fit-tile-content">
                    <div class="tile">
                        <div class="tile is-parent">
                            <article class="tile is-child box">
                                <p class="subtitle"><b>Heat map</b></p>
                                <figure class="image">
                                    <div class="has-background-link" style="height: 530px;"></div>
                                </figure>
                            </article>
                        </div>
                    </div>
                </div>

                <div id="info-panel" class="tile is-parent">
                    <article class="tile is-child box">
                        <p class="subtitle panel-button-container">
                            <b>{Zone} </b>info
                            <!-- <asp:Button ID="btnZoneSettings" runat="server" Text="<i class='fa-solid fa-gear'></i>" CssClass="button is-warning is-rounded is-small" /> -->
                            <button onserverclick="test" runat="server" id="btnZoneSettings1" class="button is-warning is-rounded is-small panel-1-button" onclick="toggleSettings('settings')">
                                <i class="fa-solid fa-gear"></i>
                            </button>
                        </p>

                        <div class="card">
                            <header class="card-header">
                                <p class="card-header-title">
                                    <span class="icon-text ">
                                        <span class="icon">
                                            <i class="fa-solid fa-traffic-light"></i>
                                        </span>
                                    </span>
                                    <span>Druktebarometer <span class="tag is-success is-light">Huidige kleur</span>
                                    </span>
                                </p>
                            </header>
                            <div class="card-content">
                                <div class="content">
                                    <div class="field">
                                        <label class="label">Manueel wijzigen</label>
                                        <div class="control">
                                            <div class="buttons has-addons">
                                                <button class="button is-success">Groen</button>
                                                <button class="button is-warning">Geel</button>
                                                <button class="button is-danger">Rood</button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <br>
                        <div class="card">
                            <header class="card-header">
                                <p class="card-header-title">
                                    <span class="icon-text ">
                                        <span class="icon">
                                            <i class="fa-solid fa-table"></i>
                                        </span>
                                    </span>
                                    <span>Logboek
                                    </span>
                                    <div class="pt-2 pr-2">
                                        <div class="select is-small">
                                            <select>
                                                <option>5 min</option>
                                                <option>15 min</option>
                                                <option>30 min</option>
                                                <option>1 uur</option>
                                                <option>2 uur</option>
                                                <option>5 uur</option>
                                            </select>
                                        </div>
                                    </div>
                                </p>
                            </header>
                            <div class="card-content">
                                <div class="content table-container logbook">
                                    <table class="table is-fullwidth is-striped">
                                        <thead>
                                            <th>Tijd</th>
                                            <th>Aantal mensen</th>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td>13:00</td>
                                                <td>100</td>
                                            </tr>
                                            <tr>
                                                <td>13:05</td>
                                                <td>122</td>
                                            </tr>
                                            <tr>
                                                <td>13:10</td>
                                                <td>143</td>
                                            </tr>
                                            <tr>
                                                <td>13:15</td>
                                                <td>98</td>
                                            </tr>
                                            <tr>
                                                <td>13:05</td>
                                                <td>122</td>
                                            </tr>
                                            <tr>
                                                <td>13:10</td>
                                                <td>143</td>
                                            </tr>
                                            <tr>
                                                <td>13:15</td>
                                                <td>98</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </article>
                </div>

                <div id="settings-panel" class="tile is-parent hide">
                    <article class="tile is-child box">
                        <p class="subtitle panel-button-container">
                            <b>{Zone} </b>instellingen
                    <button class="button is-rounded is-small panel-2-button" onclick="toggleSettings('info')">
                        <i class="fa-solid fa-arrow-left"></i>
                    </button>
                            <button class="button is-warning is-rounded is-small panel-1-button">
                                <i class="fa-solid fa-floppy-disk"></i>
                            </button>
                        </p>
                        <div class="card">
                            <header class="card-header">
                                <p class="card-header-title">
                                    <span class="icon-text ">
                                        <span class="icon">
                                            <i class="fa-solid fa-traffic-light"></i>
                                        </span>
                                    </span>
                                    <span>Druktebarometer <span class="tag is-success is-light">Huidig</span>
                                    </span>
                                </p>
                            </header>
                            <div class="card-content">
                                <div class="content">
                                    <div class="field">
                                        <label class="label">Drempelwaarde wijzigen</label>
                                        <div class="control">
                                            <div class="columns is-desktop">
                                                <div class="column">
                                                    <div class="field">
                                                        <div class="control">
                                                            <input class="input is-success" type="text" value="10">
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="column">
                                                    <div class="field">
                                                        <div class="control">
                                                            <input class="input is-warning" type="text" value="20">
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="column">
                                                    <div class="field">
                                                        <div class="control">
                                                            <input class="input is-danger" type="text" value="30">
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <br>
                        <div class="card">
                            <header class="card-header">
                                <p class="card-header-title">
                                    <span class="icon-text ">
                                        <span class="icon">
                                            <i class="fa-solid fa-font"></i>
                                        </span>
                                    </span>
                                    <span>Zone naam
                                    </span>
                                </p>
                            </header>
                            <div class="card-content">
                                <div class="content">
                                    <input class="input is-link" type="text" placeholder="Bv. Mainstage">
                                </div>
                            </div>
                        </div>
                        <br>
                        <div class="card">
                            <header class="card-header">
                                <p class="card-header-title">
                                    <span class="icon-text ">
                                        <span class="icon">
                                            <i class="fa-regular fa-trash-can"></i>
                                        </span>
                                    </span>
                                    <span>Reset aantal mensen
                                    </span>
                                </p>
                            </header>
                            <div class="card-content">
                                <div class="content">
                                    <button class="button is-danger">Reset</button>
                                </div>
                            </div>
                        </div>
                    </article>
                </div>
            </div>
        </div>

        <div class="footer has-background-white ">
            Ontworpen door Warre Willeme & Jesse UijtdeHaag
        </div>

        <!-- Loading JavaScript at the end of the page for better preformance-->
        <script src="js/navbar.js"></script>
        <script src="js/settings_panel.js"></script>
        <script src="js/search_ticket.js"></script>
        <script src="js/ticket_modal.js"></script>
    </form>
</body>
</html>
