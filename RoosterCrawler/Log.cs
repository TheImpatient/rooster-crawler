using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace RoosterCrawler
{
    public class Log
    {
        public enum DataAction
        {
            SyncDone,
            NoSync,
            Exception
        }

        public Log(TaskSchedular.CrawlTask task, UpdateResult details)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            string query = String.Format("INSERT INTO schedule_log (task_id, status, duration, details) VALUES ({0},'{1}','{2}','{3}')", task.Id, details.Completed == true ? "success":"error", details.Duration, jss.Serialize(details));
            DataParser.UpdateInternalData(query);
            // do call to log table
        }
    }
}
