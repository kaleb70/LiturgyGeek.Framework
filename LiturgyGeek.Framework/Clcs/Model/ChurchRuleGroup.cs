using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Clcs.Model
{
    public class ChurchRuleGroup<TChurchRule> where TChurchRule : ChurchRule
    {
        public Dictionary<string, TChurchRule> Rules { get; set; } = new Dictionary<string, TChurchRule>();
    }

    public class ChurchRuleGroup : ChurchRuleGroup<ChurchRule>
    {
    }
}
