<!--
   File: ticket_beheer.aspx
   Author: Warre Willeme & Jesse UijtdeHaag
   Date: May 12, 2024
   Description: This file contains the ticket management page. This page is used to manage tickets and connect them to badges.
-->

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ticket_beheer.aspx.cs" Inherits="crowd_management.pages.TicketBeheer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<meta name="viewport" content="width=device-width, initial-scale=1.0">

	<link rel="shortcut icon" href="../image/favicon.png" type="image/x-icon" />
	<title>Ticketbeheer</title>

	<link href="../FontAwesome/css/all.css" rel="stylesheet" />
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
			<div class="page-content">
				<div id="modal-sync" class="modal">
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
							<img id="imgBarcode" runat="server" src="https://barcode.orcascan.com/?type=code128&data=8720017991611" />
						</section>
						<footer class="modal-card-foot">
							<button class="button is-link" onclick="return false;">Gereed</button>
						</footer>
					</div>
				</div>
				<div class="tile is-ancestor tile-padding">
					<div class="tile is-vertical is-8 fit-tile-content">
						<div class="tile is-parent">
							<article class="tile is-child box">
								<p class="subtitle"><b>Ticket teller</b></p>
								<div class="content">
									<progress id="progress" class="progress is-link" runat="server" value="1" max="10"></progress>
									<div id="progressValue" runat="server">
									</div>
								</div>
							</article>
						</div>
						<div class="tile is-parent">
							<article class="tile is-child box">
								<div class="content">
									<nav class="panel">
										<span class="panel-heading ticket-panel-container">Tickets zoeken
                                        <div class="select is-small ticket-dropdown">
																					<asp:DropDownList ID="ddTicketSearch" runat="server" onchange="changeSearchType()">
																						<asp:ListItem Value="barcode">Barcode</asp:ListItem>
																						<asp:ListItem Value="badgerights">Badgerechten</asp:ListItem>
																						<asp:ListItem Value="rfid">RFID</asp:ListItem>
																					</asp:DropDownList>

																				</div>
										</span>
										<div class="panel-block">
											<p class="control has-icons-left">
												<asp:TextBox ID="tbTicketFilter" runat="server" class="input" placeholder="Ticket zoeken" onkeydown="return (event.keyCode!=13);" onkeyup="searchTicket();"></asp:TextBox>
												<span class="icon is-left">
													<i class="fas fa-search" aria-hidden="true"></i>
												</span>
											</p>
										</div>
										<div id="divTicketList" runat="server" class="ticket-list">
										</div>
										<div class="panel-block">
											<asp:Button ID="btnClearFilter" runat="server" class="button is-link is-outlined is-fullwidth" Text="Verwijder filter" OnClientClick="clearInput(); return false;" />
										</div>
									</nav>
								</div>
							</article>
						</div>
					</div>
					<div id="divTicketPanel" runat="server" class="tile is-parent column-disabled">
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
												<asp:Button ID="btnConnectTicket" runat="server" Text="Opnieuw koppelen" class="button is-danger js-modal-trigger is-static" data-target="modal-sync" OnClientClick="return false;" />
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
												<span>Badgerechten</span>
											</p>
										</header>
										<div id="divTicketBadgeRights" runat="server" class="card-content">
											Geen ticket geselecteerd
										</div>
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
			<script src="../js/navbar.js"></script>
			<script src="../js/settings_panel.js"></script>
			<script src="../js/search_ticket.js"></script>
			<script src="../js/ticket_modal.js"></script>

			<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
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
