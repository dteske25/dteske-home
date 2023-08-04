using System.Linq;
using Microsoft.Extensions.Logging;

namespace Automations;

[Focus]
[NetDaemonApp]
public class HallwayAutomation
{
    public HallwayAutomation(IHaContext ha, IScheduler scheduler, Entities entities, ILogger<HallwayAutomation> logger)
    {
        var hallwayLights = new List<LightEntity>
        {
            entities.Light.Hallway1,
            entities.Light.Hallway2
        };

        new MotionBuilder(entities.BinarySensor.HallwaySensorMotion, scheduler, logger)
            .WithMotionAllowed(entities.Switch.HallwaySensorMotion)
            .WithOnAction(_ => LightHelpers.TurnOn(hallwayLights))
            .WithOffAction(_ => LightHelpers.TurnOff(hallwayLights), TimeSpan.FromMinutes(2))
            .Build();

        ha.Events.Where(ZigbeeDeviceName.HallwaySwitch, ZigbeeSwitchCommands.Off).Subscribe(_ =>
        {
            entities.Light.HallwaySwitch.TurnOn();
            entities.Switch.HallwaySensorMotion.TurnOff();
            LightHelpers.TurnOff(hallwayLights);
            scheduler.Schedule(TimeSpan.FromHours(6), () => entities.Switch.HallwaySensorMotion.TurnOn());
        });

        ha.Events.Where(ZigbeeDeviceName.HallwaySwitch, ZigbeeSwitchCommands.On).Subscribe(_ =>
        {
            entities.Switch.HallwaySensorMotion.TurnOff();
            LightHelpers.TurnOn(hallwayLights, 90);
            scheduler.Schedule(TimeSpan.FromHours(2), () => entities.Switch.HallwaySensorMotion.TurnOn());
        });
    }
}
