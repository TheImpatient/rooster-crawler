using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

namespace AgendaAgent
{
    internal class Agenda
    {
        private Events _agenda;
        private CalendarService _service;

        public Agenda()
        {
            if (Repository.GetData())
            {
                _service = Repository.GetCalendar();
                _agenda = Repository.GetAgenda();
            }           
        }

        public bool LesEventHandler(LesTaak task)
        {
            switch(task.Action)
            {
                case LesTaak.TaakAction.Create:
                    return AddEvent(task.Les);

                case LesTaak.TaakAction.Update:
                    return UpdateEvent(task.Les);

                case LesTaak.TaakAction.Delete:
                    return DeleteEvent(task.Les.Guid);
                
                default:
                    return false;
            }
        }

        public bool AddEvent(Les les)
        {
            string guid = System.Guid.NewGuid().ToString();
            var startTime = new EventDateTime()
            {
                DateTime = les.StartTijd
            };

            var endTime = new EventDateTime()
            {
                DateTime = les.StartTijd.Add(les.Lengte)
            };

            //var attendeeList = new List<EventAttendee>();
            //attendeeList.Add(new EventAttendee()
            //{
            //    DisplayName = Regex.Split(les.organizer, "@")[0],
            //    Email = les.organizer
            //});

            //string[] aanwezigen = Regex.Split(les.attendeeEmail, ";");

            //for (int i = 0; i < aanwezigen.Length; i++)
            //{
            //    var ea = new EventAttendee()
            //    {
            //        DisplayName = Regex.Split(aanwezigen[i], "@")[0],
            //        Email = aanwezigen[i]
            //    };
            //    attendeeList.Add(ea);
            //}

            //TODO locatie nog weizigen naar echte locatie
            var event1 = new Event()
            {
                Id = guid,
                Summary = les.Vak,
                Start = startTime,
                End = endTime,
                GuestsCanModify = false,  
                GuestsCanSeeOtherGuests = false,               
                Description = "Vak: " + les.Vak + "\r" + "Vak code: " + les.VakCode + "\r" + "Docent: " + les.Docent + "\r" + "Lokaal: " + les.Lokaal,
                //Attendees = attendeeList,
                Location = les.Lokaal
            };

            try
            {
                Event returnobj = _service.Events.Insert(event1, Repository._agendaId).Execute();
                //do update on lessen table and add guid to les
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public bool UpdateEvent(Les les)
        {
            try
            {
                
                if (_agenda.Items.Any(x => x.Id.Equals(les.Guid)))
                {
                    var item = _agenda.Items.FirstOrDefault(x => x.Id.Equals(les.Guid));
                    item.Summary = les.Vak;
                    item.Description = "Vak: " + les.Vak + "\r" + "Vak code: " + les.VakCode + "\r" + "Docent: " + les.Docent + "\r" + "Lokaal: " + les.Lokaal;
                    item.Location = les.Lokaal;
                    item.Start = new EventDateTime() { DateTime = les.StartTijd };
                    item.End = new EventDateTime() { DateTime = les.StartTijd.Add(les.Lengte) };

                    _service.Events.Update(item, Repository._agendaId, les.Guid).Execute();
                    return true;
                }
                else
                {
                    return AddEvent(les);
                }              
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public bool DeleteEvent(string guid)
        {
            try
            {
                _service.Events.Delete(Repository._agendaId, guid).Execute();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
    }
}
