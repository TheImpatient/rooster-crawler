using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace RoosterCrawler
{
    public class Schedule
    {
        public Week ExternalWeek;
        public Week InternalWeek;
        public String klas;

        public Schedule(int weken, string _klas)
        {
            klas = _klas;

            InternalWeek = DataParser.GetInternalWeekSchedule(weken, klas);
            ExternalWeek = DataParser.GetExternalWeekSchedule(weken, klas);
        }

        /// <summary>
        /// compares internal and external schedules
        /// </summary>
        /// <returns>true = the compared schedules are equal. false = schedules are not equal</returns>
        public bool Compare()
        {
            //Als het externe rooster anders is dan ons interne rooster over schrijven we gewoon de hele week

            //Trim de weken eerst voordat hij compared kan worden
            Week InternalWeekTrimmed = InternalWeek.GetTrimmedWeek();
            Week ExternalWeekTrimmed = ExternalWeek.GetTrimmedWeek();

            return ExternalWeekTrimmed.Equals(InternalWeekTrimmed);
        }

        public UpdateResult Synchronize()
        {
            Week InternalWeekTrimmed = InternalWeek.GetTrimmedWeek();
            Week ExternalWeekTrimmed = ExternalWeek.GetTrimmedWeek();

            //string query = WeekToQuery(ExternalWeek, klas);

            //Remove comment to enable mutations
            var mutationList = ExternalWeekTrimmed.GetMutations(InternalWeekTrimmed);
            updateAgenda(mutationList, klas);
            string query = MutationToQuery(mutationList, klas);

            return DataParser.UpdateInternalData(query);
        }

        private void updateAgenda(List<LesMutation> list,string klas)
        {
            var query = new StringBuilder("INSERT INTO agenda_tasks (action, les) VALUES ");
            foreach (var lesMutation in list)
            {
                var les = new AgendaAgent.Les()
                {
                    Docent = lesMutation.MLes.Docent,
                    Klas = klas,
                    Lengte = new TimeSpan(0, 0, lesMutation.MLes.Lengte, 0),
                    Lokaal = lesMutation.MLes.Lokaal,
                    StartTijd = DateTime.Parse(lesMutation.MLes.StartTijd),
                    Vak = lesMutation.MLes.Vak,
                    VakCode = lesMutation.MLes.VakCode,
                    VakId = lesMutation.MLes.InternalId,
                    Guid = lesMutation.MLes.Guid
                };
                query.Append(String.Format(list.Last().Equals(lesMutation) ? "({0}, '{1}');" : "({0}, '{1}'),", lesMutation.Type, JsonConvert.SerializeObject(les)));
            }

            DataParser.UpdateInternalData(query.ToString());
        }

        private string MutationToQuery(List<LesMutation> _mutations, String _klas)
        {
            //GET LIST WITH MUTATIONS
            //INSERT LIST INTO AGENDA_MUTATIONS
            //APPLY MUTATIONS TO INTERNAL

            /* -create-
            INSERT INTO les
            (docent, vak, vak_code, vak_id, start_tijd, lengte, lokaal, klas)
            VALUES
            (docent, vak, vak_code, vak_id, start_tijd, lengte, lokaal, klas),
            (docent, vak, vak_code, vak_id, start_tijd, lengte, lokaal, klas),
            (docent, vak, vak_code, vak_id, start_tijd, lengte, lokaal, klas),
            (docent, vak, vak_code, vak_id, start_tijd, lengte, lokaal, klas);
             */

            List<LesMutation> createMutations = _mutations.Where(x => x.Type == LesMutation.Mutation.CREATE).ToList<LesMutation>();
            String createQuery = "";

            if(createMutations.Count > 0)
            {
                createQuery += "INSERT INTO les (docent, vak, vak_code, vak_id, start_tijd, lengte, lokaal, klas) VALUES";
                foreach (LesMutation m in createMutations)
                {
                    if (m.MLes.Docent != "" && m.MLes.Vak != "" && m.MLes.VakCode != "" && m.MLes.VakId != 0)
                    {
                        createQuery += "('" + escape(m.MLes.Docent) + "', '" + escape(m.MLes.Vak) + "', '" + escape(m.MLes.VakCode) + "', " + m.MLes.VakId + ", '" + m.MLes.StartTijd + "', '" + new TimeSpan(0, 0, m.MLes.Lengte, 0).ToString(@"hh\:mm\:ss") + "' , '" + escape(m.MLes.Lokaal) + "' , '" + escape(_klas) + "'),";
                    }
                }
                createQuery = createQuery.Remove(createQuery.Length - 1) + ";";
            }

            /* -update-
            UPDATE les
                SET 
                docent = CASE id
                    WHEN 11 THEN "Docentnaam1"
                    WHEN 12 THEN "Docentnaam2"
                    WHEN 13 THEN "Docentnaam3"
                END,
                vak = CASE id
                    WHEN 11 THEN 'Vaknaam1'
                    WHEN 12 THEN 'Vaknaam2'
                    WHEN 13 THEN 'Vaknaam3'
                END
            WHERE id IN (11,12,13);
            */
            //docent, vak, vak_code, vak_id, start_tijd, lengte, lokaal, klas

            List<LesMutation> updateMutations = _mutations.Where(x => x.Type == LesMutation.Mutation.UPDATE).ToList<LesMutation>();
            String updateQuery = "";

            if (updateMutations.Count > 0)
            {
                updateQuery += "UPDATE les SET ";

                updateQuery += "docent = CASE id ";
                foreach (LesMutation m in updateMutations)
                {
                    updateQuery += "WHEN " + m.MLes.InternalId + " THEN '" + escape(m.MLes.Docent) + "' ";
                }
                updateQuery += "END, ";

                updateQuery += "vak = CASE id ";
                foreach (LesMutation m in updateMutations)
                {
                    updateQuery += "WHEN " + m.MLes.InternalId + " THEN '" + escape(m.MLes.Vak) + "' ";
                }
                updateQuery += "END, ";

                updateQuery += "vak_code = CASE id ";
                foreach (LesMutation m in updateMutations)
                {
                    updateQuery += "WHEN " + m.MLes.InternalId + " THEN '" + escape(m.MLes.VakCode) + "' ";
                }
                updateQuery += "END, ";

                updateQuery += "vak_id = CASE id ";
                foreach (LesMutation m in updateMutations)
                {
                    updateQuery += "WHEN " + m.MLes.InternalId + " THEN '" + m.MLes.VakId + "' ";
                }
                updateQuery += "END, ";

                updateQuery += "start_tijd = CASE id ";
                foreach (LesMutation m in updateMutations)
                {
                    updateQuery += "WHEN " + m.MLes.InternalId + " THEN '" + m.MLes.StartTijd + "' ";
                }
                updateQuery += "END, ";

                updateQuery += "lengte = CASE id ";
                foreach (LesMutation m in updateMutations)
                {
                    updateQuery += "WHEN " + m.MLes.InternalId + " THEN '" + new TimeSpan(0, 0, m.MLes.Lengte, 0).ToString(@"hh\:mm\:ss") + "' ";
                }
                updateQuery += "END, ";

                updateQuery += "lokaal = CASE id ";
                foreach (LesMutation m in updateMutations)
                {
                    updateQuery += "WHEN " + m.MLes.InternalId + " THEN '" + escape(m.MLes.Lokaal) + "' ";
                }
                updateQuery += "END, ";

                updateQuery += "klas = CASE id ";
                foreach (LesMutation m in updateMutations)
                {
                    updateQuery += "WHEN " + m.MLes.InternalId + " THEN '" + escape(_klas) + "' ";
                }

                updateQuery += "END WHERE id IN (";
                foreach (LesMutation m in updateMutations)
                {
                    updateQuery += (m.MLes.InternalId + ",");
                }
                updateQuery = updateQuery.Remove(updateQuery.Length - 1) + ");";
            }

            /* -delete-
            DELETE FROM les WHERE id IN (1,2,3,4,5,6,7,8,9,10);
            */

            List<LesMutation> deleteMutations = _mutations.Where(x => x.Type == LesMutation.Mutation.DELETE).ToList<LesMutation>();
            String deleteQuery = "";

            if (deleteMutations.Count > 0)
            {
                deleteQuery += "DELETE FROM les WHERE id IN (";
                foreach (LesMutation m in deleteMutations)
                {
                    deleteQuery += (m.MLes.InternalId + ",");
                }
                deleteQuery = deleteQuery.Remove(deleteQuery.Length - 1) + ");";
            }

            return createQuery + updateQuery + deleteQuery;
        }

        private string WeekToQuery(Week week, String _klas)
        {
            /*
             INSERT INTO les
            (docent, vak, vak_code, vak_id, start_tijd, lengte, lokaal, klas)
            VALUES
            (docent, vak, vak_code, vak_id, start_tijd, lengte, lokaal, klas),
            (docent, vak, vak_code, vak_id, start_tijd, lengte, lokaal, klas),
            (docent, vak, vak_code, vak_id, start_tijd, lengte, lokaal, klas),
            (docent, vak, vak_code, vak_id, start_tijd, lengte, lokaal, klas);
             */

            String query = "INSERT INTO les (docent, vak, vak_code, vak_id, start_tijd, lengte, lokaal, klas) VALUES";

            int dayIndex = 0;
            int lesIndex = 0;

            foreach (Day d in week.days)
            {
                foreach (Les l in d.lessen)
                {

                    if (l.Docent != "" && l.Vak != "" && l.VakCode != "" && l.VakId != 0)
                    {

                        query += "('" + escape(l.Docent) + "', '" + escape(l.Vak) + "', '" + escape(l.VakCode) + "', " + l.VakId + ", '" + Util.FirstDateOfWeek(2015, week.WeekNummer, new TimeSpan(dayIndex, 0, Util.schoolHours[lesIndex % 15], 0)) + "', '" + new TimeSpan(0, 0, l.Lengte, 0).ToString(@"hh\:mm\:ss") + "' , '" + escape(l.Lokaal) + "' , '" + escape(_klas) + "'),";
                    }

                    lesIndex++;
                }
                dayIndex++;
            }
            query = query.Remove(query.Length - 1) + ";";
            //query = String.Format(query);
            return query;
            //return MySqlHelper.EscapeString(query);
            //return Regex.Replace(query, @"([^a-zA-Z0-9_]|^\s)", string.Empty);
        }

        private String escape(string str)
        {
            return Regex.Replace(str, @"([\'""\\/])", @"\$0");
        }
    }
}
