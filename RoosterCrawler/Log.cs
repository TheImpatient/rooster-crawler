using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

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
            if (!String.IsNullOrEmpty(details.Exception))
            {
                details.Exception = WebUtility.HtmlEncode(details.Exception);
            }
            
            string query = String.Format("INSERT INTO schedule_log (task_id, status, duration, details) VALUES ({0},'{1}','{2}','{3}')", task.Id, details.Completed == true ? "success":"error", details.Duration, JsonConvert.SerializeObject(details));
            DataParser.UpdateInternalData(query);
            // do call to log table
        }
    }
}
