using crowd_management.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace crowd_management.pages
{
    public partial class TicketBeheer : System.Web.UI.Page
    {
			private readonly DbRepository _dbRepository = new DbRepository();

        protected void Page_Load(object sender, EventArgs e)
        {
            SetTicketList();
            if (IsPostBack)
            {
                SetBadgeRights();
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);

            // Close all open connections
            _dbRepository.Dispose();
        }

        private protected void SetTicketList()
        {
            string query = "SELECT * FROM tickets";
            DataTable ticketList = _dbRepository.SqlExecuteReader(query);

            int enteredTickets = 0;

            divTicketList.Controls.Clear();

            foreach (DataRow row in ticketList.Rows)
            {
                LinkButton ticket = new LinkButton();
                ticket.Text = "<span class = 'panel-icon'> <i class='fa-solid fa-ticket'></i></span>" + row["barcode"];
                ticket.ID = row["id"].ToString();
                ticket.CssClass = "panel-block";

                if (row["RFID"].ToString() != "")
                {
                    ticket.CssClass = "panel-block is-active";
                }

                if (Session["ticketID"] != null)
                {
                    if (row["id"].ToString() == Session["ticketID"].ToString())
                    {
                        ticket.CssClass += " panel-selected";
                    }
                }

                ticket.Attributes["data-RFID"] = row["RFID"].ToString();
                ticket.Attributes["data-barcode"] = row["barcode"].ToString();
                ticket.Attributes["data-badgerights"] = "";

                query = $"SELECT br.name, brt.ticket_id FROM badge_rights br JOIN badge_rights_tickets brt ON brt.badge_right_id = br.id WHERE brt.ticket_id = {row["id"]}";
                DataTable badgeRights = _dbRepository.SqlExecuteReader(query);

                foreach (DataRow badgeRight in badgeRights.Rows)
                {
                    ticket.Attributes["data-badgerights"] += badgeRight["name"] + " ";
                }

                // Apply filter directly here based on ddTicketSearch and tbTicketFilter values
                string filter = tbTicketFilter.Text.ToUpper();
                string selectedFilter = ddTicketSearch.SelectedValue;

                Dictionary<string, string> filterAttributes = new Dictionary<string, string>
                {
                    { "barcode", row["barcode"].ToString().ToUpper() },
                    { "badgerights", ticket.Attributes["data-badgerights"].ToUpper() },
                    { "rfid", row["RFID"].ToString().ToUpper() }
                };

                if (filterAttributes.ContainsKey(selectedFilter))
                {
                    string attribute = filterAttributes[selectedFilter];
                    if (!string.IsNullOrEmpty(attribute) && attribute.Contains(filter))
                    {
                        ticket.Style["display"] = "";
                    }
                    else
                    {
                        ticket.Style["display"] = "none";
                    }
                }

                ticket.Click += ticketList_Click;
                divTicketList.Controls.Add(ticket);

                if (row["RFID"].ToString() != "")
                {
                    enteredTickets++;
                }
						}

            progress.Attributes["value"] = enteredTickets.ToString();
            progress.Attributes["max"] = ticketList.Rows.Count.ToString();

            progressValue.InnerHtml = $" {enteredTickets} / {ticketList.Rows.Count}";
        }

        private void SetBadgeRights()
        {
            if (Session["ticketID"] == null)
            {
                return;
            }

            string query = $"SELECT * FROM badge_rights";
            DataTable badgeRights = _dbRepository.SqlExecuteReader(query);

            query = $"SELECT * FROM badge_rights_tickets WHERE ticket_id = {Session["ticketID"]}";
            DataTable ticketBadgeRights = _dbRepository.SqlExecuteReader(query);


            divTicketBadgeRights.Controls.Clear();

            Table tbBadgeRights = new Table();
            tbBadgeRights.ID = "tblBadgeRightsEdit";
            tbBadgeRights.CssClass = "table is-fullwidth";

            foreach (DataRow row in badgeRights.Rows)
            {
                // For edit
                TableRow trEdit = new TableRow();
                TableCell tdEdit = new TableCell();

                CheckBox checkBox = new CheckBox();
                checkBox.ID = "cbBadgeRightID" + row["id"];
                checkBox.AutoPostBack = true;
                checkBox.CheckedChanged += new EventHandler(cbBadgeRights_CheckedChanged);

                foreach (DataRow ticketBadgeRight in ticketBadgeRights.Rows)
                {
                    if (row["id"].ToString() == ticketBadgeRight["badge_right_id"].ToString())
                    {
                        checkBox.Checked = true;
                    }
                }

                tdEdit.Controls.Add(checkBox);

                Label label = new Label();
                label.Text = " " + row["name"];
                tdEdit.Controls.Add(label);

                trEdit.Cells.Add(tdEdit);
                tbBadgeRights.Rows.Add(trEdit);
            }

						divTicketBadgeRights.Controls.Add(tbBadgeRights);
        }

        protected void ticketList_Click(object sender, EventArgs e)
        {
            LinkButton ticket = (LinkButton)sender;
            Session["ticketID"] = ticket.ID;

            // Only execute this if column is disabled
            if (divTicketPanel.Attributes["class"].IndexOf("column-disabled") != -1)
            {
                divTicketPanel.Attributes["class"] = divTicketPanel.Attributes["class"].Replace(" column-disabled", "");
						}

            if (ticket.Attributes["data-RFID"] == "")
            {
                btnConnectTicket.Attributes["class"] += " is-static";
            }
            else
            {
                btnConnectTicket.Attributes["class"] = btnConnectTicket.Attributes["class"].Replace("is-static", "");
            }

            imgBarcode.Attributes["src"] = "https://barcode.orcascan.com/?type=code128&data=" + ticket.Attributes["data-barcode"];

            SetTicketList();
            SetBadgeRights();
        }

        protected void cbBadgeRights_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            string badgeRightId = checkBox.ID.Replace("cbBadgeRightID", "");

            if (checkBox.Checked)
            {
                string query = $"INSERT INTO badge_rights_tickets (ticket_id, badge_right_id) VALUES ({Session["ticketID"]}, {badgeRightId})";
                _dbRepository.SqlExecute(query);
            }
            else
            {
                string query = $"DELETE FROM badge_rights_tickets WHERE ticket_id = {Session["ticketID"]} AND badge_right_id = {badgeRightId}";
                _dbRepository.SqlExecute(query);
            }

						SetTicketList();
        }

        [WebMethod]
        public static void ModalTriggered()
        {
            string query = $"UPDATE tickets SET RFID = NULL WHERE id = {HttpContext.Current.Session["ticketID"]}";
            DbRepository dbRepository = new DbRepository();
            dbRepository.SqlExecute(query);
            dbRepository.Dispose();
        }
    }
}