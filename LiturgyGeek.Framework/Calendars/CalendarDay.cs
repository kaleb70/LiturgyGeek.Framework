using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class CalendarDay
    {
        public DateTime Date { get; set; }

        public ChurchSeason Season { get; set; }

        public List<ChurchEvent> Events { get; set; } = new List<ChurchEvent>();

        public CalendarDay(DateTime date, ChurchSeason season)
        {
            Date = date;
            Season = season;
        }
    }
}
