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
        public ChurchRuleCriteria(string ruleKey)
            : base(ruleKey)
        {
        }

        public ChurchRuleCriteria(ChurchRuleCriteria other)
            : base(other.RuleKey)
        {
            StartDate = other.StartDate;
            EndDate = other.EndDate;
            IncludeDates.AddRange(other.IncludeDates);
            IncludeRanks.AddRange(other.IncludeRanks);
            ExcludeDates.AddRange(other.ExcludeDates);
        }

        public ChurchRuleCriteria Clone() => new ChurchRuleCriteria(this);

        object ICloneable.Clone() => Clone();
    }
}
