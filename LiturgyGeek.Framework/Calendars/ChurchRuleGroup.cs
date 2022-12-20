using LiturgyGeek.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class ChurchRuleGroup : Clcs.Model.ChurchRuleGroup<ChurchRule>, ICloneable<ChurchRuleGroup>
    {
        public bool _MonthViewHeadline { get; set; }

        public bool _MonthViewContent { get; set; }

        public ChurchRuleGroup()
        {
        }

        public ChurchRuleGroup(ChurchRuleGroup other)
        {
            Rules = other.Rules.Clone();
            _MonthViewHeadline = other._MonthViewHeadline;
            _MonthViewContent = other._MonthViewContent;
        }

        public ChurchRuleGroup Clone() => new ChurchRuleGroup(this);

        object ICloneable.Clone() => Clone();
    }
}
