using System;
using System.Data;
using crowd_management.classes;

namespace crowd_management.pages;

public partial class Logbook : System.Web.UI.Page
{
    private readonly LoginHandler _login = new LoginHandler();
    private readonly LogbookHandler _logbookHandler = new LogbookHandler();

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

        string query = "SELECT * FROM website_logbook ORDER BY timestamp DESC LIMIT 100";
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
        divLogin.Visible = true;
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        string tbEmailText = tbEmail.Text.Trim().ToUpper();
        string tbWwText = tbWW.Text;

        string user = _login.LoginUser(tbEmailText, tbWwText);

        if (user != null)
        {
            Session["User"] = user;
            divPage.Visible = true;
            divLogin.Visible = false;
            lbError.Visible = false;
        }
        else
        {
            divPage.Visible = false;
            divLogin.Visible = true;
            lbError.Visible = true;
            _logbookHandler.AddLogbookEntry("Login", "System", $"Failed login attempt by {tbEmailText}");
        }

        tbEmail.Text = "";
        tbWW.Text = "";
    }
}