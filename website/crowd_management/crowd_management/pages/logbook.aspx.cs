/*
* File: logbook.aspx.cs
* Author: Warre Willeme & Jesse UijtdeHaag
* Date: 20-05-2024
* Description: This file contains the code behind for the logbook page. This page is used to display logbook data.
*/

using System;
using System.Data;
using crowd_management.classes;

namespace crowd_management.pages;

public partial class Logbook : System.Web.UI.Page
{
	#region Accessors

	private readonly LoginHandler _loginHandler = new LoginHandler();
    private readonly NotificationHandler _notificationHandler = new NotificationHandler();

    #endregion

    #region Load and unload page

    protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
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
				SetLogbook();
			}
		}
	}

	#endregion

	#region Methods

	private void SetLogbook()
	{
		DbRepository dbRepository = new DbRepository();

        try
        {
            string query = "SELECT * FROM website_logbook ORDER BY timestamp DESC LIMIT 1000";
            DataTable ticketList = dbRepository.SqlExecuteReader(query);

            foreach (DataRow row in ticketList.Rows)
            {
                DateTime timestamp = DateTime.Parse(row["timestamp"].ToString());
                string html = $"<tr><td>{FormatDateTime(timestamp)}</td><td>{row["category"]}</td><td>{row["user"]}</td><td>{row["description"]}</td></tr>";

                divLogbookList.InnerHtml += html;
            }
        }
        catch (Exception)
        {
            _notificationHandler.AddNotification("Fout bij ophalen van logboek.", ENotificationCategories.Warning, this);
        }
    }

	private static string FormatDateTime(DateTime dateTime)
	{
		return $"{dateTime.Day:d2}/{dateTime.Month:d2} {dateTime.Hour:d2}:{dateTime.Minute:d2}";
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