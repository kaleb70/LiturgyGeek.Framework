using LiturgyGeek.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class ChurchOccasion : ICloneable<ChurchOccasion>
    {
        public string Name { get; set; }

        public string? ShortName { get; set; }

        [JsonConstructor]
        public ChurchOccasion(string name, string? shortName = null)
        {
            Name = name;
            ShortName = shortName;
        }

        public ChurchOccasion(ChurchOccasion other)
        {
            this.Name = other.Name;
            ShortName = other.ShortName;
        }

        public ChurchOccasion Clone() => new ChurchOccasion(this);

        object ICloneable.Clone() => Clone();
    }
}
