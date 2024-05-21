﻿/*
* File: HtmlToPdfConverter.cs
* Author: Warre Willeme & Jesse UijtdeHaag
* Date: 12-05-2024
* Description: this file contains the HtmlToPdfConverter class. This class is used to convert a html file to a pdf file using the html2pdf.com
*/

using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace crowd_management.classes;

public class HtmlToPdfConverter
{
	// This class is used to convert a html file to a pdf file using the html2pdf.com
	// This class mimics the request that is made when you upload a file to the website
	// This is not an official API with documentation

	#region Varibables and accessors

	private readonly LogbookHandler _logbookHandler = new LogbookHandler();
	private const string BaseUrl = "https://html2pdf.com";

	private string _sid = string.Empty;
	private string _fid = string.Empty;

	#endregion

	#region Methods

	private string ConvertToBase32(int value)
	{
		const string chars = "0123456789abcdefghijklmnopqrstuv";
		string result = "";

		do
		{
			result = chars[value % 32] + result;
			value /= 32;
		} while (value > 0);

		return result;
	}

	private string CreateId(int length = 16, string prefix = "")
	{
		Random random = new Random();
		string result = "";

		while (result.Length < length)
		{
			result += ConvertToBase32(random.Next(65535));
		}

		result = prefix + result.Substring(0, length);

		return result;
	}

	public (string, string) ConvertToPdf(string filePath)
	{
		_sid = CreateId();
		_fid = CreateId(26, "file_");

		ImportExternalCss(filePath);
		ConvertImgToBase64(filePath);

		UploadFile(filePath);
		CheckFileDelivery();
		var fileNames = DownloadFile(WaitForConversion());
		string fileName = fileNames.Item1;
		string friendlyName = fileNames.Item2;

		return (fileName, friendlyName);
	}

	private void ImportExternalCss(string filePath)
	{
		string htmlContent = File.ReadAllText(filePath);

		MatchCollection matches = Regex.Matches(htmlContent, @"<link\s+.*?href=[""'](.*?)[""'].*?>");

		foreach (Match match in matches)
		{
			string href = match.Groups[1].Value;

			if (Path.GetExtension(href).Equals(".css", StringComparison.OrdinalIgnoreCase))
			{
				string fullCssPath = Path.Combine(Path.GetDirectoryName(filePath) ?? string.Empty, href);
				string cssContent = MinimizeCss(File.ReadAllText(fullCssPath));

				htmlContent = htmlContent.Replace(match.Value, "<style>\n" + cssContent + "\n</style>");
			}
		}

		File.WriteAllText(filePath, htmlContent);
	}

	private string MinimizeCss(string css)
	{
		css = Regex.Replace(css, @"\/\*[\s\S]*?\*\/", string.Empty);
		css = Regex.Replace(css, @"\s+", " ");
		css = Regex.Replace(css, @"\s*([{};,:])\s*", "$1");
		css = css.Trim();

		return css;
	}

	static void ConvertImgToBase64(string filePath)
	{
		string htmlContent = File.ReadAllText(filePath);

		// Regular expression to match img tags with src attribute containing local file paths
		MatchCollection matches = Regex.Matches(htmlContent, @"<img\s+.*?src\s*=\s*(?:(?<!https?:\/\/)\s*[""'](?<url>[^\""']+\.(?:png|jpe?g|gif|bmp))[""'])\s*.*?>");

		foreach (Match match in matches)
		{
			string url = match.Groups["url"].Value;
			string fullImgPath = Path.Combine(Path.GetDirectoryName(filePath) ?? string.Empty, url);
			string base64Image;

			byte[] imageBytes = File.ReadAllBytes(fullImgPath);
			base64Image = Convert.ToBase64String(imageBytes);

			// Keep the original class attribute
			string imgTagWithBase64 = match.Value.Replace(url, $"data:image;base64,{base64Image}");

			htmlContent = htmlContent.Replace(match.Value, imgTagWithBase64);
		}

		File.WriteAllText(filePath, htmlContent);
	}

	private void UploadFile(string filePath)
	{
		using (WebClient client = new WebClient())
		{
			// Constructing the multipart request manually
			string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
			client.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);

			using (MemoryStream ms = new MemoryStream())
			using (StreamWriter writer = new StreamWriter(ms))
			{
				writer.WriteLine("--" + boundary);
				writer.WriteLine("Content-Disposition: form-data; name=\"id\"");
				writer.WriteLine();
				writer.WriteLine(_fid);

				writer.WriteLine("--" + boundary);
				writer.WriteLine("Content-Disposition: form-data; name=\"name\"");
				writer.WriteLine();
				writer.WriteLine(Path.GetFileName(filePath));

				writer.WriteLine("--" + boundary);
				writer.WriteLine($"Content-Disposition: form-data; name=\"file\"; filename=\"{Path.GetFileName(filePath)}\"");
				writer.WriteLine("Content-Type: text/html");
				writer.WriteLine();

				writer.Flush();

				// Writing file content to the request stream
				using (FileStream fs = File.OpenRead(filePath))
				{
					fs.CopyTo(ms);
				}

				writer.WriteLine();
				writer.WriteLine("--" + boundary + "--");

				// Reset the stream position before sending the request
				ms.Position = 0;

				// Uploading the request
				byte[] responseBytes = client.UploadData($"{BaseUrl}/upload/{_sid}", ms.ToArray());

				// Converting response to string
				string response = System.Text.Encoding.UTF8.GetString(responseBytes);

				dynamic jsonResponse = JsonConvert.DeserializeObject(response);

				if (jsonResponse?["data"]["file"] != Path.GetFileName(filePath))
				{
					_logbookHandler.AddLogbookEntry("Rapport", "System", "Uploaden van pdf.");
				}
			}
		}
	}

	private void CheckFileDelivery()
	{
		var client = new HttpClient();
		var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/convert/{_sid}/{_fid}");
		var response = client.SendAsync(request).Result.Content.ReadAsStringAsync().Result;

		dynamic jsonResponse = JsonConvert.DeserializeObject(response);

		if (jsonResponse?["status"] != "success")
		{
			_logbookHandler.AddLogbookEntry("Rapport", "System", "Conversie van HTML naar PDF mislukt.");
		}
	}

	private string WaitForConversion()
	{
		dynamic jsonResponse;
		do
		{
			var client = new HttpClient();
			var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/status/{_sid}/{_fid}");
			var response = client.SendAsync(request).Result.Content.ReadAsStringAsync().Result;

			jsonResponse = JsonConvert.DeserializeObject(response);
		} while (jsonResponse?["progress"] != 100);

		return jsonResponse["convert_result"];
	}

	private (string, string) DownloadFile(string resultPath)
	{
		var client = new HttpClient();
		var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/download/{_sid}/{_fid}/{resultPath}");
		Stream response = client.SendAsync(request).Result.Content.ReadAsStreamAsync().Result;

		DateTime createTime = DateTime.Now;
		string fileName = createTime.ToString("yyyyMMddHHmmss") + ".pdf";
		string friendlyName = $"{$"{createTime.Day:d2}/{createTime.Month:d2}/{createTime.Year} {createTime.Hour:d2}:{createTime.Minute:d2}"}";

		string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
		string relativePath = @"reports\" + fileName;
		string fullPath = Path.Combine(baseDirectory, relativePath);

		FileStream fs = File.Create(fullPath);
		response.CopyTo(fs);
		fs.Close();

		return (fileName, friendlyName);
	}

	#endregion
}