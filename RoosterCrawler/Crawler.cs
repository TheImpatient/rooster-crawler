using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoosterCrawler
{
    public class Crawler
    {
        public TaskSchedular.CrawlTask Task;
        public int Klas;
        public string KlasUrl = String.Empty;
        public string WeekUrl = String.Empty;
        public string log = String.Empty;

        public Crawler(TaskSchedular.CrawlTask task, int klas)
        {            
            this.Task = task;
            this.Klas = klas;
            KlasUrl = KlasUrlBuilder(Klas);
            WeekUrl = WeekUrlBuilder(task.Weken);
        }

        /// <summary>
        /// gather information about internal and external rooster, compare and sync if needed.
        /// </summary>
        /// <returns>true = internal rooster is up to date. false = something went wrong</returns>
        public bool Start()
        {
            //TODO weken moet hieronder nog goed komen
            var schedule = new Schedule(WeekUrl, KlasUrl);
            if (schedule.Compare()==false)
            {
                //there are changes to sync
                UpdateResult ur = schedule.Synchronize();
                if (!ur.Completed)
                {
                    this.log = ur.Log;
                    return false;
                }
            }

            return true;
        }

        public string KlasUrlBuilder(int klas)
        {
            var urlBuilder = new StringBuilder("c");

            for (int i = 0; i < (5 - klas.ToString().Length); i++)
            {
                urlBuilder.Append("0");
            }

            urlBuilder.Append(klas);

            return urlBuilder.ToString();
        }

        public string WeekUrlBuilder(int week)
        {
            var urlBuilder = new StringBuilder("");

            if (week < 10)
            {
                urlBuilder.Append("0");
            }

            urlBuilder.Append(week);

            return urlBuilder.ToString();
        }

    }
}
