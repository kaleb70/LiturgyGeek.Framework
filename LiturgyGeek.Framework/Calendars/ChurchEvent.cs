using LiturgyGeek.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class ChurchEvent : ICloneable<ChurchEvent>
    {
        public string? OccasionCode { get; set; }

        public List<ChurchDate> Dates { get; set; }

        public string? Name { get; set; }

        public string? ShortName { get; set; }

        public string? RankCode { get; set; }

        [JsonConstructor]
        internal ChurchEvent(string? occasionCode, IEnumerable<ChurchDate> dates, string? name, string? shortName, string? rankCode)
        {
            if (occasionCode == null && name == null)
                throw new ArgumentNullException("Either an occasion code or a name must be provided.");

            OccasionCode = occasionCode;
            Dates = dates.ToList();
            Name = name;
            ShortName = shortName;
            RankCode = rankCode;
        }

        public ChurchEvent(ChurchEvent Other)
        {
            OccasionCode = Other.OccasionCode;
            Dates = Other.Dates.ToList();
            Name = Other.Name;
            ShortName = Other.ShortName;
            RankCode = Other.RankCode;
        }

        public static ChurchEvent ByOccasion(string occasionCode, string dates, string? name = default, string? shortName = default, string? rankCode = default)
            => new ChurchEvent(occasionCode, ChurchDate.ParseCollection(dates), name, shortName, rankCode);

        public static ChurchEvent ByName(string dates, string name, string? shortName = default, string? rankCode = default)
            => new ChurchEvent(null, ChurchDate.ParseCollection(dates), name, shortName, rankCode);

        public ChurchEvent Clone() => new ChurchEvent(this);

        object ICloneable.Clone() => Clone();

        public void Resolve(IChurchCalendarProvider provider)
        {
            ChurchOccasion? occasion = default;
            if (OccasionCode != null && (Name == null || ShortName == null))
                provider.GetCommon().Occasions.TryGetValue(OccasionCode, out occasion);

            Name ??= occasion!.Name;
            ShortName ??= occasion?.ShortName ?? Name;
        }
    }
}
