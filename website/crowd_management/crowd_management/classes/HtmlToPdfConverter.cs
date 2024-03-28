using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace crowd_management.classes
{
    public class HtmlToPdfConverter
    {
        // This class is used to convert a html file to a pdf file using the html2pdf.com
        // This class mimics the request that is made when you upload a file to the website
        // This is not a official api with documentation

        string baseUrl = "https://html2pdf.com";

        string sid = string.Empty;
        string fid = string.Empty;

        public static string ConvertToBase32(int value)
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

        public static string CreateId(int length = 16, string prefix = "")
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
            sid = CreateId();
            fid = CreateId(26, "file_");

            UploadFile(filePath);
            CheckFileDelivery();
            var fileNames = DownloadFile(WaitForConversion());
            string fileName = fileNames.Item1;
            string friendlyName = fileNames.Item2;

            return (fileName, friendlyName);
        }

        private void UploadFile(string filePath)
        {
            try
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
                        writer.WriteLine(fid);

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
                        byte[] responseBytes = client.UploadData($"{baseUrl}/upload/{sid}", ms.ToArray());

                        // Converting response to string
                        string response = System.Text.Encoding.UTF8.GetString(responseBytes);

                        dynamic jsonResponse = JsonConvert.DeserializeObject(response);

                        if (jsonResponse["data"]["file"] == Path.GetFileName(filePath))
                        {
                            return;
                        }
                        else
                        {
                            throw new Exception("File upload failed");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void CheckFileDelivery()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/convert/{sid}/{fid}");
            var response = client.SendAsync(request).Result.Content.ReadAsStringAsync().Result;

            dynamic jsonResponse = JsonConvert.DeserializeObject(response);

            if (jsonResponse["status"] == "success")
            {
                return;
            }
            else
            {
                throw new Exception("File conversion failed");
            }
        }

        private string WaitForConversion()
        {
            dynamic jsonResponse = null;
            do
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/status/{sid}/{fid}");
                var response = client.SendAsync(request).Result.Content.ReadAsStringAsync().Result;

                jsonResponse = JsonConvert.DeserializeObject(response);
            }
            while (jsonResponse["progress"] != 100);

            return jsonResponse["convert_result"];
        }

        private (string, string) DownloadFile(string resultPath)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/download/{sid}/{fid}/{resultPath}");
            Stream response = client.SendAsync(request).Result.Content.ReadAsStreamAsync().Result;

            DateTime createTime = DateTime.Now;
            string fileName = createTime.ToString("yyyyMMddHHmmss") + ".pdf";
            string friendlyName = $"{$"{createTime.Day:d2}/{createTime.Month:d2}/{createTime.Year} {createTime.Hour:d2}:{createTime.Minute:d2}"}";
            
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string relativePath = @"rapports\" + fileName;
            string fullPath = Path.Combine(baseDirectory, relativePath);

            FileStream fs = File.Create(fullPath);
            response.CopyTo(fs);
            fs.Close();

            return (fileName, friendlyName);
        }
    }
}