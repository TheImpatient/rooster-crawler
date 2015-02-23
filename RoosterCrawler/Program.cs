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
        /*TODO: catch webclient errors / use html file instead / sanitise input / get around idiosyncrasy
        private static List<String>[] HtmlParser()
        {
            WebClient webClient = new WebClient();
            String page = new WebClient().DownloadString("http://misc.hro.nl/roosterdienst/webroosters/cmi/kw3/08/c/c00085.htm");
            
            HtmlDocument htmldoc =  new HtmlDocument();
            htmldoc.LoadHtml(page);

            List<HtmlNode> table = htmldoc.DocumentNode.SelectNodes("//table").First().SelectNodes("tr").Descendants("td").Skip(1).ToList();

            int i = 0;

            List<String>[] strArr = new List<String>[30];

            List<String> strlist = new List<String>();

            foreach (HtmlNode h in table)
            {
                var bla = (int)((i) / 6);
                strArr[(int)((i) / 6)].Add(h.InnerText);
                strlist.Add(h.InnerText);
                HtmlAttribute rowspan = h.Attributes.Where(x => x.Name.Equals("rowspan")).SingleOrDefault();

                String rowvalue = rowspan != null ? rowspan.Value : "EMPTY";
                i++;
                Console.WriteLine("~~~~~~I:" + i);
                
            }

            return strArr;

        }
        */

    }
}
