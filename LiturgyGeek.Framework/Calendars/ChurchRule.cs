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
        public List<ChurchDate> _VisibilityIncludeDates = new List<ChurchDate>();

        public List<ChurchDate> _VisibilityExcludeDates = new List<ChurchDate>();

        [JsonConstructor]
        public ChurchRule(string summary)
            : base(summary)
        {
        }

        public ChurchRule(ChurchRule other)
            : this(other.Summary)
        {
            _VisibilityIncludeDates.AddRange(other._VisibilityIncludeDates);
            _VisibilityExcludeDates.AddRange(other._VisibilityExcludeDates);
        }

        public ChurchRule Clone() => new ChurchRule(this);

        object ICloneable.Clone() => Clone();
    }
}
