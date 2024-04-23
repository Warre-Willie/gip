using System;
using System.Data;
using crowd_management.classes;

namespace crowd_management.pages
{
	public partial class Logbook : System.Web.UI.Page
	{
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

			string query = "SELECT * FROM website_logbook ORDER BY timestamp DESC";
			DataTable ticketList = dbRepository.SQLExecuteReader(query);

			foreach (DataRow row in ticketList.Rows)
			{
				DateTime timestamp = DateTime.Parse(row["timestamp"].ToString());
				string html = $"<tr><td>{FormatDateTime(timestamp)}</td><td>{row["category"]}</td><td>{row["user"]}</td><td>{row["description"]}</td></tr>";

				divLogbookList.InnerHtml += html;
			}
		}

		private static string FormatDateTime(DateTime dateTime)
		{
			return $"{dateTime.Day:d2}/{dateTime.Month:d2} {dateTime.Hour:d2}:{dateTime.Minute:d2}";
		}
	}
}