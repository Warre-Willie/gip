using MySql.Data.MySqlClient;
using System;
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
                    divTicketList.InnerHtml += $"<a class='panel-block is-active'><span class='panel-icon'><i class='fa-solid fa-ticket'></i></span>{reader["barcode"]}</a>";
                }
                query = "";

            }
            catch (Exception ex)
            {
                //Message: no respond
            }


        }
    }
}