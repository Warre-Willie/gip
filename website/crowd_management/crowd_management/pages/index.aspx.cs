/*
* File: index.aspx.cs
* Author: Warre Willeme & Jesse UijtdeHaag
* Date: 12-05-2024
* Description: This file contains the code behind for the index page. This page is used to display the heat map and zone information.
*/

using crowd_management.classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace crowd_management.pages;

public partial class Index : Page
{
	#region Accessors and constants

	private readonly DbRepository _dbRepository = new DbRepository();
	private readonly MqttRepository _mqttRepository = new MqttRepository();
	private readonly LogbookHandler _logbookHandler = new LogbookHandler();
	private readonly LoginHandler _loginHandler = new LoginHandler();

	private const string AccessZoneType = "access";
	private const string CountZoneType = "count";

	#endregion

	#region Load and unload page

	protected void Page_Load(object sender, EventArgs e)
	{
		if (Session["User"] == null)
		{
			divPage.Visible = false;
			divLogin.Visible = true;
		}
		else
		{
			divPage.Visible = true;
			divLogin.Visible = false;
			LoadHeatMap();
			LoadZonePanel();
		}
	}

	protected void Page_PreRender(object sender, EventArgs e)
	{
		if (Session["User"] == null)
		{
			divPage.Visible = false;
			divLogin.Visible = true;
		}
		else
		{
			divPage.Visible = true;
			divLogin.Visible = false;
			LoadHeatMap();
			LoadZonePanel();
		}
	}

	protected override void OnUnload(EventArgs e)
	{
		base.OnUnload(e);

		// Close all open connections
		_dbRepository.Dispose();
		_mqttRepository.Dispose();

		divPage.Visible = false;
		divLogin.Visible = true;
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (IsPostBack)
		{
			SetBadgeRights();
		}
	}

	#endregion

	#region Methods

	[WebMethod]
	public static string GetHeatMapData()
	{
		Index indexPage = new Index();
		return indexPage.FetchHeatMapData();
	}

	public class ZoneData
	{
		public string Name { get; set; }
		public double Percentage { get; set; }
		public bool Lockdown { get; set; }
		public string Color { get; set; }
	}

	private string FetchHeatMapData()
	{
		string query = "SELECT * FROM zones";
		DataTable zones = _dbRepository.SqlExecuteReader(query);

		_dbRepository.Dispose();

		var heatMapData = new Dictionary<string, ZoneData>
			{
				{ "1", new ZoneData() },
				{ "2", new ZoneData() },
				{ "3", new ZoneData() },
				{ "4", new ZoneData() }
			};

		foreach (DataRow row in zones.Rows)
		{
			int zoneId = Convert.ToInt32(row["id"]);
			double percentage = -1;
			bool lockdown = Convert.ToBoolean(row["lockdown"]);

			// Formatting zone data
			if (!string.IsNullOrEmpty(row["people_count"].ToString()) && !string.IsNullOrEmpty(row["max_people"].ToString()))
			{
				percentage = Convert.ToDouble(row["people_count"]) / Convert.ToDouble(row["max_people"]) * 100;
				percentage = Math.Round(percentage, 2);
			}

			string color = GetColorClass(row["barometer_color"].ToString());

			heatMapData[zoneId.ToString()] = new ZoneData { Name = row["name"].ToString(), Color = color, Percentage = percentage, Lockdown = lockdown };
		}

		return JsonConvert.SerializeObject(heatMapData);
	}

	private void LoadHeatMap()
	{
		string query = "SELECT * FROM zones";
		DataTable zones = _dbRepository.SqlExecuteReader(query);

		foreach (DataRow row in zones.Rows)
		{
			if (FindControl($"tagZoneName{row["id"]}") is Label zoneName)
			{
				zoneName.Text = row["name"].ToString();
			}

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
		zoneTagColor?.Attributes.Add("class", $"tag is-{GetColorClass(row["barometer_color"].ToString())} is-light is-medium");

		Label zonePercentage = FindControl($"tagZonePercentage{row["id"]}") as Label;
		double percentage = Convert.ToDouble(row["people_count"].ToString()) / Convert.ToDouble(row["max_people"].ToString()) * 100;
		percentage = Math.Round(percentage, 2);
		if (zonePercentage == null)
		{
			return;
		}

		zonePercentage.Text = percentage + "%";
		if (percentage > 100)
		{
			zonePercentage.Text = "100%";
		}
	}

	private void SetHeatMapLockdown(DataRow row)
	{
		HtmlGenericControl zoneLockdown = FindControl($"zoneLockdown{row["id"]}") as HtmlGenericControl;
		if (zoneLockdown == null)
		{
			return;
		}

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
		DataTable zoneInfo = _dbRepository.SqlExecuteReader(query);

		if (zoneInfo.Rows.Count > 0)
		{
			DataRow row = zoneInfo.Rows[0];

			string zoneType = !string.IsNullOrEmpty(row["people_count"].ToString()) ? CountZoneType : AccessZoneType;
			Session["zoneType"] = zoneType;

			SetZoneTitle(row["name"].ToString());

			if (zoneType == CountZoneType)
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
		DataTable logbook = _dbRepository.SqlExecuteReader(query);

		if (logbook.Rows.Count == 0)
		{
			string html = "<tr><td>--/-- --:--</td><td>Nog geen data beschikbaar</td></tr>";
			tbodyLogbook.InnerHtml = html;
			return;
		}

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
		bool isCountType = zoneType == CountZoneType;
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
		if ((string) Session["zoneType"] == AccessZoneType && Session["zoneID"] != null)
		{
			string query = $"SELECT br.*, CASE WHEN brz.zone_id = {Session["zoneID"]} THEN brz.zone_id ELSE NULL END AS zone_id FROM badge_rights br LEFT JOIN badge_rights_zones brz ON br.id = brz.badge_right_id AND brz.zone_id = {Session["zoneID"]}";
			DataTable badgeRights = _dbRepository.SqlExecuteReader(query);

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
				if (row["zone_id"] != DBNull.Value && row["zone_id"].ToString() == Session["zoneID"].ToString())
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
				label.Text = " " + row["name"];
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
		_dbRepository.SqlExecute(query);
	}

	private string GetBarometerColor(int peopleCount, int maxPeople, double thresholdGreen, double thresholdOrange)
	{
		double peopleCountInPercentage = (double) peopleCount / maxPeople * 100;
		string barometerColor;
		if (peopleCountInPercentage <= thresholdGreen)
		{
			barometerColor = "green";
		}
		else if (peopleCountInPercentage <= thresholdOrange)
		{
			barometerColor = "orange";
		}
		else
		{
			barometerColor = "red";
		}

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

		Session["zoneID"] = e.PostBackValue;
	}

	protected void btnSaveZoneSettings_Click(object sender, EventArgs e)
	{
		string query = string.Empty;
		if (Session["zoneType"].ToString() == CountZoneType)
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
					InsertBarometerLogbook(barometerColor);

					var mqttMessageJson = new { id = Convert.ToInt16(Session["zoneID"]), color = barometerColor };
					_mqttRepository.PublishAsync("gip/teller/barometer", JsonConvert.SerializeObject(mqttMessageJson));

					query = $"UPDATE zones SET name = '{tbZoneName.Text}', people_count = {tbEditPeopleCount.Text}, max_people = {tbMaxPeople.Text}, threshold_green = {tbBarThresGreen.Text}, threshold_orange = {tbBarThresOrange.Text}, barometer_color = '{barometerColor}' WHERE id = {Session["zoneID"]}";
				}
			}
		}
		else if (Session["zoneType"].ToString() == AccessZoneType)
		{
			if (string.IsNullOrEmpty(tbZoneName.Text))
			{
				// Message: empty fields
				return;
			}
			else
			{
				_dbRepository.SqlExecute($"DELETE FROM badge_rights_zones WHERE zone_id = {Session["zoneID"]}");

				DataTable badgeRights = _dbRepository.SqlExecuteReader($"SELECT * FROM badge_rights");

				foreach (DataRow row in badgeRights.Rows)
				{
					if (divBadgeRightsEdit.FindControl($"cbBadgeRightID{row["id"]}") is not CheckBox checkbox)
					{
						return;
					}

					if (checkbox.Checked)
					{
						query = $"INSERT INTO badge_rights_zones (zone_id, badge_right_id) VALUES ({Session["zoneID"]}, {row["id"]})";
						_dbRepository.SqlExecute(query);
					}
				}

				_logbookHandler.AddLogbookEntry("Zone", "Admin", $"Zone {tbZoneName.Text} rechten aangepast.");
				query = $"UPDATE zones SET name = '{tbZoneName.Text}', lockdown = {cbAccessLock.Checked} WHERE id = {Session["zoneID"]}";
			}
		}

		_dbRepository.SqlExecute(query);
		// Change the logbook entry to the correct category and change the user to the current user
		_logbookHandler.AddLogbookEntry("Zone", "Admin", $"Wijziging instellingen zone: {Session["zoneID"]}");
	}

	protected void barManChange_Click(object sender, EventArgs e)
	{
		if (!(sender is Button btn))
		{
			return;
		}

		string barometerColor = btn.Attributes["data-color"];

		cbBarLock.Checked = true;

		string query = $"UPDATE zones SET barometer_color = '{barometerColor}', lockdown = 1 WHERE id = {Session["zoneID"]}";
		_dbRepository.SqlExecute(query);
		var mqttMessageJson = new { id = Convert.ToInt16(Session["zoneID"]), color = barometerColor };
		_mqttRepository.PublishAsync("gip/teller/barometer", JsonConvert.SerializeObject(mqttMessageJson));

		InsertBarometerLogbook(barometerColor);
	}

	protected void cbBarLock_CheckedChanged(object sender, EventArgs e)
	{
		DataTable zoneInfo = _dbRepository.SqlExecuteReader($"SELECT * FROM zones WHERE id = {Session["zoneID"]}");
		DataRow row = zoneInfo.Rows[0];
		string barometerColor = row["barometer_color"].ToString();

		if (!cbBarLock.Checked)
		{
			barometerColor = GetBarometerColor(Convert.ToInt32(row["people_count"]), Convert.ToInt32(row["max_people"]), Convert.ToInt32(row["threshold_green"]), Convert.ToInt32(row["threshold_orange"]));
			InsertBarometerLogbook(barometerColor);
		}

		int lockdown = Convert.ToInt32(cbBarLock.Checked);
		string query = $"UPDATE zones SET lockdown = {lockdown}, barometer_color = '{barometerColor}' WHERE id = {Session["zoneID"]}";
		_dbRepository.SqlExecute(query);

		var mqttMessageJson = new { id = Convert.ToInt16(Session["zoneID"]), color = barometerColor };
		_mqttRepository.PublishAsync("gip/teller/barometer", JsonConvert.SerializeObject(mqttMessageJson));
	}

	protected void cbAccessLock_CheckedChanged(object sender, EventArgs e)
	{
		int lockdown = Convert.ToInt32(cbAccessLock.Checked);
		string query = $"UPDATE zones SET lockdown = {lockdown} WHERE id = {Session["zoneID"]}";
		_dbRepository.SqlExecute(query);
	}

	protected void btnLogout_Click(object sender, EventArgs e)
	{
		_loginHandler.LogoutUser(this);
	}

	protected void btnLogin_Click(object sender, EventArgs e)
	{
		_loginHandler.LoginUser(this);
	}

	#endregion
}