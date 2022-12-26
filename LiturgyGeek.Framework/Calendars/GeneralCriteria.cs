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
    public class GeneralCriteria : ChurchRuleCriteria
    {
        private new string RuleKey { get; init; } = "";

        [JsonConstructor]
        public GeneralCriteria(ChurchDate? startDate, ChurchDate? endDate,
                                IReadOnlyList<ChurchDate>? includeDates,
                                IReadOnlyList<string>? includeRanks,
                                IReadOnlyList<ChurchDate>? excludeDates)
            : base("", startDate, endDate, includeDates, includeRanks, excludeDates)
        {
        }

        public GeneralCriteria(GeneralCriteria other)
            : this(other.StartDate, other.EndDate, other.IncludeDates, other.IncludeRanks, other.ExcludeDates)
        {
        }

        public override GeneralCriteria Clone() => new GeneralCriteria(this);
    }
}
