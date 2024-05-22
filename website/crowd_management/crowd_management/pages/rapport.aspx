<!--
   File: rapport.aspx
   Author: Warre Willeme & Jesse UijtdeHaag
   Date: May 12, 2024
   Description: This file contains the HTML code for the report page.
-->

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="rapport.aspx.cs" Inherits="crowd_management.pages.Rapport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<meta name="viewport" content="width=device-width, initial-scale=1.0">
	<title>Rapport maken</title>

	<link rel="shortcut icon" href="../image/favicon.png" type="image/x-icon" />
	<link href="../FontAwesome/css/all.css" rel="stylesheet" />
	
	<!-- JS -->
	<script src="../js/SignalR/jquery-1.7.min.js"></script>
	<script src="../js/SignalR/jquery.signalR-2.4.3.min.js"></script>
	<script src="../js/search_report.js"></script>
	<script src="/signalr/hubs"></script>

	<!-- CSS -->
	<link rel="stylesheet" href="../css/bulma.css">
	<link rel="stylesheet" href="../css/style.css">
	<link rel="stylesheet" href="../css/bulma-tooltip.css">
</head>
<body>
	<form id="form1" runat="server">
		<div id="divPage" runat="server">
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
						<a href="logbook.aspx" class="navbar-item">Logboek
						</a>
					</div>

					<div class="navbar-end">
						<div class="navbar-item">
							<asp:LinkButton ID="btnLogout" runat="server" CssClass="button is-warning" OnClick="btnLogout_Click"><b>Uitloggen</b></asp:LinkButton>
						</div>
					</div>
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
												<span class="has-tooltip-multiline" data-tooltip="Lijngrafiek per zone van het aantal bezoekers op een evenement gedurende het evenement.">Aantal bezoekers op het evenement per tijdseenheid</span>
											</label>
										</td>
									</tr>
									<tr>
										<td>
											<label class="checkbox">
												<asp:CheckBox ID="cbRapport02" runat="server" />
												<span class="has-tooltip-multiline" data-tooltip="Een staafdiagram per zone dat het percentage weergeeft van hoe vaak een bepaalde kleur van de barometer is voorgekomen.">Staafdiagram van kleurvoorkomens in procenten</span>
											</label>
										</td>
									</tr>
									<tr>
										<td>
											<label class="checkbox">
												<asp:CheckBox ID="cbRapport03" runat="server" />
												<span class="has-tooltip-multiline" data-tooltip="Tijdlijn van de barometer kleuren per zone.">Barometertijdlijn</span>
											</label>
										</td>
									</tr>
									<tr>
										<td>
											<label class="checkbox">
												<asp:CheckBox ID="cbRapport04" runat="server" />
												<span class="has-tooltip-multiline" data-tooltip="Laat het aantal gescande tickets zien die gescand zijn in verhouding tot de verkochte tickets.">Gescande tickets t.o.v. verkochte tickets</span>
											</label>
										</td>
									</tr>
								</tbody>
							</table>
						</div>
						<asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="false"></asp:Label>
						<asp:Button ID="btnGenRapport" runat="server" Text="Genereer PDF" class="button is-success is-fullwidth" OnClick="btnGenRapport_Click" />
					</div>
					<div class="tile is-child box">
						<nav class="panel">
							<p class="panel-heading">Oude rapporten</p>
							<div class="panel-block">
								<p class="control has-icons-left">
									<asp:TextBox ID="tbReportFilter" runat="server" class="input" placeholder="Rapport zoeken" onkeydown="return (event.keyCode!=13);" onkeyup="searchReport();"></asp:TextBox>
									<span class="icon is-left">
										<i class="fas fa-search" aria-hidden="true"></i>
									</span>
								</p>
							</div>
							<div id="divPdfList" runat="server" class="panel-list">
							</div>
							<div class="panel-block">
								<asp:Button ID="btnClearFilterReport" runat="server" class="button is-link is-outlined is-fullwidth" Text="Verwijder filter" OnClientClick="clearInputReport(); return false;" />
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
		</div>
		<div id="divLogin" runat="server">
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
							<asp:Button ID="btnLogin" runat="server" Text="Login" Font-Bold="True" CssClass="button is-warning" OnClick="btnLogin_Click" />
						</p>
					</div>
					<asp:Label ID="lbError" runat="server" Text="Ongeldige login poging" CssClass="has-text-danger" Visible="false"></asp:Label>
				</div>
			</div>
		</div>
	</form>
</body>
</html>
