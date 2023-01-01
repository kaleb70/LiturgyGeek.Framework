using LiturgyGeek.Framework.Clcs.Dates;
using LiturgyGeek.Framework.Core;
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

        public string? _
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
        public ChurchEvent(string? occasionKey, string? name, List<ChurchDate>? dates, HashSet<string>? customFlags, string? _)
        {
            if (occasionKey == null && _ == null && name == null)
                throw new JsonException("Either \"occasionKey\" or \"name\" or \"_\" must be provided");

            if (occasionKey != null && _ != null)
                throw new JsonException("Conflicting properties \"occasionKey\" and \"_\"");

            if ((dates != null) == (_ != null))
                throw new JsonException("Either \"dates\" or \"_\" must be specified, but not both");

            if (customFlags != null && _ != null)
                throw new JsonException("Conflicting properties \"customFlags\" and \"_\"");

            if (_ != null)
            {
                var parts = _.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                    throw new JsonException("_ property must be in the form \"occasionKey date customFlag\" (customFlag is optional)");

                OccasionKey = parts[0];

                if (!ChurchDate.TryParse(parts[1], out var date))
                    throw new JsonException("date part of _ property is invalid");
                Dates.Add(date);

                CustomFlags.AddRange(parts.Skip(2));
            }
            else
            {
                OccasionKey = occasionKey;

                if (dates != null)
                    Dates = dates;

                if (customFlags != null)
                    CustomFlags = customFlags;
            }
            Name = name;
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
