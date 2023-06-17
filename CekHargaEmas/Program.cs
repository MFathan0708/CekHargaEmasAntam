using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.IO;

namespace CekHargaEmas
{
    internal class Program
    {
        static JObject dataSaved = new JObject();
        private static void cekHargaEmas()
        {
            HtmlWeb web = new HtmlWeb()
            {
                UseCookies = true,
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36",
                AutoDetectEncoding = true,
                //BrowserDelay = TimeSpan.FromSeconds(2)
            };
            Console.Title = "Cek Harga Emas Antam by Fathan";
            Console.WriteLine("\t\t===== Cek Harga Emas Antam by Fathan =====");
            Console.WriteLine();
            var htmlWeb = web.Load("https://www.logammulia.com/id/harga-emas-hari-ini");
            string nodeEmas = "";
            Console.WriteLine(htmlWeb.DocumentNode.SelectSingleNode("/html/body/section[3]/div/div[3]/h2").InnerText);
            foreach (HtmlNode htmlNode in htmlWeb.DocumentNode.SelectNodes(@"/html/body/section[3]/div/div[3]/table[1]"))
            {
                foreach (HtmlNode node in htmlNode.SelectNodes("tr"))
                {
                    if (node.SelectNodes("th") != null && node.SelectNodes("th").Count > 0)
                    {
                        if (node.Element("th").GetAttributes() != null && node.Element("th").GetAttributes().Count() > 0)
                        {
                            string th = node.Element("th").GetAttributes().First().Name;
                            if (th == "colspan")
                            {
                                nodeEmas = System.Text.RegularExpressions.Regex.Replace(node.InnerText, @"\n", "");
                                Console.WriteLine("===========================================");
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                if (dataSaved["Emas"]?[nodeEmas] == null)
                                {
                                    dataSaved["Emas"][nodeEmas] = new JObject();
                                }
                                Console.WriteLine(node.InnerText);
                            }
                        }
                    }
                    Console.ResetColor();
                    if (node.SelectNodes("td") != null && node.SelectNodes("td").Count > 0)
                    {
                        var a = node.InnerText.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        var data = dataSaved["Emas"][nodeEmas];
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Berat : " + a[0]);
                        if (!data.HasValues)
                            dataSaved["Emas"][nodeEmas] = new JObject();
                        if (dataSaved["Emas"]?[nodeEmas]?[a[0]] == null)
                        {
                            dataSaved["Emas"][nodeEmas][a[0]] = new JObject();
                            dataSaved["Emas"][nodeEmas][a[0]]["Berat"] = 0;
                            dataSaved["Emas"][nodeEmas][a[0]]["Harga Dasar"] = 0;
                            dataSaved["Emas"][nodeEmas][a[0]]["Harga (+Pajak PPh 0.25%)"] = 0;
                            Console.WriteLine("Harga Dasar : Rp. " + a[1]);
                            Console.WriteLine("Harga (+Pajak PPh 0.25%) : Rp. " + a[2]);
                        }
                        else
                        {
                            data = dataSaved["Emas"][nodeEmas][a[0]];
                            Int64 hargaDasar = Int64.Parse(data["Harga Dasar"].ToString().Replace(",", ""));
                            Int64 hargaPajak = Int64.Parse(data["Harga (+Pajak PPh 0.25%)"].ToString().Replace(",", ""));

                            Int64 hargaDasarWeb = Int64.Parse(a[1].Replace(",", ""));
                            Int64 hargaPajakWeb = Int64.Parse(a[2].Replace(",", ""));
                            Console.Write("Harga Dasar : ");
                            if (hargaDasar > hargaDasarWeb)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Rp. " + a[1]);
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            else if (hargaDasar < hargaDasarWeb)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Rp. " + a[1]);
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("Rp. " + a[1]);
                                Console.ForegroundColor = ConsoleColor.White;
                            }


                            Console.Write("Harga (+Pajak PPh 0.25%) : Rp. ");
                            if (hargaPajak > hargaPajakWeb)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Rp. " + a[2]);
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            else if (hargaPajak < hargaPajakWeb)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Rp. " + a[2]);
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("Rp. " + a[2]);
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                        }
                        dataSaved["Emas"][nodeEmas][a[0]]["Berat"] = Double.Parse(a[0].Replace(" gr", ""));
                        dataSaved["Emas"][nodeEmas][a[0]]["Harga Dasar"] = Int64.Parse(a[1].Replace(",", ""));
                        dataSaved["Emas"][nodeEmas][a[0]]["Harga (+Pajak PPh 0.25%)"] = Int64.Parse(a[2].Replace(",", ""));
                        Console.WriteLine();
                        Console.ResetColor();
                    }
                }
            }
        }

        private async static void cekNomorPenipu()
        {
            HttpClient httpClient = new HttpClient();

            // Create a cookie container and add some cookies
            var cookieContainer = new System.Net.CookieContainer();
            cookieContainer.Add(new System.Net.Cookie("Cookie", "remember_web_59ba36addc2b2f9401580f014c7f58ea4e30989d=eyJpdiI6ImczanFRVTd2emptd2xoTnUzN3puYUE9PSIsInZhbHVlIjoieHd1bWtDMmtBZjVZYXFRM0F0emkxU2ltbmJsa1R5Smg2cm1ZcVVHUWRoUnBWMlBEa1RkMTc5aVU3WTdLMFNGTHlqTnR3U2xDUU9laWpieFJ4eUg3T1hjY2FkaWhici8yMU53N1YrUlBUTzVzczBra0xQZzdod2RyVFNPWm5JVFJ5S2tLcEoyOFJkY1BiM3RPMTVOVUl3PT0iLCJtYWMiOiI3MDM2NmEzMzVkY2I3Yjc3MmYxYWY3NGUzZGNkNzQwOTlkMDFhM2IwMmEyOWE1NDYyNTU3OGEzYmMwM2FmNmJmIn0%3D; XSRF-TOKEN=eyJpdiI6IkdjRThqUUpRNFFoMjFIeHd5S2l5a0E9PSIsInZhbHVlIjoiTzhQajAySWxadC8xcEJ0Z212NFV3MTJIVHBQNGw0MllqNnAzeEZvaHZXUlBZZi9TaVNlMTB1a2JHL2ZRemN6bnp2NlhKZEtVZ3VkUmpFcEp1TnBMOW56c21CRGdKMnAydG9Qd09wbHFVNy93QzNmUk1WZ2lCVG9aelVzVk42RmMiLCJtYWMiOiJiYTM2NGJiY2M2ZDQxNzJmMWJhOWJkMjhkYjQ1MmUyN2U1NTJkNDU0YjljYzI0MGJmOTgyODcxYTBkZTZkN2MzIn0%3D; kredibelcoid_session=eyJpdiI6IjBZdTB1Rmh4azB0RDdzL2tIVmt4eEE9PSIsInZhbHVlIjoiZmEvVEoydlUreVVPbFcxSGNjekx3a0U3djQxbDBWUUVtWTUwai9Tb1g1TUcrdnlxZUhYOVBrU3VQK3JmQnIrc0xaZ2N5L1ZQQWpZTitVbHVEUWxrUHFjbjJIT2xwWkhlcE83eXQ3VTd4M1VQRndud0p1Nld4ZTV6U3dXL3JySTIiLCJtYWMiOiI2NmFkOWZiMmQ1MTA1NGNjOWNkMzdjMjFjMTMwOGYwYjkxYWQyY2VkNDI5NmJmOTYzYWQxMTU0ZjNjZjU3NmJlIn0%3D", "/", "https://www.kredibel.com/"));
            
            // Set the HttpClient's cookie container
            httpClient.DefaultRequestHeaders.Add("Cookie", cookieContainer.GetCookieHeader(new Uri("https://www.kredibel.com/")));

            // Make an HTTP request using HttpClient
            HttpResponseMessage response = await httpClient.GetAsync("https://www.kredibel.com/phone/id/81246356563");

            // Get the HTML content from the response
            string htmlContent = await response.Content.ReadAsStringAsync();

            // Parse the HTML content using HtmlAgilityPack
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            Console.Title = "Cek Nomor Telpon by Fathan";
            Console.WriteLine("\t\t===== Cek Nomor Telpon by Fathan =====");
            Console.WriteLine();
            Console.WriteLine(htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"app\"]/div/div[2]/section/div/div/div[1]/aside[2]/div[1]/div[2]/div[2]").InnerText);
            /*foreach (HtmlNode htmlNode in htmlWeb.DocumentNode.SelectNodes(@"/html/body/section[3]/div/div[3]/table[1]"))
            {
                foreach (HtmlNode node in htmlNode.SelectNodes("tr"))
                {
                    if (node.SelectNodes("th") != null && node.SelectNodes("th").Count > 0)
                    {
                        if (node.Element("th").GetAttributes() != null && node.Element("th").GetAttributes().Count() > 0)
                        {
                            string th = node.Element("th").GetAttributes().First().Name;
                            if (th == "colspan")
                            {
                                Console.WriteLine("===========================================");
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(node.InnerText);
                            }
                        }
                    }
                    Console.ResetColor();
                    if (node.SelectNodes("td") != null && node.SelectNodes("td").Count > 0)
                    {
                        var a = node.InnerText.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        Console.WriteLine("Berat : " + a[0]);
                        Console.WriteLine("Harga Dasar : Rp. " + a[1]);
                        Console.WriteLine("Harga NPWP (+Pajak 0.45%) : Rp. " + a[2]);
                        Console.WriteLine("Harga Non NPWP (+Pajak 0.90%) : Rp. " + a[3]);
                        Console.WriteLine();
                    }
                }
            }*/
        }
        static void Main(string[] args)
        {
            dataSaved = JObject.Parse(File.ReadAllText("dataSaved.json"));
            while(true)
            {
                cekHargaEmas();
                File.WriteAllText("dataSaved.json", dataSaved.ToString());
                Console.ReadKey();
                break;
            }
        }
    }
}
