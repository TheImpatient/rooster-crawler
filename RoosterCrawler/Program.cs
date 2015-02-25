using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace RoosterCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            Week foo = HtmlParser();
            Console.WriteLine(foo);
            Console.Read();
        }
        //TODO: catch webclient errors / sanitise input / get around idiosyncrasy / loop TR's instead of TD's to track row
        private static Week HtmlParser()
        {
            //WebClient webClient = new WebClient();
            //String page2 = new WebClient().DownloadString("http://misc.hro.nl/roosterdienst/webroosters/cmi/kw3/10/c/c00084.htm");

            String page = System.IO.File.ReadAllText("c00084.htm").Trim();

            HtmlDocument htmldoc = new HtmlDocument();
            htmldoc.LoadHtml(page);

            //TODO: Filter only relevant tabledata
            List<HtmlNode> table = htmldoc.DocumentNode.SelectNodes("//table").First().SelectNodes("tr/td").ToList();


            //Lists all elements innerText per row

            //List<String>[] strArr = new List<String>[37];
            //for (int i = 0; i < strArr.Length; i++) { strArr[i] = new List<string>(); }

            Week week = new Week();



            //Lists all elements innerText
            List<String> innerTextList = new List<String>();
            List<int> rowspanList = new List<int>();


            int j = 0;

            foreach (HtmlNode h in table)
            {

                //String inner = h.InnerText != "" ? h.InnerText : "EMPTY";
                for (int i = 0; i < week.days.Length - j % 6; i++)
                {
                    if (week.days[j % 6+i].Available((int)j / 6))
                    {
                        week.days[j % 6+i].Add(h.InnerText);
                        break;
                    }
                }



                innerTextList.Add(h.InnerText);

                HtmlAttribute rowspan = h.Attributes.Where(x => x.Name.Equals("rowspan")).SingleOrDefault();
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



                if (rowvalue >= 4)
                {
                    for (int i = 1; i < rowvalue / 2; i++)
                    {
                        week.days[j % 6].Add(h.InnerText);
                    }
                }


                j++;
                Console.WriteLine("~~~~~~I:" + rowvalue);
            }
            return week;
        }
    }
}
