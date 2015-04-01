using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgendaAgent
{
    public class Program
    {
        private Agenda _agenda;

        public Program()
        {
            _agenda = new Agenda();
        }

        public void Start()
        {
            while (true)
            {
                var taskList = Repository.GetAgendaTasks();
                if (taskList.Any())
                {
                    //we got tasks
                    foreach (var lesTaak in taskList)
                    {
                        if (_agenda.LesEventHandler(lesTaak))
                        {
                            //all is OK
                            //remove task from agenda_tasks table
                            Repository.DeleteAgendaTask(lesTaak.Id);
                        }
                        else
                        {
                            //something went wrong
                        }
                    }
                }
                else
                {
                    //no tasks, wait for 1 minute and check again
                    Thread.Sleep(1 * 60 * 1000);
                }

                //update google agenda info so app has same info as gcalendar
            }
        }
    }
}
