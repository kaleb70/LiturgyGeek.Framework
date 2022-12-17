using LiturgyGeek.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class ChurchOccasion : Clcs.Model.ChurchOccasion, ICloneable<ChurchOccasion>
    {
        [JsonConstructor]
        public ChurchOccasion(string name)
            : base(name)
        {
        }

        public ChurchOccasion(string name, string? shortName)
            : base(name)
        {
            ShortName = shortName;
        }

        public ChurchOccasion(ChurchOccasion other)
            : this(other.Name, other.ShortName)
        {
        }

        public ChurchOccasion Clone() => new ChurchOccasion(this);

        object ICloneable.Clone() => Clone();
    }
}
