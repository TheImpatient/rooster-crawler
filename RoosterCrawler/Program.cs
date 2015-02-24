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
            List<String>[] foo = HtmlParser();
            Console.WriteLine(foo);
            Console.Read();
        }
        //TODO: catch webclient errors / use html file instead / sanitise input / get around idiosyncrasy
        private static List<String>[] HtmlParser()
        {
            //WebClient webClient = new WebClient();
            //String page2 = new WebClient().DownloadString("http://misc.hro.nl/roosterdienst/webroosters/cmi/kw3/10/c/c00084.htm");

            String page = System.IO.File.ReadAllText("c00084.htm").Trim();

            HtmlDocument htmldoc = new HtmlDocument();
            htmldoc.LoadHtml(page);

            //TODO: Filter only relevant tabledata
            List<HtmlNode> table = htmldoc.DocumentNode.SelectNodes("//table").First().SelectNodes("tr").Descendants("td").Skip(1).ToList();


            //Lists all elements innerText per row
            List<String>[] strArr = new List<String>[37];
            for (int i = 0; i < strArr.Length; i++) { strArr[i] = new List<string>(); }

            //Lists all elements innerText
            List<String> strlist = new List<String>();

            int j = 0;

            foreach (HtmlNode h in table)
            {
                if (h.InnerText != "") { strArr[(int)((j) / 6)].Add(h.InnerText); }
                else { strArr[(int)((j) / 6)].Add("EMPTY"); }

                strlist.Add(h.InnerText);
                HtmlAttribute rowspan = h.Attributes.Where(x => x.Name.Equals("rowspan")).SingleOrDefault();

                int rowvalue = 0;

                //TODO: TryCatch parse error with non-numeric value
                try
                {
                    rowvalue = int.Parse(rowspan != null ? rowspan.Value : "EMPTY");
                }
                catch (FormatException e)
                {
                    //Chomp
                }

                if (rowvalue >= 4)
                {
                    for (int i = 1; i < rowvalue / 2; i++)
                    {
                        strArr[(int)((j) / 6) + i].Add(h.InnerText);
                    }
                }

                j++;
                Console.WriteLine("~~~~~~I:" + rowvalue);
                //Console.WriteLine("~~~~~~I:" + j);
            }
            return strArr;
        }
    }
}
