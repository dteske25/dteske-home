using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeskeHomeAssistant.Enums;

namespace TeskeHomeAssistant.Helpers
{
    public static class ZigbeeEventHelpers
    {
        public static IObservable<Event> Where(this IObservable<Event> eventSource, ZigbeeDeviceName zigbeeDeviceName, ZigbeeButtonCommands buttonCommand)
        {
            var deviceId = GlobalConfiguration.GetZigbeeDeviceId(zigbeeDeviceName);
            var command = GlobalConfiguration.GetZigbeeCommand(buttonCommand);
            return eventSource.Where(e => e.EventType == "zha_event" &&
                e.DataElement?.GetProperty("device_ieee").GetString() == deviceId &&
                e.DataElement?.GetProperty("command").GetString() == command);
        }

        public static IObservable<Event> Where(this IObservable<Event> eventSource, ZigbeeDeviceName zigbeeDeviceName, ZigbeeSwitchCommands switchCommands)
        {
            var deviceId = GlobalConfiguration.GetZigbeeDeviceId(zigbeeDeviceName);
            var command = GlobalConfiguration.GetZigbeeCommand(switchCommands);
            return eventSource.Where(e => e.EventType == "zha_event" &&
                e.DataElement?.GetProperty("device_ieee").GetString() == deviceId &&
                e.DataElement?.GetProperty("command").GetString() == command);
        }

        public static IObservable<Event> Where(this IObservable<Event> eventSource, ZigbeeDeviceName zigbeeDeviceName)
        {

            var deviceId = GlobalConfiguration.GetZigbeeDeviceId(zigbeeDeviceName);
            return eventSource.Where(e =>
            {
                Console.WriteLine(e);
                return e.EventType == "zha_event" &&
                    e.DataElement?.GetProperty("device_ieee").GetString() == deviceId;
            });
        }
    }
}
