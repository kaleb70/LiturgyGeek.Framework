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
        public string? OccasionCode { get; set; }

        public List<ChurchDate> Dates { get; set; } = new List<ChurchDate>();

        public string? Name { get; set; }

        public string? ShortName { get; set; }

        public string? RankCode { get; set; }

        [JsonConstructor]
        public ChurchEvent(string? occasionCode, string? name)
        {
            if (occasionCode == null && name == null)
                throw new ArgumentNullException(null, "Either an occasion code or a name must be provided.");

            OccasionCode = occasionCode;
            Name = name;
        }
    }
}
