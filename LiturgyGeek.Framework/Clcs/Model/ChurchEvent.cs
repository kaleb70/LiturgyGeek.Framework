using LiturgyGeek.Framework.Clcs.Dates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Clcs.Model
{
    public class ChurchEvent<TRuleCriteria>
        where TRuleCriteria : ChurchRuleCriteria
    {
        public string? OccasionKey { get; set; }

        public List<ChurchDate> Dates { get; set; } = new List<ChurchDate>();

        public string? Name { get; set; }

        public string? LongName { get; set; }

        public string? EventRankKey { get; set; }

        public HashSet<string> CustomFlags { get; set; } = new HashSet<string>();

        public Dictionary<string, TRuleCriteria[]> RuleCriteria { get; set; } = new Dictionary<string, TRuleCriteria[]>();

        [JsonConstructor]
        public ChurchEvent(string? occasionKey, string? name)
        {
            if (occasionKey == null && name == null)
                throw new ArgumentNullException(null, "Either an occasion code or a name must be provided.");

            OccasionKey = occasionKey;
            Name = name;
        }
    }

    public class ChurchEvent : ChurchEvent<ChurchRuleCriteria>
    {
        [JsonConstructor]
        public ChurchEvent(string? occasionKey, string? name)
            : base(occasionKey, name)
        {
        }
    }
}
