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
        public int Weeknummer = 0;
        public UpdateResult UpdateResult = new UpdateResult();

        public Crawler(TaskSchedular.CrawlTask task, int klas)
        {            
            this.Task = task;
            this.Klas = klas;
            KlasUrl = KlasUrlBuilder(Klas);
            Weeknummer = task.Weken;
        }

        /// <summary>
        /// gather information about internal and external rooster, compare and sync if needed.
        /// </summary>
        /// <returns>true = internal rooster is up to date. false = something went wrong</returns>
        public bool Start()
        {
            try
            {
                var schedule = new Schedule(Weeknummer, KlasUrl);
                if (schedule.Compare() == false)
                {
                    //there are changes to sync
                    UpdateResult ur = schedule.Synchronize();
                    if (!ur.Completed)
                    {
                        this.UpdateResult = ur;
                        return false;
                    }
                    else
                    {
                        UpdateResult.Action = Log.DataAction.SyncDone;
                    }
                }
                else
                {
                    UpdateResult.Action = Log.DataAction.NoSync;
                }
            }
            catch (Exception e)
            {
                UpdateResult.Action = Log.DataAction.Exception;
                UpdateResult.Exception = e.ToString();
                return false;
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

    }
}
