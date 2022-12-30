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
    public class ChurchEvent : Clcs.Model.ChurchEvent<ChurchRuleCriteria>, ICloneable<ChurchEvent>
    {
        public bool? _MonthViewHeadline { get; set; }

        public bool? _MonthViewContent { get; set; }

        [JsonConstructor]
        public ChurchEvent(string? occasionKey, string? name)
            : base(occasionKey, name)
        {
        }

        internal ChurchEvent(string? occasionKey, IEnumerable<ChurchDate> dates, string? name, string? longName, string? eventRankKey)
            : base(occasionKey, name)
        {
            Dates.AddRange(dates);
            LongName = longName;
            EventRankKey = eventRankKey;
        }

        public ChurchEvent(ChurchEvent other)
            : base(other.OccasionKey, other.Name)
        {
            Dates.AddRange(other.Dates);
            LongName = other.LongName;
            EventRankKey = other.EventRankKey;
            CustomFlags.AddRange(other.CustomFlags);
            RuleCriteria = other.RuleCriteria.Clone();
            _MonthViewHeadline = other._MonthViewHeadline;
            _MonthViewContent = other._MonthViewContent;
        }

        public static ChurchEvent ByOccasion(string occasionKey, string dates, string? name = default, string? shortName = default, string? eventRankKey = default)
            => new ChurchEvent(occasionKey, ChurchDate.ParseCollection(dates), name, shortName, eventRankKey);

        public static ChurchEvent ByName(string dates, string name, string? shortName = default, string? eventRankKey = default)
            => new ChurchEvent(null, ChurchDate.ParseCollection(dates), name, shortName, eventRankKey);

        public ChurchEvent Clone() => new ChurchEvent(this);

        object ICloneable.Clone() => Clone();

        public void Merge(ChurchCalendar calendar, IChurchCalendarProvider provider)
        {
            ChurchOccasion? occasion = default;
            if (OccasionKey != null && (Name == null || LongName == null))
                provider.GetCommon().Occasions.TryGetValue(OccasionKey, out occasion);

            ChurchEventRank? eventRank;
            if (EventRankKey == null || !calendar.EventRanks.TryGetValue(EventRankKey, out eventRank))
                eventRank = null;

            if (eventRank != null)
                CustomFlags.AddRange(eventRank.CustomFlags);

            foreach (var customFlag in CustomFlags.AsEnumerable())
            {
                if (calendar.CustomFlagBehaviors.TryGetValue(customFlag, out var behavior))
                {
                    _MonthViewHeadline ??= behavior._MonthViewHeadline;
                    _MonthViewContent ??= behavior._MonthViewContent;
                }
            }

            if (_MonthViewHeadline.HasValue || _MonthViewContent.HasValue || eventRank == null)
            {
                _MonthViewHeadline ??= false;
                _MonthViewContent ??= false;
            }
            else
            {
                _MonthViewHeadline = eventRank._MonthViewHeadline;
                _MonthViewContent = eventRank._MonthViewContent;
            }
            Name ??= occasion?.Name ?? "";
            LongName ??= occasion?.LongName ?? Name;
        }
    }
}
