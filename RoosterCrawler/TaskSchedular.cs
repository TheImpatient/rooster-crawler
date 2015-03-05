using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoosterCrawler
{
    public class TaskSchedular
    {
        private int crawlInterval = 10;
        private int refreshTasks = 5;
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

            //interprete information, check if a crawl task has to run
            if (TaskToRun())
            {
                new Crawler().Start(GetTasksToRun());
            }
        }

        private bool TaskToRun()
        {
            return true;
        }

        private List<CrawlTask> GetTasksToRun()
        {
            //get task that need to bee run and return it in a list

            return new List<CrawlTask>();
        }

        public void GetSchedule()
        {
            // refresh inforamtion from the crawl_schedule table    
        }

        public struct CrawlTask
        {
            public List<int> Rooters;
        }
    }
}
