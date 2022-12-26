using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class CalendarDayResult
    {
        public DateTime Date { get; set; }

        public ChurchSeason Season { get; set; }

        public ChurchRuleResult[] Rules { get; set; }

        public ChurchEvent[] Events { get; set; }

        public CalendarDayResult(DateTime date, ChurchSeason season, ChurchRuleResult[] rules, ChurchEvent[] events)
        {
            Date = date;
            Season = season;
            Rules = rules;
            Events = events;
        }
    }
}
