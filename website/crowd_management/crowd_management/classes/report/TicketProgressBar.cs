using System;
using System.Data;
using System.IO;
using Newtonsoft.Json;

namespace crowd_management.classes.report;

public class TicketProgressBar
{
	private readonly DbRepository _dbRepository = new DbRepository();

	public void MakeGraph(string contentPath)
	{
		string json = MakeJson();

		string relativePath = "~/reports/ticket_progress_bar.html";
		string fullPath = relativePath.Replace("~", AppDomain.CurrentDomain.BaseDirectory);
		string graphHtml = File.ReadAllText(fullPath);

		graphHtml = graphHtml.Replace("{{graphJson}}", json);

		string templateContent = File.ReadAllText(contentPath);
		templateContent += graphHtml;
		File.WriteAllText(contentPath, templateContent);
	}

	private string MakeJson()
	{
		int ticketPercentage = GetData();

		// Constructing JSON object
		var jsonObject = new
		{
			type = "progressBar",
			data = new
			{
				datasets = new[]
				{
					new
					{
						data = new[] { ticketPercentage }
					}
				}
			}
		};

		return JsonConvert.SerializeObject(jsonObject);
	}

	private int GetData()
	{
		string query = "SELECT (SUM(CASE WHEN RFID IS NOT NULL THEN 1 ELSE 0 END) * 100.0) / COUNT(*) AS percentage_with_RFID FROM tickets;\r\n";
		DataTable dtTickets = _dbRepository.SqlExecuteReader(query);

		int ticketPercentage = 0;
		foreach (DataRow row in dtTickets.Rows)
		{
			ticketPercentage = Convert.ToInt32(row[0]);
		}

		return ticketPercentage;
	}
}