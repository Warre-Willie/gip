using crowd_management.classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace crowd_management.pages
{
    public partial class rapport : System.Web.UI.Page
    {
        private DbRepository dbRepository = new DbRepository();
        private HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pdfContainer.Visible = false;
            }
            setPdfList();
        }
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);

            // Close all open connections
            dbRepository.Dispose();
        }

        protected void btnGenRapport_Click(object sender, EventArgs e)
        {
            // Generate the pdf file with the filter data

            //var fileNames = htmlToPdfConverter.ConvertToPdf(@"C:\Users\warre\Documents\Programmeren\gip\website\print_rapport.html");
            var fileNames = htmlToPdfConverter.ConvertToPdf(Server.MapPath("~/rapports/print_rapport.html"));

            string fileName = fileNames.Item1;
            string friendlyName = fileNames.Item2;

            // make a function that checks if the file is already in the database with the same friendly name if so add a number to the end
            string query = $"SELECT * FROM report_files WHERE friendly_name LIKE '%{friendlyName}%'";
            DataTable dt = dbRepository.SQLExecuteReader(query);
            if (dt.Rows.Count > 0)
            {
                friendlyName = $"{friendlyName} ({dt.Rows.Count})";
            }
            query = $@"INSERT INTO report_files (file_name, friendly_name) VALUES ('{fileName}', '{friendlyName}')";
            dbRepository.SQLExecute(query);

            SetPdfContainer(fileName);
        }

        private protected void setPdfList()
        {
            string query = "SELECT * FROM report_files";
            DataTable pdfList = dbRepository.SQLExecuteReader(query);

            divPdfList.Controls.Clear();

            foreach (DataRow row in pdfList.Rows)
            {
                LinkButton pdf = new LinkButton();

                pdf.Text = $"<span class='panel-icon'><i class='fa-solid fa-file-pdf'></i></span>{row["friendly_name"].ToString()}<br>";
                pdf.ID = row["file_name"].ToString();
                pdf.Click += pdfList_Click;

                divPdfList.Controls.Add(pdf);
            }
        }   
        protected void pdfList_Click(object sender, EventArgs e)
        {
            LinkButton file = (LinkButton)sender;
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
            setPdfList();
        }
    }
}