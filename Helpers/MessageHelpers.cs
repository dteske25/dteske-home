using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeskeHomeAssistant.Helpers
{
    public static class MessageHelpers
    {
        public static void Message(this IHaContext ha, string message, string title = "")
        {
            ha.CallService("notify", "persistent_notification", data: new { message , title });
        }
    }
}
