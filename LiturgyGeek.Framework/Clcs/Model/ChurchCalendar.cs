using LiturgyGeek.Framework.Clcs.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Clcs.Model
{
    public class ChurchCalendar<TEventRank, TSeason, TEvent>
        where TEventRank : ChurchEventRank
        where TSeason : ChurchSeason
        where TEvent : ChurchEvent
    {
        public string Name { get; set; }

        public string TraditionCode { get; set; }

        public CalendarReckoning SolarReckoning { get; set; }

        public CalendarReckoning PaschalReckoning { get; set; }

        public List<TEventRank> EventRanks { get; set; } = new List<TEventRank>();

        public List<TSeason> Seasons { get; set; } = new List<TSeason>();

        public List<TEvent> Events { get; set; } = new List<TEvent>();

        public ChurchCalendar(string name, string traditionCode)
        {
            Name = name;
            TraditionCode = traditionCode;
        }
    }

    public class ChurchCalendar : ChurchCalendar<ChurchEventRank, ChurchSeason, ChurchEvent>
    {
        [JsonConstructor]
        public ChurchCalendar(string name, string traditionCode)
            : base(name, traditionCode)
        {
        }
    }
}
