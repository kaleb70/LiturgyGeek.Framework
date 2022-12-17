using LiturgyGeek.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class ChurchCommon : Clcs.Model.ChurchCommon<ChurchOccasion>, ICloneable<ChurchCommon>
    {
        public ChurchCommon()
        {
        }

        public ChurchCommon(ChurchCommon other)
        {
            foreach (var occasion in other.Occasions)
                Occasions.Add(occasion.Key, occasion.Value.Clone());
        }

        public ChurchCommon Clone() => new ChurchCommon(this);

        object ICloneable.Clone() => Clone();
    }
}
