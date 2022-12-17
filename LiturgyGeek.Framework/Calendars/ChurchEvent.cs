using LiturgyGeek.Framework.Clcs.Dates;
using LiturgyGeek.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class ChurchEvent : Clcs.Model.ChurchEvent, ICloneable<ChurchEvent>
    {
        [JsonConstructor]
        public ChurchEvent(string? occasionCode, string? name)
            : base(occasionCode, name)
        {
        }

        internal ChurchEvent(string? occasionCode, IEnumerable<ChurchDate> dates, string? name, string? shortName, string? rankCode)
            : base(occasionCode, name)
        {
            Dates = dates.ToList();
            ShortName = shortName;
            RankCode = rankCode;
        }

        public ChurchEvent(ChurchEvent other)
            : base(other.OccasionCode, other.Name)
        {
            Dates = other.Dates.ToList();
            ShortName = other.ShortName;
            RankCode = other.RankCode;
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
