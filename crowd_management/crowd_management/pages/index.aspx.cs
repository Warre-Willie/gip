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
        // Database connection should go in another file for all the pages
        const string constring = "SERVER=localhost;DATABASE=crowd_management;UID=root;PASSWORD=gip-WJ;";
        MySqlConnection conn = new MySqlConnection(constring);

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        void loadZonePanel()
        {
            try
            {
                string query = $"SELECT * FROM zones WHERE id='{Session["activeZoneID"]}';";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                // Check firt if zone exist if not show messages

                while (reader.Read())
                {
                    if (!string.IsNullOrEmpty(reader["people_count"].ToString()))
                    {
                        Session["ActiveZoneType"] = "count";

                        // Show the right card for info and settings panel
                        divInfoZoneCardsCount.Visible = true;
                        divSettingsZoneCardsCount.Visible = true;
                        divInfoZoneCardsAccess.Visible = false;
                        divSettingsZoneCardsAccess.Visible = false;

                        spanZoneName.InnerHtml = "<b>" + reader["name"].ToString() + "</b> info";
                        spanZoneNameSettings.InnerHtml = "<b>" + reader["name"].ToString() + "</b> instellingen";
                        
                        // Fill zone data from db in
                        tbBarThresGreen.Text = reader["threshold_green"].ToString();
                        tbBarThresOrange.Text = reader["threshold_orange"].ToString();
                        tbBarThresRed.Text = reader["threshold_red"].ToString();

                        tbZoneName.Text = reader["name"].ToString();
                        tbEditPeopleCount.Text = reader["people_count"].ToString();

                        chBarLock.Checked = Convert.ToBoolean(reader["barometer_lock"]);

                        string colorClass = "";
                        switch (reader["barometer_color"])
                        {
                            case "green":
                                colorClass = "success";
                                break;
                            case "orange":
                                colorClass = "warning";
                                break;
                            case "red":
                                colorClass = "danger";
                                break;
                        }
                        tagCurrentStatus.Attributes["class"] = $"tag is-{colorClass} is-light";
                        tagCurrentStatusSettings.Attributes["class"] = $"tag is-{colorClass} is-light";

                    }
                    else
                    {
                        Session["ActiveZoneType"] = "access";

                        // Show the right card for info and settings panel
                        divInfoZoneCardsCount.Visible = false;
                        divSettingsZoneCardsCount.Visible = false;
                        divInfoZoneCardsAccess.Visible = true;
                        divSettingsZoneCardsAccess.Visible = true;

                        spanZoneName.InnerHtml = "<b>" + reader["name"].ToString() + "</b> info";
                        spanZoneNameSettings.InnerHtml = "<b>" + reader["name"].ToString() + "</b> instellingen";

                        // Fill zone data from db in
                        tbZoneName.Text = reader["name"].ToString();

                        chAccessLock.Checked = Convert.ToBoolean(reader["access_lock"]);
                    }

                }
                conn.Close();

                // Code for loading logbook but firts check wich zone type is active
            }
            catch (Exception ex)
            {
                // Message: connection with db lost
            }
        }

        protected void imgHeatMap_Click(object sender, ImageMapEventArgs e)
        {
            // Only execute this once to activate the right panel
            if (Session["activeZoneID"] == null)
            {
                divInfoPanel.Attributes["class"] = divInfoPanel.Attributes["class"].Replace(" column-disabled", "");
                btnZoneSettings.Attributes["class"] = btnZoneSettings.Attributes["class"].Replace("is-static", "");

                btnBarManGreen.Attributes["class"] = btnBarManGreen.Attributes["class"].Replace(" is-static", "");
                btnBarManOrange.Attributes["class"] = btnBarManOrange.Attributes["class"].Replace(" is-static", "");
                btnBarManRed.Attributes["class"] = btnBarManRed.Attributes["class"].Replace(" is-static", "");
            }

            Session["activeZoneID"] = e.PostBackValue.ToString();
            loadZonePanel();
        }

        protected void btnSaveZoneSettings_Click(object sender, EventArgs e)
        {
            string query = "";
            if (Session["activeZoneType"].ToString() == "count")
            {
                if (!string.IsNullOrEmpty(tbBarThresGreen.Text) && !string.IsNullOrEmpty(tbBarThresOrange.Text) && !string.IsNullOrEmpty(tbBarThresRed.Text) && !string.IsNullOrEmpty(tbZoneName.Text) && !string.IsNullOrEmpty(tbEditPeopleCount.Text))
                {
                    query = $"UPDATE zones SET name = '{tbZoneName.Text}', threshold_green = '{tbBarThresGreen.Text}', threshold_orange = '{tbBarThresOrange.Text}', threshold_red = '{tbBarThresRed.Text}', people_count='{tbEditPeopleCount.Text}' WHERE id = '{Session["activeZoneID"]}';";
                }
                else
                {
                    // Message: not all fields are filled
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(tbZoneName.Text))
                {
                    query = $"UPDATE zones SET name = '{tbZoneName.Text}' WHERE id = '{Session["activeZoneID"]}';";
                }
                else
                {
                    // Message: not all fields are filled
                }
            }

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                // Message: connection with db lost
            }
            loadZonePanel();
        }

        protected void chBarLock_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                string query = $"UPDATE zones SET barometer_lock = '{Convert.ToInt16(chBarLock.Checked)}' WHERE id = '{Session["activeZoneID"]}';";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                conn.Close();

                // MQTT: barometer update
            }
            catch (Exception ex)
            {
                // Message: connection with db lost
            }
            loadZonePanel();
        }
        
        protected void chAccessLock_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                string query = $"UPDATE zones SET access_lock = '{Convert.ToInt16(chAccessLock.Checked)}' WHERE id = '{Session["activeZoneID"]}';";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                conn.Close();

                // MQTT: access update
            }
            catch (Exception ex)
            {
                // Message: connection with db lost
            }
            loadZonePanel();
        }

        protected void barManChange_Click(object sender, EventArgs e)
        {
            Button btnBarMan = (Button)sender;
            try
            {
                string query = $"UPDATE zones SET barometer_color = '{btnBarMan.Attributes["data-color"]}' WHERE id = '{Session["activeZoneID"]}';";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                conn.Close();

                // MQTT: barometer update
            }
            catch (Exception ex)
            {
                // Message: connection with db lost
            }
            loadZonePanel();
        }
    }
}