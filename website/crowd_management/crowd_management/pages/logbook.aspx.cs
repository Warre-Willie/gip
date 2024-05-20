using System;
using System.Data;
using System.Web;
using crowd_management.classes;

namespace crowd_management.pages;

public partial class Logbook : System.Web.UI.Page
{
    private readonly DbRepository _dbRepository = new DbRepository();

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


    private void SetLogbook()
    {
        DbRepository dbRepository = new DbRepository();

        string query = "SELECT * FROM website_logbook ORDER BY timestamp DESC";
        DataTable ticketList = dbRepository.SqlExecuteReader(query);

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

    protected void btnLogout_Click(object sender, EventArgs e)
    {
        Session["User"] = null;
        divPage.Visible = false;
        divLogin.Visible = true; ;
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        string tbEmail_text = tbEmail.Text.Trim().ToUpper();
        string tbWW_text = tbWW.Text.Trim();
        string hashedWW = Hash.GetHash(tbWW_text);

        DataTable dt = _dbRepository.SqlExecuteReader($"SELECT * FROM users WHERE email = '{tbEmail_text}'");

        if (dt.Rows.Count > 0)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (row["password"].ToString() == hashedWW)
                {
                    Session["User"] = row["username"];
                    divPage.Visible = true;
                    divLogin.Visible = false;
                }

                break;
            }

            lbError.Visible = true;
            tbWW.Text = "";
        }
        else
        {
            lbError.Visible = true;
            tbWW.Text = "";
        }
    }
}