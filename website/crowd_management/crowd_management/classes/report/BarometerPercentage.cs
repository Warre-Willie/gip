using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Script.Serialization;
using QuickChart;

namespace crowd_management.classes.report;

public class BarometerPercentage
{
	private int Id { get; set; }
	private string Zone { get; set; }
	private int PercentageGreen { get; set; }
	private int PercentageOrange { get; set; }
	private int PercentageRed { get; set; }


	private readonly DbRepository _dbRepository = new DbRepository();

	public void MakeGraph(string contentPath)
	{
		string json = MakeJson();

		Chart qc = new Chart();

		qc.Width = 500;
		qc.Height = 300;

		qc.Config = json;

		string graphBase64 = Convert.ToBase64String(qc.ToByteArray());

		string relativePath = "~/reports/barometer_zone_percentage.html";
		string fullPath = relativePath.Replace("~", AppDomain.CurrentDomain.BaseDirectory);
		string graphHtml = File.ReadAllText(fullPath);

		graphHtml = graphHtml.Replace("{{graphBase64}}", graphBase64);
		string templateContent = File.ReadAllText(contentPath);
		templateContent += graphHtml;
		File.WriteAllText(contentPath, templateContent);
	}

	private string MakeJson()
	{
		List<BarometerPercentage> barometerPercentages = GetChartData();
		var labels = new List<string>();
		var datasets = new List<object>();

		foreach (var barometerPercentage in barometerPercentages)
		{
			labels.Add(barometerPercentage.Zone);

			datasets.Add(new
			{
				label = "Groen",
				backgroundColor = "lime",
				data = new[] { barometerPercentage.PercentageGreen }
			});

			datasets.Add(new
			{
				label = "Geel",
				backgroundColor = "yellow",
				data = new[] { barometerPercentage.PercentageOrange }
			});

			datasets.Add(new
			{
				label = "Rood",
				backgroundColor = "red",
				data = new[] { barometerPercentage.PercentageRed }
			});
		}

		var jsonData = new
		{
			type = "bar",
			data = new
			{
				labels,
				datasets
			},
			options = new
			{
				title = new { display = false },
				scales = new
				{
					xAxes = new[] { new { stacked = true, barThickness = 50 } },
					yAxes = new[] { new { stacked = true } }
				}
			}
		};

		JavaScriptSerializer serializer = new JavaScriptSerializer();
		return serializer.Serialize(jsonData);
	}

	private List<BarometerPercentage> GetChartData()
	{
		string query = "SELECT id, people_count, name FROM zones";
		DataTable dtZones = _dbRepository.SqlExecuteReader(query);

		List<BarometerPercentage> barometerPercentages = new List<BarometerPercentage>();
		Dictionary<int, Dictionary<string, int>> secondCount = new Dictionary<int, Dictionary<string, int>>();

		foreach (DataRow row in dtZones.Rows)
		{
			if (row["people_count"] != DBNull.Value)
			{
				BarometerPercentage barometerPercentage = new BarometerPercentage
				{
					Id = (int) row["id"],
					Zone = row["name"].ToString()
				};
				barometerPercentages.Add(barometerPercentage);

				secondCount.Add((int) row["id"], new Dictionary<string, int> { { "green", 0 }, { "orange", 0 }, { "red", 0 } });
			}
		}

		query = "SELECT * FROM barometer_logbook ORDER BY timestamp ASC;";
		DataTable dtLogbook = _dbRepository.SqlExecuteReader(query);

		DateTime prevTime = new DateTime();

		foreach (DataRow row in dtLogbook.Rows)
		{
			if (prevTime == new DateTime())
			{
				prevTime = DateTime.Parse(row["timestamp"].ToString());
				continue;
			}

			DateTime currentTime = DateTime.Parse(row["timestamp"].ToString());

			int seconds = (int)(currentTime - prevTime).TotalSeconds;

			secondCount[(int) row["zone_id"]][row["color"].ToString()] += seconds;

			prevTime = currentTime;
		}

		foreach (KeyValuePair<int, Dictionary<string, int>> kvp in secondCount)
		{
			int totalSecondsZone = kvp.Value["green"] + kvp.Value["orange"] + kvp.Value["red"];

			foreach (BarometerPercentage barometerPercentage in barometerPercentages)
			{
				if (barometerPercentage.Id == kvp.Key)
				{
					barometerPercentage.PercentageGreen = (int) Math.Round((double) kvp.Value["green"] / totalSecondsZone * 100);
					barometerPercentage.PercentageOrange = (int) Math.Round((double) kvp.Value["orange"] / totalSecondsZone * 100);
					barometerPercentage.PercentageRed = (int) Math.Round((double) kvp.Value["red"] / totalSecondsZone * 100);
				}
			}
		}

		return barometerPercentages;
	}
}