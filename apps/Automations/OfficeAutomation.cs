using Microsoft.Extensions.Logging;

namespace Automations;

/// <summary>
///     Showcase using the new HassModel API and turn on light on movement
/// </summary>
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

        var dimmer = new Dimmer(100, 20, 2);

        new MotionBuilder(entities.BinarySensor.OfficeSensorMotion, scheduler, logger)
            .WithMotionAllowed(entities.InputBoolean.OfficeMotionAllowed)
            .WithOnAction(_ => LightHelpers.TurnOn(officeLights, dimmer.Current))
            .WithOffAction(_ =>
            {
                LightHelpers.TurnOff(officeLights);
                dimmer.SetStep(2);
            }, TimeSpan.FromHours(1))
            .Build();

        ha.Events.Where(ZigbeeDeviceName.OfficeButton, ZigbeeButtonCommands.LongPress).Subscribe(_ =>
        {
            entities.InputBoolean.OfficeMotionAllowed.TurnOff();
            LightHelpers.TurnOff(officeLights);
            dimmer.SetStep(2);
            scheduler.Schedule(TimeSpan.FromHours(2), () => entities.InputBoolean.OfficeMotionAllowed.TurnOn());
        });

        ha.Events.Where(ZigbeeDeviceName.OfficeButton, ZigbeeButtonCommands.Press).Subscribe(_ =>
        {
            entities.InputBoolean.OfficeMotionAllowed.TurnOff();
            LightHelpers.TurnOn(officeLights, dimmer.Current);
            scheduler.Schedule(TimeSpan.FromHours(1), () => entities.InputBoolean.OfficeMotionAllowed.TurnOn());
        });

        ha.Events.Where(ZigbeeDeviceName.OfficeButton, ZigbeeButtonCommands.DoublePress).Subscribe(_ =>
        {
            LightHelpers.TurnOn(officeLights, dimmer.Next());
            logger.LogInformation("New brightness is {@brightness}", dimmer.Current);
        });
    }
}
