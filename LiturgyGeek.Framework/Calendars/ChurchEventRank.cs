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
        [JsonConstructor]
        public ChurchEventRank(int precedence, string rankCode)
            : base(precedence, rankCode)
        {
        }

        public ChurchEventRank(ChurchEventRank other)
            : base(other.Precedence, other.RankCode)
        {
            MonthViewHeadline = other.MonthViewHeadline;
            MonthViewContent = other.MonthViewContent;
        }

        public ChurchEventRank Clone() => new ChurchEventRank(this);

        object ICloneable.Clone() => Clone();
    }
}
