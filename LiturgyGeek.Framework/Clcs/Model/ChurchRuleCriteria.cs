using LiturgyGeek.Framework.Clcs.Dates;
using LiturgyGeek.Framework.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Clcs.Model
{
    public class ChurchRuleCriteria
    {
        public string RuleKey { get; private init; }

        public ChurchDate? StartDate { get; private init; }

        public ChurchDate? EndDate { get; private init; }

        public IReadOnlyList<ChurchDate> IncludeDates { get; private init; }

        public IReadOnlyList<string> IncludeRanks { get; private init; }

        public IReadOnlyList<ChurchDate> ExcludeDates { get; private init; }

        [JsonConstructor]
        public ChurchRuleCriteria(string ruleKey,
                                    ChurchDate? startDate,
                                    ChurchDate? endDate,
                                    IReadOnlyList<ChurchDate>? includeDates,
                                    IReadOnlyList<string>? includeRanks,
                                    IReadOnlyList<ChurchDate>? excludeDates)
        {
            RuleKey = ruleKey;
            StartDate = startDate;
            EndDate = endDate;
            IncludeDates = includeDates ?? new List<ChurchDate>();
            IncludeRanks = includeRanks ?? new List<string>();
            ExcludeDates = excludeDates ?? new List<ChurchDate>();
        }
    }
}
