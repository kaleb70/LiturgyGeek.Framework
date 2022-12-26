using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public struct ChurchRuleResult
    {
        public KeyValuePair<string, ChurchRuleGroup> RuleGroup { get; set; }

        public KeyValuePair<string, ChurchRule> Rule { get; set; }

        public bool Show { get; set; }
    }
}
