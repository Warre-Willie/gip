using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using crowd_management.classes;

namespace crowd_management.pages
{
    public partial class logbook : System.Web.UI.Page
    {
			protected void Page_Load(object sender, EventArgs e)
			{
			}

			private void SetLogbook()
        {
            DbRepository dbRepository = new DbRepository();

            string query = "SELECT * FROM website_logbook ORDER BY timestamp DESC";
            DataTable ticketList = dbRepository.SQLExecuteReader(query);

            foreach(DataRow row in ticketList.Rows)
            {
                string html = $"<tr><td>{row["timestamp"]}</td><td>{row["category"]}</td><td>{row["user"]}</td><td>{row["description"]}</td></tr>";

                divLogbookList.InnerHtml += html;
            }
        }
    }
}