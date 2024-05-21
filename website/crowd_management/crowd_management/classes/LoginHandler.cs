using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace crowd_management.classes;

public class LoginHandler
{
	private readonly DbRepository _dbRepository = new DbRepository();
	private readonly LogbookHandler _logbookHandler = new LogbookHandler();

	public void LoginUser(Page page)
	{
		if (page.FindControl("tbEmail") is not TextBox tbEmail)
		{
			return;
		}

		if (page.FindControl("tbWW") is not TextBox tbWw)
		{
			return;
		}

		Control divPage = page.FindControl("divPage");
		Control divLogin = page.FindControl("divLogin");

		if (page.FindControl("lbError") is not Label lbError)
		{
			return;
		}

		string emailInput = tbEmail.Text.Trim().ToUpper();
		string passwordInput = tbWw.Text;

		string hashedWw = GetHash(passwordInput);

		DataTable dt = _dbRepository.SqlExecuteReader($"SELECT * FROM users WHERE email = '{emailInput}'");

		string user = null;
		if (dt.Rows.Count > 0)
		{
			foreach (DataRow row in dt.Rows)
			{
				if (row["password"].ToString() == hashedWw)
				{
					page.Session["User"] = row["username"].ToString();
					user = row["username"].ToString();
				}
				else
				{
					page.Session["User"] = null;
					user = null;
				}
			}
		}

		if (user != null)
		{
			divPage.Visible = true;
			divLogin.Visible = false;
			lbError.Visible = false;
		}
		else
		{
			divPage.Visible = false;
			divLogin.Visible = true;
			lbError.Visible = true;
			_logbookHandler.AddLogbookEntry("Login", "System", $"Failed login attempt by {emailInput.ToLower()}");
		}

		tbEmail.Text = "";
		tbWw.Text = "";
	}

	public void LogoutUser(Page page)
	{
		page.Session["User"] = null;
		page.Session.Clear();
		page.Session.Abandon();

		Control divPage = page.FindControl("divPage");
		Control divLogin = page.FindControl("divLogin");

		divPage.Visible = false;
		divLogin.Visible = true;
	}

	private string GetHash(string input)
	{
		using (SHA256 sha256 = SHA256.Create())
		{
			byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < bytes.Length; i++)
			{
				builder.Append(bytes[i].ToString("x2"));
			}

			return builder.ToString();
		}
	}
}