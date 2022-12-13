using LiturgyGeek.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class ChurchCalendar : ICloneable<ChurchCalendar>
    {
        public string Name { get; set; }

        public CalendarReckoning SolarReckoning { get; set; }

        public CalendarReckoning PaschalReckoning { get; set; }

        public List<ChurchSeason> Seasons { get; set; } = new List<ChurchSeason>();

        public List<ChurchEvent> Events { get; set; } = new List<ChurchEvent>();

        public ChurchCalendar(string name, CalendarReckoning solarReckoning, CalendarReckoning paschalReckoning, IEnumerable<ChurchSeason> seasons, IEnumerable<ChurchEvent> events)
        {
            Name = name;
            SolarReckoning = solarReckoning;
            PaschalReckoning = paschalReckoning;
            Seasons = seasons.ToList();
            Events = events.ToList();
        }

        public ChurchCalendar(string name, CalendarReckoning solarReckoning, CalendarReckoning paschalReckoning)
        {
            Name = name;
            SolarReckoning = solarReckoning;
            PaschalReckoning = paschalReckoning;
        }

        public ChurchCalendar(ChurchCalendar other)
        {
            Name = other.Name;
            SolarReckoning = other.SolarReckoning;
            PaschalReckoning = other.PaschalReckoning;
            Seasons = other.Seasons.Clone();
            Events = other.Events.Clone();
        }

        public ChurchCalendar Clone() => new ChurchCalendar(this);

        object ICloneable.Clone() => Clone();

        public ChurchCalendar CloneAndResolve(IChurchCalendarProvider provider)
        {
            var result = Clone();

            foreach (var churchEvent in result.Events)
                churchEvent.Resolve(provider);

            return result;
        }
    }
}
