using System.Text.Json.Serialization;
using System.Text.Json;

namespace TeskeHomeAssistant.Helpers
{
    public static class ZigbeeEventHelpers
    {
        public static IObservable<Event<ZhaEventData>> Where(this IObservable<Event> eventSource, ZigbeeDeviceName zigbeeDeviceName, ZigbeeButtonCommands buttonCommand)
        {
            var deviceId = GlobalConfiguration.GetZigbeeDeviceId(zigbeeDeviceName);
            var command = GlobalConfiguration.GetZigbeeCommand(buttonCommand);
            return eventSource.Filter<ZhaEventData>("zha_event")
                .Where(e => e.Data?.DeviceIeee == deviceId && e.Data?.Command == command);
        }

        public static IObservable<Event<ZhaEventData>> Where(this IObservable<Event> eventSource, IEnumerable<ZigbeeDeviceName> zigbeeDeviceNames, ZigbeeButtonCommands buttonCommand)
        {
            var deviceIds = zigbeeDeviceNames.Select(GlobalConfiguration.GetZigbeeDeviceId).ToHashSet();
            
            var command = GlobalConfiguration.GetZigbeeCommand(buttonCommand);
            return eventSource.Filter<ZhaEventData>("zha_event")
                .Where(e => deviceIds.Contains(e.Data?.DeviceIeee ?? "") && e.Data?.Command == command);
        }

        public static IObservable<Event<ZhaEventData>> Where(this IObservable<Event> eventSource, ZigbeeDeviceName zigbeeDeviceName, ZigbeeSwitchCommands switchCommands)
        {
            var deviceId = GlobalConfiguration.GetZigbeeDeviceId(zigbeeDeviceName);
            var command = GlobalConfiguration.GetZigbeeCommand(switchCommands);
            return eventSource.Filter<ZhaEventData>("zha_event")
                .Where(e => e.Data?.DeviceIeee == deviceId && e.Data?.Command == command);
        }

        public static IObservable<Event<ZhaEventData>> Where(this IObservable<Event> eventSource, ZigbeeDeviceName zigbeeDeviceName)
        {
            var deviceId = GlobalConfiguration.GetZigbeeDeviceId(zigbeeDeviceName);
            return eventSource.Filter<ZhaEventData>("zha_event")
                .Where(e => e.Data?.DeviceIeee == deviceId);
        }
    }
}
