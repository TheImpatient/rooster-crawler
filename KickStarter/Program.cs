using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace KickStarter
{
    class Program
    {
        static void Main(string[] args)
        {
            var crawlerThread = new Thread(new ThreadStart(Crawler));
            var agendaThread = new Thread(new ThreadStart(Agenda));
            
            crawlerThread.Start();
            agendaThread.Start();           
        }

        static void Agenda()
        {
            AgendaAgent.Program agenda = new AgendaAgent.Program();
            agenda.Start(); 
        }

        static void Crawler()
        {
            RoosterCrawler.Program app = new RoosterCrawler.Program();
            app.Main();
        }
    }
}
