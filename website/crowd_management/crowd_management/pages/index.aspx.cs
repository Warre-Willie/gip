using crowd_management.classes;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace crowd_management.pages
{
    public partial class index : Page
    {
        private DbRepository dbRepository = new DbRepository();
        private MqttRepository mqttRepository = new MqttRepository();
        private Logbook_handler logbookHandler = new Logbook_handler();

        private const string accessZoneType = "access";
        private const string countZoneType = "count";

        protected void Page_PreRender(object sender, EventArgs e)
        {
            LoadHeatMap();
            if (IsPostBack)
            {
                LoadZonePanel();
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);

            // Close all open connections
            dbRepository.Dispose();
            mqttRepository.Dispose();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (IsPostBack)
            {
                SetBadgeRights();
            }

        }

        private void LoadHeatMap()
        {
            string query = "SELECT * FROM zones";
            DataTable zones = dbRepository.SQLExecuteReader(query);

            foreach (DataRow row in zones.Rows)
            {
                Label zoneName = FindControl($"tagZoneName{row["id"]}") as Label;
                zoneName.Text = row["name"].ToString();

                SetHeatMapLockdown(row);

                if (!string.IsNullOrEmpty(row["people_count"].ToString()))
                {
                    SetHeatMapCountInfo(row);
                }
            }
        }

        private void SetHeatMapCountInfo(DataRow row)
        {
            HtmlGenericControl zoneTagColor = FindControl($"tagZoneColor{row["id"]}") as HtmlGenericControl;
            zoneTagColor.Attributes.Add("class", $"tag is-{GetColorClass(row["barometer_color"].ToString())} is-light is-medium");

            Label zonePrecentage = FindControl($"tagZonePrecentage{row["id"]}") as Label;
            double precentage = Convert.ToDouble(row["people_count"].ToString()) / Convert.ToDouble(row["max_people"].ToString()) * 100;
            precentage = Math.Round(precentage, 2);
            zonePrecentage.Text = precentage + "%";
            if (precentage > 100)
            {
                zonePrecentage.Text = "100%";
            }
        }

        private void SetHeatMapLockdown(DataRow row)
        {
            HtmlGenericControl zoneLockdown = FindControl($"zoneLockdown{row["id"]}") as HtmlGenericControl;
            if (Convert.ToBoolean(row["lockdown"]))
            {
                zoneLockdown.Attributes["class"] = "fa-solid fa-lock";
            }
            else
            {
                zoneLockdown.Attributes["class"] = "fa-solid fa-lock-open";
            }
        }

        private string GetColorClass(string color)
        {
            switch (color)
            {
                case "green":
                    return "success";
                case "orange":
                    return "warning";
                case "red":
                    return "danger";
                default:
                    return "link";
            }
        }

        private void LoadZonePanel()
        {
            if (Session["zoneID"] == null)
            {
                return;
            }

            string query = $"SELECT * FROM zones WHERE id = {Session["zoneID"]}";
            DataTable zoneInfo = dbRepository.SQLExecuteReader(query);

            if (zoneInfo.Rows.Count > 0)
            {
                DataRow row = zoneInfo.Rows[0];

                string zoneType = (!string.IsNullOrEmpty(row["people_count"].ToString())) ? countZoneType : accessZoneType;
                Session["zoneType"] = zoneType;

                SetZoneTitle(row["name"].ToString());

                if (zoneType == countZoneType)
                {
                    SetCountZoneInfo(row);
                    SetStatusColor(row["barometer_color"].ToString());
                    SetLogbook();
                }
                else
                {
                    cbAccessLock.Checked = Convert.ToBoolean(row["lockdown"].ToString());
                    SetBadgeRights();
                }

                SetCardVisibility(zoneType);
            }
            else
            {
                // Message: zone not found
            }
        }

        private void SetLogbook()
        {
            string query = $"SELECT * FROM zone_population_data WHERE zone_id = '{Session["zoneID"]}' ORDER BY timestamp DESC;";
            DataTable logbook = dbRepository.SQLExecuteReader(query);
            tbodyLogbook.InnerHtml = "";

            int interval = Convert.ToInt16(dbZoneLogbookFilter.SelectedValue);
            DateTime prevTime = DateTime.Parse(logbook.Rows[0]["timestamp"].ToString()).AddMinutes(interval);

            int maxRows = 20;
            foreach (DataRow row in logbook.Rows)
            {
                DateTime currentTimestamp = IgnoreSeconds(DateTime.Parse(row["timestamp"].ToString()));
                DateTime newTime = IgnoreSeconds(prevTime.AddMinutes(-interval));

                if (currentTimestamp == newTime)
                {
                    tbodyLogbook.InnerHtml += $"<tr><td>{FormatDateTime(currentTimestamp)}</td><td>{row["people_count"]}</td></tr>";
                    prevTime = currentTimestamp;
                    maxRows--;
                }
                else if (currentTimestamp < newTime)
                {
                    tbodyLogbook.InnerHtml += $"<tr><td>--/-- --:--</td><td>-</td></tr>";
                    tbodyLogbook.InnerHtml += $"<tr><td>{FormatDateTime(currentTimestamp)}</td><td>{row["people_count"]}</td></tr>";
                    prevTime = currentTimestamp;
                    maxRows -= 2;
                }
                if (maxRows <= 0)
                {
                    break;
                }
            }
        }

        private DateTime IgnoreSeconds(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, 0);
        }

        public static string FormatDateTime(DateTime dateTime)
        {
            return $"{dateTime.Day:d2}/{dateTime.Month:d2} {dateTime.Hour:d2}:{dateTime.Minute:d2}";
        }

        private void SetZoneTitle(string zoneName)
        {
            spanZoneName.InnerHtml = $"<b>{zoneName}</b> info";
            spanZoneNameSettings.InnerHtml = $"<b>{zoneName}</b> instellingen";

            tbZoneName.Text = zoneName;
        }

        private void SetCardVisibility(string zoneType)
        {
            bool isCountType = zoneType.ToString() == countZoneType;
            divInfoZoneCardsCount.Visible = isCountType;
            divSettingsZoneCardsCount.Visible = isCountType;
            divInfoZoneCardsAccess.Visible = !isCountType;
            divSettingsZoneCardsAccess.Visible = !isCountType;
        }

        private void SetCountZoneInfo(DataRow row)
        {
            tbBarThresGreen.Text = Convert.ToDouble(row["threshold_green"].ToString()).ToString(System.Globalization.CultureInfo.InvariantCulture);
            tbBarThresOrange.Text = Convert.ToDouble(row["threshold_orange"].ToString()).ToString(System.Globalization.CultureInfo.InvariantCulture);

            tbMaxPeople.Text = row["max_people"].ToString();

            tbEditPeopleCount.Text = row["people_count"].ToString();

            cbBarLock.Checked = Convert.ToBoolean(row["lockdown"].ToString());
        }

        private void SetStatusColor(string barometerColor)
        {
            string colorClass = GetColorClass(barometerColor);

            tagCurrentStatus.Attributes["class"] = $"tag is-{colorClass} is-light";
            tagCurrentStatusSettings.Attributes["class"] = $"tag is-{colorClass} is-light";
        }

        private void SetBadgeRights()
        {
            if (Session["zoneType"] == accessZoneType && Session["zoneID"] != null)
            {
                string query = $"SELECT br.*, brz.zone_id FROM badge_rights br LEFT JOIN badge_rights_zones brz ON brz.badge_right_id = br.id";
                DataTable badgeRights = dbRepository.SQLExecuteReader(query);

                divBadgeRightsEdit.Controls.Clear();
                tableBadgeRightsView.Controls.Clear();

                Table tbBadgeRightsEdit = new Table();
                tbBadgeRightsEdit.ID = "tblBadgeRightsEdit";
                tbBadgeRightsEdit.CssClass = "table is-fullwidth";

                foreach (DataRow row in badgeRights.Rows)
                {
                    // For edit
                    TableRow trEdit = new TableRow();
                    TableCell tdEdit = new TableCell();

                    CheckBox checkBox = new CheckBox();
                    checkBox.ID = "cbBadgeRightID" + row["id"];

                    if (row["zone_id"] != DBNull.Value)
                    {
                        // Check edit checkbox
                        checkBox.Checked = row["zone_id"] != DBNull.Value;

                        // For view
                        HtmlTableRow trView = new HtmlTableRow();
                        HtmlTableCell tdView = new HtmlTableCell();
                        tdView.InnerHtml = row["name"].ToString();
                        trView.Cells.Add(tdView);
                        tableBadgeRightsView.Controls.Add(trView);
                    }

                    tdEdit.Controls.Add(checkBox);

                    Label label = new Label();
                    label.Text = " " + row["name"].ToString();
                    tdEdit.Controls.Add(label);

                    trEdit.Cells.Add(tdEdit);
                    tbBadgeRightsEdit.Rows.Add(trEdit);
                }
                divBadgeRightsEdit.Controls.Add(tbBadgeRightsEdit);
            }
        }
        private void InsertBarometerLogbook(string barometerColor)
        {
            string query = $"INSERT INTO barometer_logbook (zone_id, color) VALUES ({Session["zoneID"]}, '{barometerColor}')";
            dbRepository.SQLExecute(query);
        }

        private string GetBarometerColor(int peopleCount, int maxPeople, double thresholdGreen, double thresholdOrange)
        {
            double peopleCountInPrecentage = ((double)peopleCount / maxPeople) * 100;
            string barometerColor = string.Empty;
            if (peopleCountInPrecentage <= thresholdGreen)
            {
                barometerColor = "green";
            }
            else if (peopleCountInPrecentage <= thresholdOrange)
            {
                barometerColor = "orange";
            }
            else
            {
                barometerColor = "red";
            }
            InsertBarometerLogbook(barometerColor);

            return barometerColor;
        }

        protected void imgHeatMap_Click(object sender, ImageMapEventArgs e)
        {
            // Only execute this if column is disabled
            if (divInfoPanel.Attributes["class"].IndexOf("column-disabled") != -1)
            {
                divInfoPanel.Attributes["class"] = divInfoPanel.Attributes["class"].Replace(" column-disabled", "");
                btnZoneSettings.Attributes["class"] = btnZoneSettings.Attributes["class"].Replace("is-static", "");

                btnBarManGreen.Attributes["class"] = btnBarManGreen.Attributes["class"].Replace(" is-static", "");
                btnBarManOrange.Attributes["class"] = btnBarManOrange.Attributes["class"].Replace(" is-static", "");
                btnBarManRed.Attributes["class"] = btnBarManRed.Attributes["class"].Replace(" is-static", "");
            }

            Session["zoneID"] = e.PostBackValue.ToString();
        }

        protected void btnSaveZoneSettings_Click(object sender, EventArgs e)
        {
            string query = string.Empty;
            if (Session["zoneType"].ToString() == countZoneType)
            {
                if (string.IsNullOrEmpty(tbZoneName.Text) ||
                    string.IsNullOrEmpty(tbMaxPeople.Text) ||
                    string.IsNullOrEmpty(tbBarThresGreen.Text) ||
                    string.IsNullOrEmpty(tbBarThresOrange.Text) ||
                    string.IsNullOrEmpty(tbEditPeopleCount.Text))
                {
                    // Message: empty fields
                    return;
                }
                else
                {
                    if (cbBarLock.Checked)
                    {
                        query = $"UPDATE zones SET name = '{tbZoneName.Text}', people_count = {tbEditPeopleCount.Text}, max_people = {tbMaxPeople.Text}, threshold_green = {tbBarThresGreen.Text}, threshold_orange = {tbBarThresOrange.Text} WHERE id = {Session["zoneID"]}";
                    }
                    else
                    {
                        string barometerColor = GetBarometerColor(Convert.ToInt32(tbEditPeopleCount.Text), Convert.ToInt32(tbMaxPeople.Text), Convert.ToDouble(tbBarThresGreen.Text), Convert.ToDouble(tbBarThresOrange.Text));
                        query = $"UPDATE zones SET name = '{tbZoneName.Text}', people_count = {tbEditPeopleCount.Text}, max_people = {tbMaxPeople.Text}, threshold_green = {tbBarThresGreen.Text}, threshold_orange = {tbBarThresOrange.Text}, barometer_color = '{barometerColor}' WHERE id = {Session["zoneID"]}";
                    }
                }
            }
            else if (Session["zoneType"].ToString() == accessZoneType)
            {
                if (string.IsNullOrEmpty(tbZoneName.Text))
                {
                    // Message: empty fields
                    return;
                }
                else
                {
                    dbRepository.SQLExecute($"DELETE FROM badge_rights_zones WHERE zone_id = {Session["zoneID"]}");

                    DataTable badgeRights = dbRepository.SQLExecuteReader($"SELECT * FROM badge_rights");

                    foreach (DataRow row in badgeRights.Rows)
                    {
                        CheckBox checkbox = (CheckBox)divBadgeRightsEdit.FindControl($"cbBadgeRightID{row["id"]}");
                        if (checkbox.Checked)
                        {
                            query = $"INSERT INTO badge_rights_zones (zone_id, badge_right_id) VALUES ({Session["zoneID"]}, {row["id"]})";
                            dbRepository.SQLExecute(query);
                        }
                    }
                    logbookHandler.AddLogbookEntry("Zone", "Admin", $"Zone {tbZoneName.Text} rechten aangepast.");
                    query = $"UPDATE zones SET name = '{tbZoneName.Text}', lockdown = {cbAccessLock.Checked} WHERE id = {Session["zoneID"]}";
                }
            }
            dbRepository.SQLExecute(query);
            // Change the logbook entry to the correct category and change the user to the current user
            logbookHandler.AddLogbookEntry("Zone", "Admin", $"Wijziging instellingen zone: {Session["zoneID"]}");
        }

        protected void barManChange_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string barometerColor = btn.Attributes["data-color"];

            cbBarLock.Checked = true;

            string query = $"UPDATE zones SET barometer_color = '{barometerColor}', lockdown = 1 WHERE id = {Session["zoneID"]}";
            dbRepository.SQLExecute(query);
            var mqttMessageJson = new { id = Session["zoneID"], color = barometerColor };
            mqttRepository.PublishAsync("gip/teller/barometer", JsonConvert.SerializeObject(mqttMessageJson));

            InsertBarometerLogbook(barometerColor);
            // Change the logbook entry to the correct category and change the user to the current user
            logbookHandler.AddLogbookEntry("Barometer", "Admin", $"Barometer kleur zone:{Session["zoneID"]} ingesteld op {barometerColor}.");
        }

        protected void cbBarLock_CheckedChanged(object sender, EventArgs e)
        {
            DataTable zoneInfo = dbRepository.SQLExecuteReader($"SELECT * FROM zones WHERE id = {Session["zoneID"]}");
            DataRow row = zoneInfo.Rows[0];
            string barometerColor = row["barometer_color"].ToString();

            if (!cbBarLock.Checked)
            {
                barometerColor = GetBarometerColor(Convert.ToInt32(row["people_count"]), Convert.ToInt32(row["max_people"]), Convert.ToInt32(row["threshold_green"]), Convert.ToInt32(row["threshold_orange"]));
            }

            int lockdown = Convert.ToInt32(cbBarLock.Checked);
            string query = $"UPDATE zones SET lockdown = {lockdown}, barometer_color = '{barometerColor}' WHERE id = {Session["zoneID"]}";
            dbRepository.SQLExecute(query);

            var mqttMessageJson = new { id = Session["zoneID"], color = barometerColor };
            mqttRepository.PublishAsync("gip/teller/barometer", JsonConvert.SerializeObject(mqttMessageJson));
            // Change the logbook entry to the correct category and change the user to the current user
            logbookHandler.AddLogbookEntry("Barometer", "Admin", $"Barometer zone:{Session["zoneID"]} {(Convert.ToBoolean(lockdown) ? "vergrendeld" : "ontgrendeld")}.");
        }

        protected void cbAccessLock_CheckedChanged(object sender, EventArgs e)
        {
            int lockdown = Convert.ToInt32(cbAccessLock.Checked);
            string query = $"UPDATE zones SET lockdown = {lockdown} WHERE id = {Session["zoneID"]}";
            dbRepository.SQLExecute(query);
            // Change the logbook entry to the correct category and change the user to the current user
            logbookHandler.AddLogbookEntry("Zone", "Admin", $"Zone {tbZoneName.Text} {(Convert.ToBoolean(lockdown) ? "vergrendeld" : "ontgrendeld")}");
        }
    }
}