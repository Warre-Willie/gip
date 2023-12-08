using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace crowd_management.pages
{
    public partial class index : System.Web.UI.Page
    {
        const string constring = "SERVER=localhost;DATABASE=crowd_management;UID=root;PASSWORD=gip-WJ;";
        MySqlConnection conn = new MySqlConnection(constring);
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void imgheatmap_Click(object sender, ImageMapEventArgs e)
        {
            string zoneID = e.PostBackValue.ToString();

            
            try
            {
                conn.Open();
                string query = $"SELECT * FROM zones WHERE id='{zoneID}';";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lblZoneName.InnerText = reader["name"].ToString();
                    lblZoneNameSettings.InnerText = reader["name"].ToString();

                    tbBarThresGreen.Text = reader["threshold_green"].ToString();
                    tbBarThresOragne.Text = reader["threshold_orange"].ToString();
                    tbBarThresRed.Text = reader["threshold_red"].ToString();

                    tbZoneName.Text = reader["name"].ToString();

                    switch (reader["current_status"])
                    {
                        case "green":
                            tagCurrentStatus.Attributes["class"] = "tag is-success is-light";
                            tagCurrentStatusSettings.Attributes["class"] = "tag is-success is-light";
                            break;
                        case "orange":
                            tagCurrentStatus.Attributes["class"] = "tag is-warning is-light";
                            tagCurrentStatusSettings.Attributes["class"] = "tag is-warning is-light";
                            break;
                        case "red":
                            tagCurrentStatus.Attributes["class"] = "tag is-danger is-light";
                            tagCurrentStatusSettings.Attributes["class"] = "tag is-danger is-light";
                            break;
                    }
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                // Handle exceptions (log or display an error message)
                Console.WriteLine(ex.Message);
            }
        }
    }
}