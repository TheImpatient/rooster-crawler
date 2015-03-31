using System;
using System.Collections.Generic;
using System.Linq;
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

        public Boolean AddEvent(Les les)
        {
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
                Summary = les.Vak,
                Start = startTime,
                End = endTime,
                Description = "Vak: " + les.Vak + "\r" + "Vak code: " + les.VakCode + "\r" + "Docent: " + les.Docent + "\r" + "Lokaal: " + les.Lokaal,
                //Attendees = attendeeList,
                Location = les.Lokaal
            };

            try
            {
                Event returnobj = _service.Events.Insert(event1, Repository._agendaId).Execute();

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }

            return false;
        } 


    }
}
