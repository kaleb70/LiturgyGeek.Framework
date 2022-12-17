using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Clcs.Model
{
    public class ChurchCommon<TOccasion> where TOccasion : ChurchOccasion
    {
        public Dictionary<string, TOccasion> Occasions { get; init; } = new Dictionary<string, TOccasion>();

        public ChurchCommon()
        {
        }
    }

    public class ChurchCommon : ChurchCommon<ChurchOccasion>
    {
        public ChurchCommon()
        {
        }
    }
}
