using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using QuickChart;

namespace crowd_management.classes.report;

public class BarometerPeriod
{
    public int ZoneId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Color { get; set; }
}

public class BarometerDataSet
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}

public class BarometerTimeline
{
    private int Id { get; set; }
    private string Zone { get; set; }
    private List<Dictionary<string, List<BarometerDataSet>>> Data { get; set; }

    private readonly DbRepository _dbRepository = new DbRepository();

    public void MakeGraph(string contentPath)
    {
        string json = MakeJson();

        Chart qc = new Chart();

        qc.Width = 1300;
        qc.Height = 120;
        qc.Version = "3";

        qc.Config = json;

        string graphBase64 = Convert.ToBase64String(qc.ToByteArray());

        string relativePath = "~/reports/barometer_timeline.html";
        string fullPath = relativePath.Replace("~", AppDomain.CurrentDomain.BaseDirectory);
        string graphHtml = File.ReadAllText(fullPath);

        graphHtml = graphHtml.Replace("{{graphBase64}}", graphBase64);
        string templateContent = File.ReadAllText(contentPath);
        templateContent += graphHtml;
        File.WriteAllText(contentPath, templateContent);
    }

    private string MakeJson()
    {
        List<BarometerTimeline> barometerTimelines = GetChartData();

        var datasets = new List<string>();

        foreach (var timeline in barometerTimelines)
        {
            foreach (var colorData in timeline.Data)
            {
                foreach (var color in colorData.Keys)
                {
                    var dataPoints = new List<string>();

                    foreach (var data in colorData[color])
                    {
                        string startX = $"new Date('{data.Start:yyyy-MM-ddTHH:mm:ss}')";
                        string endX = $"new Date('{data.End:yyyy-MM-ddTHH:mm:ss}')";

                        string dataPoint = $"{{ x: [{startX}, {endX}], y: '{timeline.Zone}' }}";
                        dataPoints.Add(dataPoint);
                    }

                    string dataset = $"{{ label: '{color}', data: [{string.Join(",", dataPoints)}], backgroundColor: ['{GetChartColor(color)}'] }}";
                    datasets.Add(dataset);
                }
            }
        }

        Dictionary<string, DateTime> firstAndLastTimestamps = GetFirstAndLastTimestamps(barometerTimelines);

        string minTimestamp = firstAndLastTimestamps["FirstTimestamp"].ToString("yyyy-MM-ddTHH:mm:ss");
        string maxTimestamp = firstAndLastTimestamps["LastTimestamp"].ToString("yyyy-MM-ddTHH:mm:ss");

        string labels = string.Join(",", barometerTimelines.Select(t => $"'{t.Zone}'"));

        // Constructing the JSON string
        string jsonData = @"
    {{
        type: 'bar',
        data: {{
						labels: [{0}],
            datasets: [{1}]
        }},
        options: {{
            animation: false,
            barPercentage: 1,
            indexAxis: 'y',
            aspectRatio: 10,
            scales: {{
                x: {{
                    type: 'time',
                    min: new Date('{2}'),
                    max: new Date('{3}'),
                    ticks: {{
                        maxRotation: 45,
                        minRotation: 45,
                    }},
                    time: {{
                        unit: 'minute',
                        stepSize: 15,
                        autoSkip: false,
                        displayFormats: {{
                            hour: 'HH:mm',
                            minute: 'HH:mm',
                        }},
                    }},
                }},
                y: {{
                    beginAtZero: true,
                    stacked: true,
                    ticks: {{
                        display: true,
                    }},
                    grid: {{
                        display: false,
                    }},
                }},
            }},
            plugins: {{
                legend: {{
                    display: false,
                }},
            }},
        }},
    }}";

        jsonData = string.Format(jsonData, labels, string.Join(",", datasets), minTimestamp, maxTimestamp);

        return jsonData;
    }

    private string GetChartColor(string color)
    {
        switch (color)
        {
            case "green":
                return "lime";
            case "orange":
                return "yellow";
            case "red":
                return "red";
            default:
                return string.Empty;
        }
    }

    private Dictionary<string, DateTime> GetFirstAndLastTimestamps(List<BarometerTimeline> barometerTimelines)
    {
        DateTime firstTimestamp = DateTime.MaxValue;
        DateTime lastTimestamp = DateTime.MinValue;

        foreach (var timeline in barometerTimelines)
        {
            foreach (var colorData in timeline.Data)
            {
                foreach (var colorDataEntry in colorData)
                {
                    foreach (var data in colorDataEntry.Value)
                    {
                        if (data.Start < firstTimestamp)
                        {
                            firstTimestamp = data.Start;
                        }

                        if (data.End > lastTimestamp)
                        {
                            lastTimestamp = data.End;
                        }
                    }
                }
            }
        }

        var timestamps = new Dictionary<string, DateTime>();
        timestamps["FirstTimestamp"] = firstTimestamp;
        timestamps["LastTimestamp"] = DateTime.Now;

        return timestamps;
    }


    private List<BarometerTimeline> GetChartData()
    {
        List<BarometerPeriod> barometerPeriods = new List<BarometerPeriod>();

        string query = "SELECT * FROM barometer_logbook ORDER BY timestamp ASC";
        DataTable dtLogbook = _dbRepository.SqlExecuteReader(query);

        if (dtLogbook.Rows.Count >= 2)
        {
            foreach (DataRow row in dtLogbook.Rows)
            {
                int currentIndex = dtLogbook.Rows.IndexOf(row);

                DateTime currentTimestamp = DateTime.Parse(row["timestamp"].ToString());
                DateTime nextTimestamp = new DateTime();
                if (dtLogbook.Rows.Count - 1 == currentIndex)
                {
                    nextTimestamp = DateTime.Now;
                }
                else
                {
                    nextTimestamp = DateTime.Parse(dtLogbook.Rows[currentIndex + 1]["timestamp"].ToString());
                }


                barometerPeriods.Add(new BarometerPeriod
                {
                    ZoneId = Convert.ToInt32(row["zone_id"]),
                    Start = currentTimestamp,
                    End = nextTimestamp,
                    Color = row["color"].ToString()
                });
            }
        }


        var groupedPeriods = barometerPeriods.GroupBy(p => p.ZoneId);

        List<BarometerTimeline> barometerTimelines = new List<BarometerTimeline>();
        foreach (var group in groupedPeriods)
        {
            BarometerTimeline timeline = new BarometerTimeline
            {
                Id = group.Key,
                Data = new List<Dictionary<string, List<BarometerDataSet>>>()
            };

            var groupedPeriodsByColor = group.GroupBy(p => p.Color);
            foreach (var colorGroup in groupedPeriodsByColor)
            {
                Dictionary<string, List<BarometerDataSet>> colorData = new Dictionary<string, List<BarometerDataSet>>();
                List<BarometerDataSet> datasets = new List<BarometerDataSet>();

                foreach (var period in colorGroup)
                {
                    datasets.Add(new BarometerDataSet
                    {
                        Start = period.Start,
                        End = period.End
                    });
                }

                colorData.Add(colorGroup.Key, datasets);
                timeline.Data.Add(colorData);
            }

            barometerTimelines.Add(timeline);
        }

        foreach (var barometerTimeline in barometerTimelines)
        {
            query = $"SELECT name FROM zones WHERE id = {barometerTimeline.Id}";
            DataTable dtZone = _dbRepository.SqlExecuteReader(query);
            barometerTimeline.Zone = dtZone.Rows[0]["name"].ToString();
        }

        return barometerTimelines;
    }
}