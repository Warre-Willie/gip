using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace crowd_management.pages
{
    public partial class ticket_beheer : System.Web.UI.Page
    {
        const string constring = "SERVER=localhost;DATABASE=crowd_management;UID=root;PASSWORD=gip-WJ;";
        MySqlConnection conn = new MySqlConnection(constring);

        protected void test(object sender, EventArgs e)
        {
            LinkButton linkButton = (LinkButton)sender;
            Debug.WriteLine(linkButton.ID);
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Load the barcode search
                string query = $"SELECT * FROM tickets;";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                int ticketCounter = 0, RFIDCounter = 0;
                while (reader.Read())
                {
                    ticketCounter++;

                    if (!string.IsNullOrEmpty(reader["RFID"].ToString()))
                    {
                        RFIDCounter++;
                    }
                
                    LinkButton ticket = new LinkButton();
                    ticket.Text = "<span class = 'panel-icon'> <i class='fa-solid fa-ticket'></i></span>" + reader["barcode"].ToString();
                    ticket.ID = reader["barcode"].ToString();
                    ticket.CssClass = "panel-block is-active";

                    string[] badgerights = { "camping", "VIP", "backstage", "artiest" };

                    foreach (string right in badgerights)
                    {
                        if (Convert.ToBoolean(reader[right]))
                        {
                            ticket.Attributes["data-badgerights"] += right + " ";
                        }
                    }

                    ticket.Click += test;
                    divTicketList.Controls.Add(ticket);
                }
                conn.Close();

                progress.Attributes["value"] = RFIDCounter.ToString();
                progress.Attributes["max"] = ticketCounter.ToString();

                progressValue.InnerHtml = $" {RFIDCounter} / {ticketCounter}";
            }
            catch (Exception ex)
            {
                //Message: no respond
            }

            // Load the progressbar
            try
            {
                string query = "";
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                
            }
            conn.Close();
            }
            catch(Exception ex1)
            {
            }
        }
    }
}