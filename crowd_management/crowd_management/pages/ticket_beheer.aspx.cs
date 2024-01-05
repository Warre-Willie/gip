using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Load the barcode search
                string query = $"SELECT barcode FROM tickets;";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    divTicketList.InnerHtml += $"<asp:LinkButton ID='LinkButton1' runat='server' class='panel-block is-active' OnClientClick='' data-barcode={reader["barcode"]}><span class='panel-icon'><class='fa-solid fa-ticket'></></span>{reader["barcode"]}</asp:LinkButton>";
                }
                conn.Close();

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

                while(reader.Read())
                {
                    progress.Attributes["value"] = progress.Attributes["value"].Replace("1",reader["value"].ToString());
                    progress.Attributes["max"] = progress.Attributes["max"].Replace("10", reader["max"].ToString());

                    progressValue.InnerHtml = $"{reader["value"]} / {reader["max"]}";
                }
                conn.Close();
            }
            catch
            {
            }
        }
    }
}