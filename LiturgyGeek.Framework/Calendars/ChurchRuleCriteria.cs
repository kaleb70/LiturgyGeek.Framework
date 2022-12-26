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
    public class ChurchRuleCriteria : Clcs.Model.ChurchRuleCriteria, ICloneable<ChurchRuleCriteria>
    {
        [JsonConstructor]
        public ChurchRuleCriteria(string ruleKey,
                                    ChurchDate? startDate,
                                    ChurchDate? endDate,
                                    IReadOnlyList<ChurchDate>? includeDates,
                                    IReadOnlyList<string>? includeRanks,
                                    IReadOnlyList<ChurchDate>? excludeDates)
            : base(ruleKey, startDate, endDate, includeDates, includeRanks, excludeDates)
        {
        }

        public ChurchRuleCriteria(ChurchRuleCriteria other)
            : this(other.RuleKey, other.StartDate, other.EndDate, other.IncludeDates, other.IncludeRanks, other.ExcludeDates)
        {
        }

        public virtual ChurchRuleCriteria Clone() => new ChurchRuleCriteria(this);

        object ICloneable.Clone() => Clone();
    }
}
