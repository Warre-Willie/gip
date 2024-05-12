/*
 * File: logboek.aspx.cs
 * Author: Warre Willeme & Jesse UijtdeHaag
 * Date: May 12, 2024
 * Description: This file contains the code behind for the logbook page. This page displays the logbook entries in a table.
 */

using System;
using System.Data;
using crowd_management.classes;

namespace crowd_management.pages
{
	public partial class Logbook : System.Web.UI.Page
	{
		LogbookHandler logbookHandler = new LogbookHandler();

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				SetLogbook();
			}
		}

		private void SetLogbook()
		{
			DbRepository dbRepository = new DbRepository();
			try
			{
				string query = "SELECT * FROM website_logbook ORDER BY timestamp DESC";
				DataTable ticketList = dbRepository.SqlExecuteReader(query);

				foreach (DataRow row in ticketList.Rows)
				{
					DateTime timestamp = DateTime.Parse(row["timestamp"].ToString());
					string html = $"<tr><td>{FormatDateTime(timestamp)}</td><td>{row["category"]}</td><td>{row["user"]}</td><td>{row["description"]}</td></tr>";

					divLogbookList.InnerHtml += html;
				}
			}
			catch (Exception ex)
			{
                divLogbookList.InnerHtml = "<tr><td colspan='4'>Er is een fout opgetreden bij het ophalen van de logboekgegevens.</td></tr>";
				logbookHandler.AddLogbookEntry( "System", Session["user"].ToString(), ex.ToString());
            }
			
		}

		private static string FormatDateTime(DateTime dateTime)
		{
			return $"{dateTime.Day:d2}/{dateTime.Month:d2} {dateTime.Hour:d2}:{dateTime.Minute:d2}";
		}
	}
}