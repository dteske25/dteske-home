namespace TeskeHomeAssistant.apps;

[NetDaemonApp(Id = "LaundryRoom")]
public class LaundryRoomAutomation
{
    public LaundryRoomAutomation(IScheduler scheduler, Entities entities, ILogger<LaundryRoomAutomation> logger)
    {
        var laundryRoomLights = new List<Entity>
        {
            entities.Light.LaundryRoomLight
        };

        new MotionBuilder(entities.BinarySensor.LaundryRoomSensorMotion, scheduler, logger)
            .WithMotionAllowed(entities.Switch.LaundryRoomSensorMotion)
            .WithOnAction(_ => laundryRoomLights.TurnOn(60))
            .WithOffAction(_ => laundryRoomLights.TurnOff(), TimeSpan.FromMinutes(5))
            .Build();
    }
}
