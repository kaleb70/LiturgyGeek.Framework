using LiturgyGeek.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class ChurchEventRank : ICloneable<ChurchEventRank>
    {
        public int Precedence { get; set; }

        public string RankCode { get; set; }

        [JsonConstructor]
        public ChurchEventRank(int precedence, string rankCode)
        {
            Precedence = precedence;
            RankCode = rankCode;
        }

        public ChurchEventRank(ChurchEventRank other)
        {
            Precedence = other.Precedence;
            RankCode = other.RankCode;
        }

        public ChurchEventRank Clone() => new ChurchEventRank(this);

        object ICloneable.Clone() => Clone();
    }
}
