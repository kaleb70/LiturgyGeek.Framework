using LiturgyGeek.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class ChurchEvent
    {
        public string? OccasionCode { get; set; }

        public ChurchDate[] Dates { get; set; }

        public string? Name { get; set; }

        public string? ShortName { get; set; }

        [JsonConstructor]
        internal ChurchEvent(string? occasionCode, ChurchDate[] dates, string? name, string? shortName)
        {
            if (occasionCode == null && name == null)
                throw new ArgumentNullException("Either an occasion code or a name must be provided.");

            OccasionCode = occasionCode;
            Dates = dates;
            Name = name;
            ShortName = shortName;
        }

        public static ChurchEvent ByOccasion(string occasionCode, string dates, string? name = default, string? shortName = default)
            => new ChurchEvent(occasionCode, ChurchDate.ParseCollection(dates), name, shortName);

        public static ChurchEvent ByName(string dates, string name, string? shortName = default)
            => new ChurchEvent(null, ChurchDate.ParseCollection(dates), name, shortName);
    }
}
