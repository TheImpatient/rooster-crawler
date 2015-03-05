using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace RoosterCrawler
{
    public class TaskSchedular
    {
        private List<CrawlTask> _tasks = new List<CrawlTask>(); 

        public TaskSchedular()
        {
            // > task schedular 
            // | haal informatie op welke rooster onderdelen gecrawlt moet worden en om de hoeveel tijd dit moet gebeuren , crawl info van db table 'crawl_schedule'
            // | op gedicteerde tijden start de task schedular de rooster crawler en geeft mee welke rooster onderdelen gecrawlt moeten worden
        }

        public void Start()
        {
            // call GetSchedule to get information about what and when to schedule crawl action (via thread elke 5 min herhalen ?)
            // voor nu laten we dit deel er nog ff uit tot we precies weten wat we willen en wat de structuur is
            GetSchedule();

            //interprete information, check if a crawl task has to run
            if (TaskToRun())
            {
                //misschien hier de tijd pakken, als een task te snel klaar is zodat we niet geblokt worden door school
                foreach (CrawlTask crawlTask in GetTasksToRun())
                {
                    foreach (int klas in crawlTask.Klassen)
                    {
                        var crawler = new Crawler(crawlTask, klas);
                        string logMessage = String.Empty;
                        string completed = String.Empty;

                        if (crawler.Start())
                        {
                            //all done 
                            // do a log wright 
                            logMessage = "";
                            completed = "success";
                        }
                        else
                        {
                            // return false >> prob an error 
                            // do a log wright 

                            logMessage = crawler.log;
                            completed = "error";
                        }
                        new Log(crawlTask, klas, completed, logMessage);
                    }        
                }
            }
        }

        private bool TaskToRun()
        {
            if (_tasks.Any(x => x.Datetime.Date.Equals(DateTime.Now.Date)))
            {
                return true;
            }

            return false;
        }

        private List<CrawlTask> GetTasksToRun()
        {
            //get task that need to bee run and return it in a list

            var list = _tasks.Where(x => x.Datetime.Date.Equals(DateTime.Now.Date)).ToList();

            return list;
        }

        public void GetSchedule()
        {


            int temp_raw_id = 1;
            string temp_raw_klassen = "{\"klassen\":[84,82,81]}";
            int temp_raw_interval = 5;
            int temp_raw_weken = 2;
            DateTime temp_raw_datetime = DateTime.Parse("2015-03-05 10:22:57");

            var jss = new JavaScriptSerializer();
            var b = jss.Deserialize<CrawlTask>(temp_raw_klassen);
            b.Interval = temp_raw_interval;
            b.Weken = temp_raw_weken;
            b.Datetime = temp_raw_datetime;
            b.Id = temp_raw_id;

            _tasks.Add(b);

            var b2 = jss.Deserialize<CrawlTask>(temp_raw_klassen);
            b2.Id = 2;
            b2.Interval = temp_raw_interval;
            b2.Weken = temp_raw_weken;
            b2.Datetime = DateTime.Parse("2015-03-06 10:22:57");

            _tasks.Add(b2);
            // refresh inforamtion from the crawl_schedule table    
        }

        public struct CrawlTask
        {
            public int Id;
            public List<int> Klassen;
            public int Interval;
            public int Weken;
            public DateTime Datetime;
        }    
    }
}
