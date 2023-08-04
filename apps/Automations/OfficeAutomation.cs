using Microsoft.Extensions.Logging;
using NetDaemon.Extensions.Tts;

namespace Automations;

/// <summary>
///     Showcase using the new HassModel API and turn on light on movement
/// </summary>
[Focus]
[NetDaemonApp]
public class OfficeAutomation
{
    public OfficeAutomation(IHaContext ha, IScheduler scheduler, Entities entities, ILogger<OfficeAutomation> logger)
    {
        var officeLights = new List<LightEntity>
        {
            entities.Light.OfficeDeskLamp1,            
            entities.Light.OfficeDeskLamp2,
            entities.Light.Shapes7b48
        };

        new MotionBuilder(entities.BinarySensor.OfficeSensorMotion, scheduler, logger)
            .WithMotionAllowed(entities.InputBoolean.OfficeMotionAllowed)
            .WithOnAction(_ => LightHelpers.TurnOn(officeLights, 60))
            .WithOffAction(_ => LightHelpers.TurnOff(officeLights))
            .Build();

        ha.Events.Where(ZigbeeDeviceName.OfficeButton, ZigbeeButtonCommands.LongPress).Subscribe(_ =>
        {
            entities.InputBoolean.OfficeMotionAllowed.TurnOff();
            LightHelpers.TurnOff(officeLights);
            scheduler.Schedule(TimeSpan.FromHours(2), () => entities.InputBoolean.OfficeMotionAllowed.TurnOn());
        });

        ha.Events.Where(ZigbeeDeviceName.OfficeButton, ZigbeeButtonCommands.Press).Subscribe(_ =>
        {
            entities.InputBoolean.OfficeMotionAllowed.TurnOff();
            LightHelpers.TurnOn(officeLights, 60);
            scheduler.Schedule(TimeSpan.FromHours(1), () => entities.InputBoolean.OfficeMotionAllowed.TurnOn());
        });
    }
}
