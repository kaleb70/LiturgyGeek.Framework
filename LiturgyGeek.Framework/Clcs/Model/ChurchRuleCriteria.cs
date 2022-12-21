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

        private readonly int criteriaHashCode;
        private readonly int fullHashCode;

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

            unchecked
            {
                criteriaHashCode = 17;
                if (StartDate != null) criteriaHashCode = criteriaHashCode * 23 + StartDate.GetHashCode();
                if (EndDate != null) criteriaHashCode = criteriaHashCode * 23 + EndDate.GetHashCode();
                foreach (var d in IncludeDates)
                    criteriaHashCode = criteriaHashCode * 23 + d.GetHashCode();
                foreach (var r in IncludeRanks)
                    criteriaHashCode = criteriaHashCode * 23 + r.GetHashCode();
                foreach (var d in ExcludeDates)
                    criteriaHashCode = criteriaHashCode * 23 + d.GetHashCode();
                
                fullHashCode = criteriaHashCode * 23 + RuleKey.GetHashCode();
            }
        }

        public override int GetHashCode() => fullHashCode;

        public bool Equals(ChurchRuleCriteria? other)
        {
            return other != null
                    && fullHashCode == other.fullHashCode
                    && RuleKey == other.RuleKey
                    && CriteriaComparer.Equals(this, other);
        }

        public int GetCriteriaHashCode() => criteriaHashCode;

        public bool CriteriaEquals(ChurchRuleCriteria? y)
        {
            return GetType() == y?.GetType()
                    && StartDate == y.StartDate
                    && EndDate == y.EndDate
                    && (IncludeDates == y.IncludeDates || (IncludeDates?.SequenceEqual(y.IncludeDates) ?? false))
                    && (IncludeRanks == y.IncludeRanks || (IncludeRanks?.SequenceEqual(y.IncludeRanks) ?? false))
                    && (ExcludeDates == y.ExcludeDates || (ExcludeDates?.SequenceEqual(y.ExcludeDates) ?? false));
        }

        public override bool Equals(object? obj) => Equals(obj as ChurchRuleCriteria);

        public static IEqualityComparer<ChurchRuleCriteria> CriteriaComparer { get; }
            = new AnonymousEqualityComparer<ChurchRuleCriteria>(
                (x, y) => x == null ? y == null : x.Equals(y),
                obj => obj.GetCriteriaHashCode());
    }
}
