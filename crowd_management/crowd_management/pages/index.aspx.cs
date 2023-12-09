using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
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
            Session["activeZoneID"] = zoneID;
            
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
                    tbBarThresOrange.Text = reader["threshold_orange"].ToString();
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
                Debug.WriteLine(ex.Message);
            }
        }

        protected void btnSaveZoneSettings_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbBarThresGreen.Text) && !string.IsNullOrEmpty(tbBarThresOrange.Text) && !string.IsNullOrEmpty(tbBarThresRed.Text) && !string.IsNullOrEmpty(tbZoneName.Text))
            {
                try
                {
                    conn.Open();
                    string query = $"UPDATE zones SET name = '{tbZoneName.Text}', threshold_green = '{tbBarThresGreen.Text}', threshold_orange = '{tbBarThresOrange.Text}', threshold_red = '{tbBarThresRed.Text}' WHERE id = '{Session["activeZoneID"]}';";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    // Handle exceptions (log or display an error message)
                    Debug.WriteLine(ex.Message);
                }
            }
            else
            {
                Debug.WriteLine("Empty field");
                divInfoPanel.Attributes["class"] = "tile is-parent hide";
                divSettingsPanel.Attributes["class"] = "tile is-parent";
            }
        }
    }
}