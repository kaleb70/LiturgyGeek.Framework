using LiturgyGeek.Framework.Clcs.Dates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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

        [JsonPropertyName("o")]
        public string? _o
        {
            get => null;
            set => throw new NotImplementedException();
        }

        [JsonPropertyName("d")]
        public ChurchDate? _d
        {
            get => null;
            set => throw new NotImplementedException();
        }

        [JsonPropertyName("f")]
        public string? _f
        {
            get => null;
            set => throw new NotImplementedException();
        }

        public ChurchEvent(string? occasionKey, string? name)
        {
            if (occasionKey == null && name == null)
                throw new ArgumentNullException(null, "Either an occasion code or a name must be provided.");

            OccasionKey = occasionKey;
            Name = name;
        }

        [JsonConstructor]
        public ChurchEvent(string? occasionKey, string? name, List<ChurchDate>? dates, HashSet<string>? customFlags,
                            string? _o, ChurchDate? _d, string? _f)
        {
            if (occasionKey == null && _o == null && name == null)
                throw new JsonException("Either \"occasionKey\" or \"o\" or \"name\" must be provided");

            if (occasionKey != null && _o != null)
                throw new JsonException("Conflicting properties \"occasionKey\" and \"o\"");

            if ((dates != null) == (_d != null))
                throw new JsonException("Either \"dates\" or \"d\" must be specified, but not both");

            if (customFlags != null && _f != null)
                throw new JsonException("Conflicting properties \"customFlags\" and \"f\"");

            OccasionKey = occasionKey ?? _o;
            Name = name;

            if (dates != null)
                Dates = dates;
            else
                Dates.Add(_d!);

            if (customFlags != null)
                CustomFlags = customFlags;

            if (_f != null)
                CustomFlags.Add(_f);
        }

        [JsonConstructor]
        public ChurchEvent(string o, ChurchDate d, string? f)
        {
            OccasionKey = o;
            Dates.Add(d);
            if (f != null)
                CustomFlags.Add(f);
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
