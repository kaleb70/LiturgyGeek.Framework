using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class ChurchEventResult
    {
        public ChurchEvent Event { get; set; }

        public DateTime? TransferredFrom { get; set; }

        public ChurchEventResult(ChurchEvent churchEvent, DateTime? transferredFrom)
        {
            Event = churchEvent;
            TransferredFrom = transferredFrom;
        }
    }
}
