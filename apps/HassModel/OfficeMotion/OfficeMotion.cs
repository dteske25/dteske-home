// Use unique namespaces for your apps if you going to share with others to avoid
// conflicting names
using System.Reactive.Concurrency;
using HomeAssistantGenerated;
using NetDaemon.Extensions.Scheduler;
using TeskeHomeAssistant;

namespace HassModel;

/// <summary>
///     Showcase using the new HassModel API and turn on light on movement
/// </summary>
[NetDaemonApp]
public class OfficeMotion
{
    public OfficeMotion(IHaContext ha, IScheduler scheduler)
    {
        var entities = new Entities(ha);
        entities.BinarySensor.OfficeMotionIasZone
            .StateChanges()
            .Where(obv => obv.New?.State == "on")
            .Subscribe(_ =>
            {
                var turnOnData = new LightTurnOnParameters
                {
                    Transition = 3,
                    BrightnessPct = GlobalConfiguration.GetBrightness(),
                };
                entities.Light.HueBloom.TurnOn(turnOnData);
                entities.Light.HueBloom2.TurnOn(turnOnData);
                entities.Light.Shapes7b48.TurnOn(turnOnData);
            });

        entities.BinarySensor.OfficeMotionIasZone
            .StateChanges()
            .WhenStateIsFor(obv => obv?.State == "off", TimeSpan.FromMinutes(15), scheduler)
            .Subscribe(_ =>
            {
                var turnOffData = new LightTurnOffParameters
                {
                    Transition = 3
                };
                entities.Light.HueBloom.TurnOff(turnOffData); 
                entities.Light.HueBloom2.TurnOff(turnOffData);
                entities.Light.Shapes7b48.TurnOff(turnOffData);
            });


//        Event { DataElement = {"device_ieee":"00:12:4b:00:25:14:6f:60","unique_id":"00:12:4b:00:25:14:6f:60:1:0x0006","device_id":"4cb66373e53a04de144b897a321661f2","endpoint_id":1,"cluster_id":6,"command":"toggle","args":[],"params":{}}, EventType = zha_event, Origin = LOCAL, TimeFired = 7/11/2023 11:20:37 PM }
//Event { DataElement = {"device_ieee":"00:12:4b:00:25:14:6f:60","unique_id":"00:12:4b:00:25:14:6f:60:1:0x0006","device_id":"4cb66373e53a04de144b897a321661f2","endpoint_id":1,"cluster_id":6,"command":"off","args":[],"params":{}}, EventType = zha_event, Origin = LOCAL, TimeFired = 7/11/2023 11:20:40 PM }

        ha.Events.Where(e => e.EventType == "zha_event").Subscribe(e =>
        {
            var deviceId = e.DataElement?.GetProperty("device_id").GetString();
            var command = e.DataElement?.GetProperty("command").GetString();

            Console.WriteLine($"{deviceId} {command}");
        });
        entities.Button.EwelinkWb01606f1425Identify.StateChanges().Subscribe(obv => obv.ToString());
        entities.Button.EwelinkWb0164bd1625Identify.StateChanges().Subscribe(obv => obv.ToString());
        entities.Button.EwelinkWb01Cab21625Identify.StateChanges().Subscribe(obv => obv.ToString());
    }
}
