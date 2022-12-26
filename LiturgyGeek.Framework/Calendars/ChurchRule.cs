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
    public class ChurchRule : Clcs.Model.ChurchRule, ICloneable<ChurchRule>
    {
        public GeneralCriteria? _VisibilityCriteria { get; set; }

        [JsonConstructor]
        public ChurchRule(string summary)
            : base(summary)
        {
        }

        public ChurchRule(ChurchRule other)
            : this(other.Summary)
        {
            _VisibilityCriteria = other._VisibilityCriteria?.Clone();
        }

        public ChurchRule Clone() => new ChurchRule(this);

        object ICloneable.Clone() => Clone();
    }
}
