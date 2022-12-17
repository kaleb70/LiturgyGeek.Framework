using LiturgyGeek.Framework.Clcs.Dates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Clcs.Model
{
    public class ChurchEvent
    {
        public string? OccasionKey { get; set; }

        public List<ChurchDate> Dates { get; set; } = new List<ChurchDate>();

        public string? Name { get; set; }

        public string? LongName { get; set; }

        public string? EventRankKey { get; set; }

        [JsonConstructor]
        public ChurchEvent(string? occasionKey, string? name)
        {
            if (occasionKey == null && name == null)
                throw new ArgumentNullException(null, "Either an occasion code or a name must be provided.");

            OccasionKey = occasionKey;
            Name = name;
        }
    }
}
