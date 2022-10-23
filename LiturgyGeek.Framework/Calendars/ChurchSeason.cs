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
        public string? OccasionCode { get; set; }

        public ChurchDate StartDate { get; set; }

        public ChurchDate EndDate { get; set; }

        public bool IsDefault { get; set; }

        public ChurchEvent[] Events { get; set; }

        [JsonConstructor]
        public ChurchSeason(string? occasionCode, ChurchDate startDate, ChurchDate endDate, bool isDefault = false, ChurchEvent[]? events = default)
        {
            OccasionCode = occasionCode;
            StartDate = startDate;
            EndDate = endDate;
            IsDefault = isDefault;
            Events = events ?? new ChurchEvent[0];
        }

        public ChurchSeason(ChurchDate startDate, ChurchDate endDate, bool isDefault = false, ChurchEvent[]? events = default)
            : this(default, startDate, endDate, isDefault, events)
        {
        }
    }
}
