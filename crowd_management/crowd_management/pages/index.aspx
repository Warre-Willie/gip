<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="crowd_management.pages.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>

    <link rel="shortcut icon" href="../image/favicon.png" type="image/x-icon" />
    <title>Crowd management</title>

    <script src="https://kit.fontawesome.com/08c8f3812a.js" crossorigin="anonymous"></script>
    
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.1.1/jquery.min.js"></script>
    <script src="https://res.cloudinary.com/positionrelativ/raw/upload/v1492377595/jquery.rwdImageMaps_lq5sye.js"></script>

    <!-- CSS -->
    <link rel="stylesheet" href="../css/bulma.css"/>
    <link rel="stylesheet" href="../css/style.css"/>
    <link rel="stylesheet" href="../css/bulma-tooltip.css"/>
    <link rel="stylesheet" href="../css/bulma-switch.min.css"/>
</head>
<body>
    <form id="form1" runat="server">
        <nav class="navbar is-link" role="navigation" aria-label="main navigation">
            <div class="navbar-brand">
                <a class="navbar-item navbar-brand-container">
                    <img class="navbar-brand-img" src="../image/logo_navbar.png"/>
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

                <%--<div class="navbar-end">
                    <div class="navbar-item">
                        <div class="buttons">
                            <a class="button is-warning">
                                <strong>Login</strong>
                            </a>
                        </div>
                    </div>
                </div>--%>
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
                                    <asp:imageMap ID="imgheatmap" imageurl="~/image/foor_plan.png" runat="server" hotspotmode="PostBack" OnClick="imgheatmap_Click">
                                        <asp:rectanglehotspot
                                            top="13"
                                            left="16"
                                            bottom="260"
                                            right="510"
                                            postbackvalue="1">
                                        </asp:rectanglehotspot>
                                        <asp:rectanglehotspot
                                            top="13"
                                            left="548"
                                            bottom="260"
                                            right="1157"
                                            postbackvalue="2">
                                        </asp:rectanglehotspot>
                                    </asp:imageMap>
                                </figure>
                            </article>
                        </div>
                    </div>
                </div>

                <div ID="divInfoPanel" runat="server" class="tile is-parent column-disabled">
                    <article class="tile is-child box">
                        <p class="subtitle panel-button-container">
                            <span ID="spanZoneName" runat="server"><b>Geen zone geselecteerd</b></span>
                            <asp:LinkButton ID="btnZoneSettings" runat="server" class="button is-warning is-rounded is-small panel-1-button is-static" OnClientClick="toggleSettings(); return false;">
                                <i class="fa-solid fa-gear"></i>
                            </asp:LinkButton>
                        </p>

                        <div class="card">
                            <header class="card-header">
                                <p class="card-header-title">
                                    <span class="icon-text ">
                                        <span class="icon">
                                            <i class="fa-solid fa-traffic-light"></i>
                                        </span>
                                    </span>
                                    <span>Druktebarometer 
                                        <span ID="tagCurrentStatus" runat="server" class="tag is-light">Huidige kleur</span>
                                    </span>
                                </p>
                            </header>
                            <div class="card-content">
                                <div class="content">
                                    <div class="field">
                                        <label class="label">Manueel wijzigen</label>
                                        <div class="control">
                                            <div class="buttons has-addons">
                                                <asp:Button ID="btnBarManGreen" runat="server" Text="Groen" class="button is-success is-static"/>
                                                <asp:Button ID="btnBarManOrange" runat="server" Text="Geel" class="button is-warning is-static"/>
                                                <asp:Button ID="btnBarManRed" runat="server" Text="Rood" class="button is-danger is-static"/>
                                            </div>
                                        </div>
                                        <div class="field">
                                            <asp:CheckBox ID="chBarLock" runat="server" name="barLock" AutoPostBack="True" OnCheckedChanged="chBarLock_CheckedChanged" />
                                            <label for="barLock">Barometer slot</label>
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
                                                <td colspan="2" class="has-text-centered">Geen zone geselecteerd</td>
                                            </tr>
                                            <%--<tr>
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
                                            </tr>--%>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </article>
                </div>

                <div ID="divSettingsPanel" runat="server" class="tile is-parent hide">
                    <article class="tile is-child box">
                        <p class="subtitle panel-button-container">
                            <span ID="spanZoneNameSettings" runat="server"><b>Geen zone geselecteerd</b></span>                           
                            <asp:LinkButton ID="btnExitZoneSettings" runat="server" class="button is-rounded is-small panel-2-button" OnClientClick="toggleSettings(); return false;">
                                <i class="fa-solid fa-arrow-left"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="btnSaveZoneSettings" runat="server" class="button is-warning is-rounded is-small panel-1-button" OnClick="btnSaveZoneSettings_Click">
                                <i class="fa-solid fa-floppy-disk"></i>
                            </asp:LinkButton>
                        </p>
                        <div class="card">
                            <header class="card-header">
                                <p class="card-header-title">
                                    <span class="icon-text ">
                                        <span class="icon">
                                            <i class="fa-solid fa-traffic-light"></i>
                                        </span>
                                    </span>
                                    <span>Druktebarometer 
                                        <span ID="tagCurrentStatusSettings" runat="server" class="tag is-light">Huidige kleur</span>
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
                                                            <asp:TextBox ID="tbBarThresGreen" runat="server" class="input is-success" onkeydown="return (event.keyCode!=13);"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="column">
                                                    <div class="field">
                                                        <div class="control">
                                                            <asp:TextBox ID="tbBarThresOrange" runat="server" class="input is-warning" onkeydown="return (event.keyCode!=13);"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="column">
                                                    <div class="field">
                                                        <div class="control">
                                                            <asp:TextBox ID="tbBarThresRed" runat="server" class="input is-danger" onkeydown="return (event.keyCode!=13);"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <br />
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
                                    <asp:TextBox ID="tbZoneName" runat="server" class="input is-link" placeholder="Bv. Mainstage" onkeydown="return (event.keyCode!=13);"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <br>
                        <div class="card">
                            <header class="card-header">
                                <p class="card-header-title">
                                    <span class="icon-text ">
                                        <span class="icon">
                                            <i class="fa-solid fa-pen-to-square"></i>
                                        </span>
                                    </span>
                                    <span>Wijwig aantal mensen
                                    </span>
                                </p>
                            </header>
                            <div class="card-content">
                                <div class="content">
                                    <asp:TextBox ID="tbEditPeopleCount" runat="server" class="input is-link" onkeydown="return (event.keyCode!=13);"></asp:TextBox>
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
        <script src="../js/image_map.js"></script>
        <script src="../js/navbar.js"></script>
        <script src="../js/settings_panel.js"></script>
        <script src="../js/search_ticket.js"></script>
        <script src="../js/ticket_modal.js"></script>
    </form>
</body>
</html>
