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

        void loadZoneInfo()
        {
            try
            {
                conn.Open();
                string query = $"SELECT * FROM zones WHERE id='{Session["activeZoneID"]}';";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    spanZoneName.InnerHtml = "<b>" + reader["name"].ToString() + "</b> info";
                    spanZoneNameSettings.InnerHtml = "<b>" + reader["name"].ToString() + "</b> instellingen";

                    tbBarThresGreen.Text = reader["threshold_green"].ToString();
                    tbBarThresOrange.Text = reader["threshold_orange"].ToString();
                    tbBarThresRed.Text = reader["threshold_red"].ToString();

                    tbZoneName.Text = reader["name"].ToString();
                    tbEditPeopleCount.Text = reader["people_count"].ToString();

                    chBarLock.Checked = Convert.ToBoolean(reader["barometer_lock"]);

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

            divInfoPanel.Attributes["class"] = divInfoPanel.Attributes["class"].Replace(" column-disabled", "");
            btnZoneSettings.Attributes["class"] = btnZoneSettings.Attributes["class"].Replace("is-static", "");

            btnBarManGreen.Attributes["class"] = btnBarManGreen.Attributes["class"].Replace(" is-static", "");
            btnBarManOrange.Attributes["class"] = btnBarManOrange.Attributes["class"].Replace(" is-static", "");
            btnBarManRed.Attributes["class"] = btnBarManRed.Attributes["class"].Replace(" is-static", "");

        }

        protected void imgheatmap_Click(object sender, ImageMapEventArgs e)
        {
            string zoneID = e.PostBackValue.ToString();
            Session["activeZoneID"] = zoneID;
            loadZoneInfo();
        }

        protected void btnSaveZoneSettings_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbBarThresGreen.Text) && !string.IsNullOrEmpty(tbBarThresOrange.Text) && !string.IsNullOrEmpty(tbBarThresRed.Text) && !string.IsNullOrEmpty(tbZoneName.Text) && !string.IsNullOrEmpty(tbEditPeopleCount.Text))
            {
                try
                {
                    conn.Open();
                    string query = $"UPDATE zones SET name = '{tbZoneName.Text}', threshold_green = '{tbBarThresGreen.Text}', threshold_orange = '{tbBarThresOrange.Text}', threshold_red = '{tbBarThresRed.Text}', people_count='{tbEditPeopleCount.Text}' WHERE id = '{Session["activeZoneID"]}';";
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
            loadZoneInfo() ;
        }

        protected void chBarLock_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                string query = $"UPDATE zones SET barometer_lock = '{Convert.ToInt16(chBarLock.Checked)}' WHERE id = '{Session["activeZoneID"]}';";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                // Handle exceptions (log or display an error message)
                Debug.WriteLine(ex.Message);
            }
            loadZoneInfo();
        }
    }
}