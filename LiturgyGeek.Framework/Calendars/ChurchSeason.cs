using LiturgyGeek.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class ChurchSeason : ICloneable<ChurchSeason>
    {
        public string? OccasionCode { get; set; }

        public ChurchDate StartDate { get; set; }

        public ChurchDate EndDate { get; set; }

        public bool IsDefault { get; set; }

        //public List<ChurchEvent> Events { get; set; }

        [JsonConstructor]
        public ChurchSeason(string? occasionCode, ChurchDate startDate, ChurchDate endDate, bool isDefault = false, IEnumerable<ChurchEvent>? events = default)
        {
            OccasionCode = occasionCode;
            StartDate = startDate;
            EndDate = endDate;
            IsDefault = isDefault;
        }

        public ChurchSeason(ChurchDate startDate, ChurchDate endDate, bool isDefault = false, IEnumerable<ChurchEvent>? events = default)
            : this(default, startDate, endDate, isDefault, events)
        {
        }

        public ChurchSeason(ChurchSeason other)
        {
            OccasionCode = other.OccasionCode;
            StartDate = other.StartDate;
            EndDate = other.EndDate;
            IsDefault = other.IsDefault;
        }

        public ChurchSeason Clone() => new ChurchSeason(this);

        object ICloneable.Clone() => Clone();
    }
}
