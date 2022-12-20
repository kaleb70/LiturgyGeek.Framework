using LiturgyGeek.Framework.Clcs.Dates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Clcs.Model
{
    public class ChurchRuleCriteria
    {
        public string RuleKey { get; set; }

        public ChurchDate? StartDate { get; set; }

        public ChurchDate? EndDate { get; set; }

        public List<ChurchDate> IncludeDates { get; set; } = new List<ChurchDate>();

        public List<string> IncludeRanks { get; set; } = new List<string>();

        public List<ChurchDate> ExcludeDates { get; set; } = new List<ChurchDate>();

        [JsonConstructor]
        public ChurchRuleCriteria(string ruleKey)
        {
            RuleKey = ruleKey;
        }
    }
}
