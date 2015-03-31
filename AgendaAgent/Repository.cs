using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;

namespace AgendaAgent
{
    internal static class Repository
    {
        private static readonly string _serviceAccountEmail = "177600524325-6sbfabk2ldv0ickbpjtvfjqbieagg289@developer.gserviceaccount.com";
        private static readonly string _privateKeyPassword = "notasecret";
        internal static readonly string _agendaId = "roosterify@gmail.com";
        private static readonly string ApplicationName = "Roosterify";
        private static CalendarService _service;
        private static Events _agenda;

        private static CalendarService GoogleCalendar()
        {
            var certificate = new X509Certificate2(Properties.Resources.Roosterify_b75ac5863d29, _privateKeyPassword, X509KeyStorageFlags.Exportable);

            var credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(_serviceAccountEmail)
                {
                    Scopes = new[] { CalendarService.Scope.Calendar }
                }.FromCertificate(certificate));

            return new CalendarService(
                new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName
                });
        }

        public static bool GetData()
        {
            _service = GoogleCalendar();
            _agenda = _service.Events.List(_agendaId).Execute();
            return true;
        }

        public static Events GetAgenda()
        {
            return _agenda;
        }

        public static CalendarService GetCalendar()
        {
            return _service;
        }
    }
}
