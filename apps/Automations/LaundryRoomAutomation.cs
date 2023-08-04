using Microsoft.Extensions.Logging;

namespace Automations;

[NetDaemonApp]
public class LaundryRoomAutomation
{
    public LaundryRoomAutomation(IScheduler scheduler, Entities entities, ILogger<LaundryRoomAutomation> logger)
    {
        var laundryRoomLights = new List<LightEntity>
        {
            entities.Light.LaundryRoomLight
        };

        new MotionBuilder(entities.BinarySensor.LaundryRoomSensorMotion, scheduler, logger)
            .WithMotionAllowed(entities.Switch.LaundryRoomSensorMotion)
            .WithOnAction(_ => LightHelpers.TurnOn(laundryRoomLights, 60))
            .WithOffAction(_ => LightHelpers.TurnOff(laundryRoomLights), TimeSpan.FromMinutes(5))
            .Build();
    }
}
