using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using QuickChart;

namespace crowd_management.classes.report;

public class ZonePopulation
{
	private int Id { get; set; }
	private string Label { get; set; }
	private string BackgroundColor { get; set; }
	private string BorderColor { get; set; }
	private List<Dictionary<string, object>> Data { get; set; }

	private readonly DbRepository _dbRepository = new DbRepository();

	public void MakeGraph(string contentPath)
	{
		string json = MakeJson();

		Chart qc = new Chart();

		qc.Width = 500;
		qc.Height = 300;
		qc.Version = "3";

		qc.Config = json;

		string graphBase64 = Convert.ToBase64String(qc.ToByteArray());

		string relativePath = "~/reports/zone_population.html";
		string fullPath = relativePath.Replace("~", AppDomain.CurrentDomain.BaseDirectory);
		string graphHtml = File.ReadAllText(fullPath);

		graphHtml = graphHtml.Replace("{{graphBase64}}", graphBase64);
		string templateContent = File.ReadAllText(contentPath);
		templateContent += graphHtml;
		File.WriteAllText(contentPath, templateContent);
	}

	private string MakeJson()
	{
		List<ZonePopulation> zonePopulations = GetChartData();
		var datasets = new List<string>();

		foreach (var zone in zonePopulations)
		{
			var dataPoints = new List<string>();

			foreach (var dataPoint in zone.Data)
			{
				// Convert timestamp to ISO 8601 format (string) and add single quotes
				string isoTimestamp = ((DateTime) dataPoint["x"]).ToString("yyyy-MM-ddTHH:mm");

				// Construct the data point with 'new Date()' and single quotes around the ISO timestamp
				dataPoints.Add($"{{ x: new Date('{isoTimestamp}'), y: {dataPoint["y"]} }}");
			}

			datasets.Add($"{{ label: '{zone.Label}', backgroundColor: '{zone.BackgroundColor}', borderColor: '{zone.BorderColor}', data: [{string.Join(",", dataPoints)}], fill: false, pointRadius: 0 }}");
		}

		Dictionary<string, DateTime> firstAndLastTimestamps = GetFirstAndLastTimestamps(zonePopulations);

		string minTimestamp = firstAndLastTimestamps["FirstTimestamp"].ToString("yyyy-MM-ddTHH:mm");
		string maxTimestamp = firstAndLastTimestamps["LastTimestamp"].ToString("yyyy-MM-ddTHH:mm");

		string jsonData = "{{ 'type': 'line', 'data': {{ 'datasets': [{0}] }}, 'options': {{ 'title': {{ 'display': false }}, 'scales': {{ 'x': {{ 'type': 'time', 'ticks': {{ 'maxRotation': 45, 'minRotation': 45 }}, 'time': {{ 'unit': 'minute', 'stepSize': 5, 'autoSkip': false, 'displayFormats': {{ 'hour': 'HH:mm', 'minute': 'HH:mm' }} }}, 'min': new Date('{1}'), 'max': new Date('{2}') }} }} }} }}";

		jsonData = string.Format(jsonData, string.Join(",", datasets), minTimestamp, maxTimestamp);

		return jsonData;
	}


	private Dictionary<string, DateTime> GetFirstAndLastTimestamps(List<ZonePopulation> zonePopulations)
	{
		DateTime firstTimestamp = DateTime.MaxValue;
		DateTime lastTimestamp = DateTime.MinValue;

		foreach (var zone in zonePopulations)
		{
			foreach (var dataPoint in zone.Data)
			{
				if (dataPoint["x"] is not DateTime timestamp)
				{
					return null;
				}

				if (timestamp < firstTimestamp)
				{
					firstTimestamp = timestamp;
				}

				if (timestamp > lastTimestamp)
				{
					lastTimestamp = timestamp;
				}
			}
		}

		var timestamps = new Dictionary<string, DateTime>();
		timestamps["FirstTimestamp"] = firstTimestamp;
		timestamps["LastTimestamp"] = lastTimestamp;

		return timestamps;
	}


	private string GenerateRandomColor()
	{
		Random random = new Random();
		return $"rgba({random.Next(0, 255)}, {random.Next(0, 255)}, {random.Next(0, 255)})";
	}

	private List<ZonePopulation> GetChartData()
	{
		string query = "SELECT id, name, people_count FROM zones";
		DataTable dtZones = _dbRepository.SqlExecuteReader(query);

		List<ZonePopulation> zonePopulations = new List<ZonePopulation>();

		foreach (DataRow row in dtZones.Rows)
		{
			if (row["people_count"] != DBNull.Value)
			{
				string color = GenerateRandomColor();
				ZonePopulation zonePopulation = new ZonePopulation
				{
					Id = (int) row["id"],
					Label = row["name"].ToString(),
					BackgroundColor = color,
					BorderColor = color,
					Data = new List<Dictionary<string, object>>()
				};
				zonePopulations.Add(zonePopulation);
			}
		}

		query = "SELECT * FROM zone_population_data ORDER BY timestamp ASC;";
		DataTable dtLogbook = _dbRepository.SqlExecuteReader(query);

		foreach (DataRow row in dtLogbook.Rows)
		{
			foreach (ZonePopulation zonePopulation in zonePopulations)
			{
				if (zonePopulation.Id == Convert.ToInt16(row["zone_id"]))
				{
					zonePopulation.Data.Add(new Dictionary<string, object>
					{
						{ "x", Convert.ToDateTime(row["timestamp"]) },
						{ "y", Convert.ToInt32(row["people_count"]) }
					});
				}
			}
		}

		return zonePopulations;
	}
}