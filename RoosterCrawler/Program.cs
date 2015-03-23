using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Globalization;
using MySql.Data.MySqlClient;

namespace RoosterCrawler
{
    public class Program
    {
        public void Main()
        {
            //start task schedular
            // > task schedular 
            // | haal informatie op welke rooster onderdelen gecrawlt moet worden en om de hoeveel tijd dit moet gebeuren , crawl info van db table 'crawl_schedule'
            // | op gedicteerde tijden start de task schedular de rooster crawler en geeft mee welke rooster onderdelen gecrawlt moeten worden
            // > rooster crawler
            // | haalt extern rooster op (voegtoe: html pagina nummer dat opgeroepen is)
            // | haalt intern rooster op (haalop: where html pagina nummer zie boventstaand)
            // | vergelijkt extern met intern en update het interne rooster indien nodig (zorg ervoor dat je de juiste week planningen met elkaar vergelijkt)

            var h = new Heartbeat();

            var t = new TaskSchedular();
            t.Start();

            //var a = new Schedule(13, "c00001");
            //var b = a.Synchronize();
            //Console.WriteLine(b.ToString());
        }
    }
}
