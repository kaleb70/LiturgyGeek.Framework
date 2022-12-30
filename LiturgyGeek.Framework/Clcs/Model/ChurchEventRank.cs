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

        public List<string> CustomFlags { get; set; } = new List<string>();

        [JsonConstructor]
        public ChurchEventRank()
        {
        }
    }
}
