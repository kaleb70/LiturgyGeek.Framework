using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Clcs.Model
{
    public class ChurchEventRank
    {
        public int Precedence { get; set; }

        public HashSet<string> CustomFlags { get; set; } = new HashSet<string>();

        [JsonConstructor]
        public ChurchEventRank()
        {
        }
    }
}
