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

        internal ChurchEvent(string? occasionCode, IEnumerable<ChurchDate> dates, string? name, string? longName, string? eventRankKey)
            : base(occasionCode, name)
        {
            Dates = dates.ToList();
            LongName = longName;
            EventRankKey = eventRankKey;
        }

        public ChurchEvent(ChurchEvent other)
            : base(other.OccasionCode, other.Name)
        {
            Dates = other.Dates.ToList();
            LongName = other.LongName;
            EventRankKey = other.EventRankKey;
        }

        public static ChurchEvent ByOccasion(string occasionCode, string dates, string? name = default, string? shortName = default, string? eventRankKey = default)
            => new ChurchEvent(occasionCode, ChurchDate.ParseCollection(dates), name, shortName, eventRankKey);

        public static ChurchEvent ByName(string dates, string name, string? shortName = default, string? eventRankKey = default)
            => new ChurchEvent(null, ChurchDate.ParseCollection(dates), name, shortName, eventRankKey);

        public ChurchEvent Clone() => new ChurchEvent(this);

        object ICloneable.Clone() => Clone();

        public void Resolve(IChurchCalendarProvider provider)
        {
            ChurchOccasion? occasion = default;
            if (OccasionCode != null && (Name == null || LongName == null))
                provider.GetCommon().Occasions.TryGetValue(OccasionCode, out occasion);

            Name ??= occasion!.Name;
            LongName ??= occasion?.LongName ?? Name;
        }
    }
}
