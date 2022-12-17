﻿using LiturgyGeek.Framework.Clcs.Dates;
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
        public ChurchEvent(string? occasionKey, string? name)
            : base(occasionKey, name)
        {
        }

        internal ChurchEvent(string? occasionKey, IEnumerable<ChurchDate> dates, string? name, string? longName, string? eventRankKey)
            : base(occasionKey, name)
        {
            Dates = dates.ToList();
            LongName = longName;
            EventRankKey = eventRankKey;
        }

        public ChurchEvent(ChurchEvent other)
            : base(other.OccasionKey, other.Name)
        {
            Dates = other.Dates.ToList();
            LongName = other.LongName;
            EventRankKey = other.EventRankKey;
        }

        public static ChurchEvent ByOccasion(string occasionKey, string dates, string? name = default, string? shortName = default, string? eventRankKey = default)
            => new ChurchEvent(occasionKey, ChurchDate.ParseCollection(dates), name, shortName, eventRankKey);

        public static ChurchEvent ByName(string dates, string name, string? shortName = default, string? eventRankKey = default)
            => new ChurchEvent(null, ChurchDate.ParseCollection(dates), name, shortName, eventRankKey);

        public ChurchEvent Clone() => new ChurchEvent(this);

        object ICloneable.Clone() => Clone();

        public void Resolve(IChurchCalendarProvider provider)
        {
            ChurchOccasion? occasion = default;
            if (OccasionKey != null && (Name == null || LongName == null))
                provider.GetCommon().Occasions.TryGetValue(OccasionKey, out occasion);

            Name ??= occasion!.Name;
            LongName ??= occasion?.LongName ?? Name;
        }
    }
}
