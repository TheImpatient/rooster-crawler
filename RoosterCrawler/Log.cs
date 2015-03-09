using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoosterCrawler
{
    public class Log
    {
        public Log(TaskSchedular.CrawlTask task,int klas, string status, string message)
        {
            string query = String.Format("INSERT INTO schedule_log (task_id, status, message) VALUES ({0},'{1}','{2}')", task.Id, status, message);
            DataParser.UpdateInternalData(query);
            // do call to log table
        }
    }
}
