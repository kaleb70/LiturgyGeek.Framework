using LiturgyGeek.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class ChurchSeason
    {
        public string OccasionCode { get; set; }

        public ChurchDate StartDate { get; set; }

        public ChurchDate EndDate { get; set; }

        public ChurchEvent[] Events { get; set; }

        public string? Name { get; set; }

        public string? ShortName { get; set; }

        public ChurchSeason(string occasionCode, ChurchDate startDate, ChurchDate endDate, ChurchEvent[] events, string? name = default, string? shortName = default)
        {
            OccasionCode = occasionCode;
            StartDate = startDate;
            EndDate = endDate;
            Events = events;
            Name = name;
            ShortName = shortName;
        }
    }
}
