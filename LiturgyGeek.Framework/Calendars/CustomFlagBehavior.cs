using LiturgyGeek.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class CustomFlagBehavior : Clcs.Model.CustomFlagBehavior, ICloneable<CustomFlagBehavior>
    {
        public bool? _MonthViewHeadline { get; set; }

        public bool? _MonthViewContent { get; set; }

        public CustomFlagBehavior Clone() => new CustomFlagBehavior
        {
            _MonthViewHeadline = _MonthViewHeadline,
            _MonthViewContent = _MonthViewContent,
        };

        object ICloneable.Clone() => Clone();
    }
}
