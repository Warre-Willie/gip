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
</head>
<body onresize = "checkCoordsChange()">
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
                                <div class="imageMapContainer">
                                    <!-- Visible imageMap with id for adjusting the heatmap text -->
                                    <img src="../image/foor_plan.png" usemap="#heatMap" alt="Floor Plan" />
                                    <div id="divHeatMapZone1" class="heatMapZone">Zone1</div>
                                    <div id="divHeatMapZone2" class="heatMapZone">Zone2</div>
                                    <map name="heatMap" id="heatMap">
                                        <area id="HeatMapZone1" shape="rect" coords="16,13,510,260" onclick="__doPostBack('imgHeatMap','0')"/>
                                        <area id="HeatMapZone2" shape="rect" coords="548,13,1157,260" onclick="__doPostBack('imgHeatMap','1')"/>
                                    </map>

                                    <!-- Hidden imageMap for triggering the backend -->
                                    <asp:imageMap ID="imgHeatMap" imageurl="~/image/foor_plan.png" runat="server" hotspotmode="PostBack" OnClick="imgHeatMap_Click" class="hide">
                                        <asp:rectanglehotspot
                                            postbackvalue="1">
                                        </asp:rectanglehotspot>
                                        <asp:rectanglehotspot
                                            postbackvalue="2">
                                        </asp:rectanglehotspot>
                                    </asp:imageMap>
                                    
                                </div>
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
                        <div ID="divInfoZoneCardsCount" runat="server">
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
                                                    <asp:Button ID="btnBarManGreen" runat="server" Text="Groen" class="button is-success is-static" OnClick="barManChange_Click" data-color="green"/>
                                                    <asp:Button ID="btnBarManOrange" runat="server" Text="Geel" class="button is-warning is-static" OnClick="barManChange_Click" data-color="orange"/>
                                                    <asp:Button ID="btnBarManRed" runat="server" Text="Rood" class="button is-danger is-static" OnClick="barManChange_Click" data-color="red"/>
                                                </div>
                                            </div>
                                            <label class="checkbox">
                                                <asp:CheckBox ID="chBarLock" runat="server" AutoPostBack="True" OnCheckedChanged="chBarLock_CheckedChanged" />
                                                Barometer slot
                                            </label>
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
                                                <i class="fa-solid fa-table"></i>
                                            </span>
                                        </span>
                                        <span>Logboek
                                        </span>
                                        <div class="pt-2 pr-2">
                                            <div class="select is-small">
                                                <asp:DropDownList ID="dbZoneLogbookFilter" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dbZoneLogbookFilter_SelectedIndexChanged">
                                                    <asp:ListItem Selected="True" Value="5">5 min</asp:ListItem>
                                                    <asp:ListItem Value="15">15 min</asp:ListItem>
                                                    <asp:ListItem Value="30">30 min</asp:ListItem>
                                                    <asp:ListItem Value="60">1 uur</asp:ListItem>
                                                    <asp:ListItem Value="120">2 uur</asp:ListItem>
                                                    <asp:ListItem Value="180">3 uur</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </p>
                                </header>
                                <div id="dbLogbookFilter" class="card-content">
                                    <div class="content table-container logbook">
                                        <table class="table is-fullwidth is-striped">
                                            <thead>
                                                <th>Tijd</th>
                                                <th>Aantal mensen</th>
                                            </thead>
                                            <tbody ID="tbodyLogbook" runat="server">
                                                <tr>
                                                    <td colspan="2" class="has-text-centered">Geen zone geselecteerd</td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div ID="divInfoZoneCardsAccess" runat="server" visible="False">
                            <div class="card">
                                <header class="card-header">
                                    <p class="card-header-title">
                                        <span class="icon-text ">
                                            <span class="icon">
                                                <i class="fa-solid fa-arrow-down-up-lock"></i>
                                            </span>
                                        </span>
                                        <span>Blokeer toegang</span>
                                    </p>
                                </header>
                                <div class="card-content">
                                    <div class="content">
                                        <label class="checkbox">
                                            <asp:CheckBox ID="chAccessLock" runat="server" AutoPostBack="True" OnCheckedChanged="chAccessLock_CheckedChanged"/>
                                            Ingang afsluiten
                                        </label>
                                    </div>
                                </div>
                            </div>
                            <br />
                            <div class="card">
                                <header class="card-header">
                                    <p class="card-header-title">
                                        <span class="icon-text ">
                                            <span class="icon">
                                                <i class="fa-regular fa-id-badge"></i>
                                            </span>
                                        </span>
                                        <span>Badgerechten</span>
                                    </p>
                                </header>
                                <div class="card-content">
                                    <label class="label">Rechten met toegang:</label>
                                    <table class="table is-fullwidth">
                                        <tbody ID="tbodyBadgeRights" runat="server">
                                            <tr>
                                                <td>
                                                    Kamping
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    VIP
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Backstage
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Artiest
                                                </td>
                                            </tr>
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
                        <br />
                        <div ID="divSettingsZoneCardsCount" runat="server">
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
                        </div>
                        <div ID="divSettingsZoneCardsAccess" runat="server" visible="False">
                            <div class="card">
                                <header class="card-header">
                                    <p class="card-header-title">
                                        <span class="icon-text ">
                                            <span class="icon">
                                                <i class="fa-regular fa-id-badge"></i>
                                            </span>
                                        </span>
                                        <span>Badgerechten</span>
                                    </p>
                                </header>
                                <div class="card-content">
                                    <table class="table is-fullwidth">
                                        <tbody>
                                            <tr>
                                                <td>
                                                    <label class="checkbox">
                                                        <asp:CheckBox ID="CheckBox1" runat="server" Checked="true"/>
                                                        Kamping
                                                    </label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <label class="checkbox">
                                                        <asp:CheckBox ID="CheckBox2" runat="server"/>
                                                        VIP
                                                    </label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <label class="checkbox">
                                                        <asp:CheckBox ID="CheckBox3" runat="server"/>
                                                        Backstage
                                                    </label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <label class="checkbox">
                                                        <asp:CheckBox ID="CheckBox4" runat="server"/>
                                                        Artiest
                                                    </label>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
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
