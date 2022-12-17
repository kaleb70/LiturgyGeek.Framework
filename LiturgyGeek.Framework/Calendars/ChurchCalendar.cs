using LiturgyGeek.Framework.Clcs.Enums;
using LiturgyGeek.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class ChurchCalendar : Clcs.Model.ChurchCalendar<ChurchEventRank, ChurchSeason, ChurchEvent>, ICloneable<ChurchCalendar>
    {
        [JsonConstructor]
        public ChurchCalendar(string name, string traditionCode)
            : base(name, traditionCode)
        {
        }

        public ChurchCalendar(string name, string traditionCode, CalendarReckoning solarReckoning, CalendarReckoning paschalReckoning)
            :  base(name, traditionCode)
        {
            SolarReckoning = solarReckoning;
            PaschalReckoning = paschalReckoning;
        }

        public ChurchCalendar(ChurchCalendar other)
            : base(other.Name, other.TraditionCode)
        {
            SolarReckoning = other.SolarReckoning;
            PaschalReckoning = other.PaschalReckoning;
            EventRanks = other.EventRanks.Clone();
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
