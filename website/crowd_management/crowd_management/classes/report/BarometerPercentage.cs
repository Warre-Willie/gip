using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
		var datasets = new List<string>();
		List<string> labels = new List<string>();
		List<int> greenData = new List<int>();
		List<int> orangeData = new List<int>();
		List<int> redData = new List<int>();


		foreach (var barometerPercentage in barometerPercentages)
		{
			labels.Add($"'{barometerPercentage.Zone}'");

			greenData.Add(barometerPercentage.PercentageGreen);
			orangeData.Add(barometerPercentage.PercentageOrange);
			redData.Add(barometerPercentage.PercentageRed);
		}

		datasets.Add($"{{ label: 'Groen', backgroundColor: ['lime'], data: [{string.Join(",", greenData)}] }}");
		datasets.Add($"{{ label: 'Geel', backgroundColor: ['yellow'], data: [{string.Join(",", orangeData)}] }}");
		datasets.Add($"{{ label: 'Rood', backgroundColor: ['red'], data: [{string.Join(",", redData)}] }}");

		// Constructing the JSON string
		string jsonData = @"
    {{
      ""type"": ""bar"",
      ""data"": {{
        ""labels"": [{0}],
        ""datasets"": [{1}]
      }},
      ""options"": {{
        ""scales"": {{
          ""xAxes"": [
            {{
							""scaleLabel"": {{
								display: true,
								labelString: 'Zones',
							}},
              ""stacked"": true,
              ""barThickness"": 50
            }}
          ],
          ""yAxes"": [
            {{
							""scaleLabel"": {{
								display: true,
								labelString: 'Percentages',
							}},
              ""ticks"": {{
                ""callback"": (val) => {{
                  return val + '%';
                }}
              }},
              ""stacked"": true
            }}
          ]
        }}
      }}
    }}";

		jsonData = string.Format(jsonData, string.Join(",", labels), string.Join(",", datasets));

		return jsonData;
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

			int seconds = (int) (currentTime - prevTime).TotalSeconds;

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
					double greenPercentage = (double) kvp.Value["green"] / totalSecondsZone * 100;
					double orangePercentage = (double) kvp.Value["orange"] / totalSecondsZone * 100;
					double redPercentage = (double) kvp.Value["red"] / totalSecondsZone * 100;

					int roundedGreen = (int) Math.Floor(greenPercentage);
					int roundedOrange = (int) Math.Floor(orangePercentage);
					int roundedRed = (int) Math.Floor(redPercentage);

					int totalRounded = roundedGreen + roundedOrange + roundedRed;

					// Calculate rounding difference
					double roundingDifference = 100 - totalRounded;

					if (roundingDifference > 0)
					{
						// Add rounding difference to the largest percentage rounding difference
						if (greenPercentage - roundedGreen > orangePercentage - roundedOrange && greenPercentage - roundedGreen > redPercentage - roundedRed)
						{
							roundedGreen += (int) roundingDifference;
						}
						else if (orangePercentage - roundedOrange > redPercentage - roundedRed)
						{
							roundedOrange += (int) roundingDifference;
						}
						else
						{
							roundedRed += (int) roundingDifference;
						}
					}
					else if (roundingDifference < 0)
					{
						// Subtract rounding difference from the largest percentage rounding difference
						if (greenPercentage - roundedGreen < orangePercentage - roundedOrange && greenPercentage - roundedGreen < redPercentage - roundedRed)
						{
							roundedGreen -= (int) Math.Abs(roundingDifference);
						}
						else if (orangePercentage - roundedOrange < redPercentage - roundedRed)
						{
							roundedOrange -= (int) Math.Abs(roundingDifference);
						}
						else
						{
							roundedRed -= (int) Math.Abs(roundingDifference);
						}
					}

					// Assign rounded percentages
					barometerPercentage.PercentageGreen = roundedGreen;
					barometerPercentage.PercentageOrange = roundedOrange;
					barometerPercentage.PercentageRed = roundedRed;
				}
			}
		}

		return barometerPercentages;
	}
}