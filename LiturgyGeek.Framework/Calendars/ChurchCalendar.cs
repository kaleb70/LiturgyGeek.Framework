using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class ChurchCalendar
    {
        public ChurchSeason[] Seasons { get;set; }

        public ChurchEvent[] Events { get; set; }

        public ChurchCalendar()
        {
            Seasons = new ChurchSeason[0];
            Events = new ChurchEvent[0];
        }

        public ChurchCalendar(ChurchSeason[] seasons, ChurchEvent[] events)
        {
            Seasons = seasons;
            Events = events;
        }
    }
}
