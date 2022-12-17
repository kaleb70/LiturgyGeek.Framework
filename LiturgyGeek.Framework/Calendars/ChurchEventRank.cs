using LiturgyGeek.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class ChurchEventRank : Clcs.Model.ChurchEventRank, ICloneable<ChurchEventRank>
    {
        public bool _MonthViewHeadline { get; set; }

        public bool _MonthViewContent { get; set; }

        [JsonConstructor]
        public ChurchEventRank()
        {
        }

        public ChurchEventRank(ChurchEventRank other)
        {
            Precedence = other.Precedence;
            _MonthViewHeadline = other._MonthViewHeadline;
            _MonthViewContent = other._MonthViewContent;
        }

        public ChurchEventRank Clone() => new ChurchEventRank(this);

        object ICloneable.Clone() => Clone();
    }
}
