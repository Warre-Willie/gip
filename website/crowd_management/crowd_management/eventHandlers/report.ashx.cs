using System.IO;
using System.Web;

namespace crowd_management;

public class DisplayPdf : IHttpHandler
{
	public void ProcessRequest(HttpContext context)
	{
		string filename = context.Request.QueryString["filename"];

		if (string.IsNullOrEmpty(filename))
		{
			context.Response.StatusCode = 400; // Bad request
			context.Response.End();
			return;
		}

		string filePath = context.Server.MapPath("~/reports/" + filename); // Assuming PDFs are stored in a "pdfs" directory
		if (!File.Exists(filePath))
		{
			context.Response.StatusCode = 404; // File not found
			context.Response.End();
			return;
		}

		context.Response.ContentType = "application/pdf";
		context.Response.AppendHeader("Content-Disposition", "filename=" + filename);
		context.Response.TransmitFile(filePath);
	}

	public bool IsReusable
	{
		get
		{
			return false;
		}
	}
}