using crowd_management.classes;
using System;
using System.Data;
using System.Diagnostics;
using System.Web.UI.WebControls;
using crowd_management.classes.report;
using System.IO;
using System.Web;

namespace crowd_management.pages;

public partial class Rapport : System.Web.UI.Page
{
	private readonly DbRepository _dbRepository = new DbRepository();
	private readonly HtmlToPdfConverter _htmlToPdfConverter = new HtmlToPdfConverter();
	private readonly LogbookHandler _logbookHandler = new LogbookHandler();
	private readonly BarometerPercentage _barometerPercentage = new BarometerPercentage();
	private readonly ZonePopulation _zonePopulation = new ZonePopulation();
	private readonly BarometerTimeline _barometerTimeline = new BarometerTimeline();
	private readonly TicketProgressBar _ticketProgressBar = new TicketProgressBar();
    private readonly LoginHandler _login = new LoginHandler();


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
        }

        if (!IsPostBack)
        {
            pdfContainer.Visible = false;
        }

        SetPdfList();

    }

    protected override void OnUnload(EventArgs e)
    {
        base.OnUnload(e);

        // Close all open connections
        _dbRepository.Dispose();
    }

	protected void btnGenRapport_Click(object sender, EventArgs e)
	{
		if (cbRapport01.Checked || cbRapport02.Checked || cbRapport03.Checked || cbRapport04.Checked)
		{
			// Generate the pdf file with the filter data
			string contentPath = Server.MapPath("~/reports/report_template_content_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".html");
			FileStream fs = File.Create(contentPath);
			fs.Close();

			string filePath = Server.MapPath("~/reports/report_template.html");
			string newTemplate = Path.Combine(Path.GetDirectoryName(filePath) ?? string.Empty, Path.GetFileNameWithoutExtension(filePath) + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(filePath));
			File.Copy(filePath, newTemplate);

			if (cbRapport01.Checked)
			{
				_zonePopulation.MakeGraph(contentPath);
			}

			if (cbRapport02.Checked)
			{
				_barometerPercentage.MakeGraph(contentPath);
			}

			if (cbRapport03.Checked)
			{
				_barometerTimeline.MakeGraph(contentPath);
			}

			if (cbRapport04.Checked)
			{
				_ticketProgressBar.MakeGraph(contentPath);
			}

			cbRapport01.Checked = false;
			cbRapport02.Checked = false;
			cbRapport03.Checked = false;
			cbRapport04.Checked = false;

			File.WriteAllText(newTemplate, File.ReadAllText(newTemplate).Replace("{{HtmlContent}}", File.ReadAllText(contentPath)));

			var fileNames = _htmlToPdfConverter.ConvertToPdf(newTemplate);
			File.Delete(newTemplate);
			File.Delete(contentPath);

			string fileName = fileNames.Item1;
			string friendlyName = fileNames.Item2;

			// make a function that checks if the file is already in the database with the same friendly name if so add a number to the end
			string query = $"SELECT * FROM report_files WHERE friendly_name LIKE '%{friendlyName}%'";
			DataTable dt = _dbRepository.SqlExecuteReader(query);
			if (dt.Rows.Count > 0)
			{
				friendlyName = $"{friendlyName} ({dt.Rows.Count})";
			}

			query = $@"INSERT INTO report_files (file_name, friendly_name) VALUES ('{fileName}', '{friendlyName}')";
			_dbRepository.SqlExecute(query);

			//Change the logbook entry to the correct category and change the user to the current user
			_logbookHandler.AddLogbookEntry("Rapport", "Admin", $"Rapport {fileName} gegenereerd");

			SetPdfContainer(fileName);
			lblError.Visible = false;
		}
		else
		{
			lblError.Text = "Selecteer minstens één optie om te genereren.";
			lblError.Visible = true;
		}
	}

    private void SetPdfList()
    {
        string query = "SELECT * FROM report_files ORDER BY timestamp DESC";
        DataTable pdfList = _dbRepository.SqlExecuteReader(query);

        divPdfList.Controls.Clear();

        foreach (DataRow row in pdfList.Rows)
        {
            LinkButton pdf = new LinkButton();

            pdf.Text = $"<span class='panel-icon'><i class='fa-solid fa-file-pdf'></i></span>{row["friendly_name"]}";
            pdf.ID = row["file_name"].ToString();
            pdf.Click += pdfList_Click;
            pdf.CssClass = "panel-block";

            divPdfList.Controls.Add(pdf);
        }
    }

	protected void btnDeletePdf_Click(object sender, EventArgs e)
	{
		if (sender is not LinkButton file)
		{
			return;
		}

		string query = $"DELETE FROM report_files WHERE file_name = '{file.Attributes["fileName"]}'";
		_dbRepository.SqlExecute(query);

		if (File.Exists(Server.MapPath($"~/rapports/{file.Attributes["fileName"]}")))
		{
			File.Delete(Server.MapPath($"~/rapports/{file.Attributes["fileName"]}"));
		}

		pdfContainer.Visible = false;
		SetPdfList();
	}

    protected void pdfList_Click(object sender, EventArgs e)
    {
        if (sender is not LinkButton file)
        {
            return;
        }

        SetPdfContainer(file.ID);
    }

    private void SetPdfContainer(string filename)
    {
        pdfContainer.Visible = true;
        string pdfUrl = $"../eventHandlers/report.ashx?filename={filename}";

        pdfContainer.InnerHtml = $@"<p class=""subtitle""><b>Voorstelling</b></p>
											<div class=""is-hidden-touch"">
												<object data=""\rapports\{pdfUrl}"" type=""application/pdf"" width=""100%"" height=""700px"">
													<p>Er is een fout opgetreden met het openen van de pdf</p>
												</object>
											</div>
											<div class=""is-hidden-desktop"">
												<p>Open a PDF file
													<a href=""{pdfUrl}"" target=""_blank"">example</a>.
												</p>
											</div>";
        SetPdfList();
    }

    protected void btnLogout_Click(object sender, EventArgs e)
    {
        Session["User"] = null;
        Session.Clear();
        Session.Abandon();

        Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetNoStore();

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