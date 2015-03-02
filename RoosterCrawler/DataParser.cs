using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Net;

namespace RoosterCrawler
{
    public static class DataParser
    {
        //TODO: catch webclient errors / sanitise input more / Divide parts into functions for readability / Consider giving Week some of these functions (Maybe all)
        public static Week GetExternalWeekSchedule(int weekNummer)
        {
            WebClient webClient = new WebClient();

            //TODO: catch and handle 404exception, and other webclient exceptions
            String page = new WebClient().DownloadString("http://misc.hro.nl/roosterdienst/webroosters/cmi/kw3/" + weekNummer + "/c/c00084.htm").Trim();

            //String page = System.IO.File.ReadAllText("c00084.htm").Trim();

            page = page.Replace("\r", String.Empty).Replace("\n", String.Empty);

            HtmlDocument htmldoc = new HtmlDocument();
            htmldoc.LoadHtml(page);

            List<HtmlNode> table = htmldoc.DocumentNode.SelectNodes("//table").First().SelectNodes("tr/td").ToList();


            //Contains all data per week, day, hour
            Week week = new Week();



            //Lists all elements innerText
            List<String> innerTextList = new List<String>();
            List<int> rowspanList = new List<int>();


            int rowCount = 0;
            int columnCount = 0;
            List<HtmlNode> tableRows = htmldoc.DocumentNode.SelectNodes("//table").First().SelectNodes("tr").Skip(1).Where((x, i) => i % 2 == 0).ToList();
            IEnumerable<HtmlNode> tableDatas = new List<HtmlNode>();
            HtmlAttribute rowspan;
            foreach (HtmlNode tr in tableRows)
            {
                columnCount = 0;
                Console.WriteLine(tr.ChildNodes.Count-1);

                tableDatas = tr.ChildNodes.Skip(1);
                foreach (HtmlNode td in tableDatas)
                {                    
                    for (int i = 0; i < week.Days.Length - columnCount; i++)
                    {
                        if (week.Days[columnCount + i].Available(rowCount))
                        {
                            //Check for bold text
                            HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(td.InnerHtml);

                            if (td.InnerText != "" && doc.DocumentNode.SelectNodes("//tr").First().SelectNodes("td/font/b") != null)//er is een lokaal bekent
                            {
                                string[] a = td.InnerText.Split(')');
                                string[] b = a[0].Split(' ');

                                week.Days[columnCount + i].Add(
                                    new Les()
                                    {
                                        Lokaal = b[0],
                                        Docent = b[1],
                                        Vak = a[1].Trim(),
                                        VakCode = b[2],
                                        VakId = int.Parse(b[3])
                                    });
                            }
                            else if (td.InnerText != "")//er is geen lokaal bekent
                            {
                                string[] a = td.InnerText.Split(')');
                                string[] b = a[0].Split(' ');

                                week.Days[columnCount + i].Add(
                                    new Les()
                                    {
                                        Lokaal = String.Empty,
                                        Docent = b[0],
                                        Vak = a[1].Trim(),
                                        VakCode = b[1],
                                        VakId = int.Parse(b[2])
                                    });
                            }
                            else// geen les
                            {
                                
                            }

                            ////check for consecutive lessons
                            //Check for rowspan
                            rowspan = td.Attributes.Where(x => x.Name.Equals("rowspan")).SingleOrDefault();
                            int rowvalue = 0;
                            try
                            {
                                rowvalue = int.Parse(rowspan != null ? rowspan.Value : "EMPTY");
                                rowspanList.Add(rowvalue);
                            }
                            catch (FormatException e)
                            {
                                rowspanList.Add(0);
                            }
                            //wat doet deze check ? 
                            if (rowvalue >= 4)
                            {
                                for (int j = 1; j < rowvalue / 2; j++)
                                {
                                    //Check for bold text
                                    doc.LoadHtml(td.InnerHtml);
                                    if (td.InnerText != "" && doc.DocumentNode.SelectNodes("//tr").First().SelectNodes("td/font/b") != null)//lokaal bekent
                                    {
                                        string[] a = td.InnerText.Split(')');
                                        string[] b = a[0].Split(' ');

                                        week.Days[columnCount + i].Add(
                                            new Les()
                                            {
                                                Lokaal = b[0],
                                                Docent = b[1],
                                                Vak = a[1].Trim(),
                                                VakCode = b[2],
                                                VakId = int.Parse(b[3])
                                            });
                                    }
                                    else if (td.InnerText != "")//er is geen lokaal bekent
                                    {
                                        string[] a = td.InnerText.Split(')');
                                        string[] b = a[0].Split(' ');

                                        week.Days[columnCount + i].Add(
                                            new Les()
                                            {
                                                Lokaal = String.Empty,
                                                Docent = b[0],
                                                Vak = a[1].Trim(),
                                                VakCode = b[1],
                                                VakId = int.Parse(b[2])
                                            });
                                    }
                                    else// geen les
                                    {
                                        
                                    }
                                }
                            }

                            
                            break;
                        }
                    }


                    Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~");
                    Console.WriteLine(td.InnerHtml);


                    columnCount++;
                }
                rowCount++;
            }
            return week;
        }

        public static Week GetInternalWeekSchedule()
        {
            return new Week();
        }
    }
}
