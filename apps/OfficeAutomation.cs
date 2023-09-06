namespace TeskeHomeAssistant.apps;

[NetDaemonApp]
public class OfficeAutomation
{
    public OfficeAutomation(IHaContext ha, IScheduler scheduler, Entities entities, ILogger<OfficeAutomation> logger)
    {
        var officeLights = new List<Entity>
        {
            entities.Light.OfficeDeskLamp1,
            entities.Light.OfficeDeskLamp2,
            entities.Light.Shapes7b48
        };

        var dimmer = new Dimmer(100, 20, 2);

        new MotionBuilder(entities.BinarySensor.OfficeSensorMotion, scheduler, logger)
            .WithMotionAllowed(entities.InputBoolean.OfficeMotionAllowed)
            .WithOnAction(_ => officeLights.TurnOn(dimmer.Current))
            .WithOffAction(_ =>
            {
                officeLights.TurnOff();
                dimmer.SetStep(2);
            }, TimeSpan.FromHours(1))
            .Build();

        ha.Events.Where(ZigbeeDeviceName.OfficeButton, ZigbeeButtonCommands.LongPress).Subscribe(_ =>
        {
            entities.InputBoolean.OfficeMotionAllowed.TurnOff();
            officeLights.TurnOff();
            dimmer.SetStep(2);
            scheduler.Schedule(TimeSpan.FromHours(2), () => entities.InputBoolean.OfficeMotionAllowed.TurnOn());
        });

        ha.Events.Where(ZigbeeDeviceName.OfficeButton, ZigbeeButtonCommands.Press).Subscribe(_ =>
        {
            entities.InputBoolean.OfficeMotionAllowed.TurnOff();
            officeLights.TurnOn(dimmer.Current);
            scheduler.Schedule(TimeSpan.FromHours(1), () => entities.InputBoolean.OfficeMotionAllowed.TurnOn());
        });

        ha.Events.Where(ZigbeeDeviceName.OfficeButton, ZigbeeButtonCommands.DoublePress).Subscribe(_ =>
        {
            officeLights.TurnOn(dimmer.Next());
            logger.LogInformation("New brightness is {@brightness}", dimmer.Current);
        });
    }
}
