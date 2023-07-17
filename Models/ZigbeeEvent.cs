using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeskeHomeAssistant.Enums;

namespace TeskeHomeAssistant.Models
{
    public record ZigbeeEvent : Event
    {
        protected ZigbeeEvent(Event original) : base(original)
        {

        }

        public ZigbeeButtonCommands Command { get; }
    }
}
