namespace TeskeHomeAssistant.apps;

[NetDaemonApp]
public class OfficeAutomation
{
    public OfficeAutomation(IHaContext ha, IScheduler scheduler, Entities entities, ILogger<OfficeAutomation> logger)
    {
        var dimmer = new Dimmer(100, 20, 2);

        new MotionBuilder(entities.BinarySensor.OfficeMotionSensor, scheduler, logger)
            .WithMotionAllowed(entities.InputBoolean.OfficeMotionAllowed)
            .WithOnAction(_ =>
            {
                entities.Light.Shapes7b48.TurnOn();
                entities.Light.OfficeLights.TurnOn(brightnessPct: dimmer.Current);
            })
            .WithOffAction(_ =>
            {
                entities.Light.OfficeLights.TurnOff();
                entities.Light.Shapes7b48.TurnOff();
                dimmer.SetStep(2);
            }, TimeSpan.FromHours(1))
            .Build();

        ha.Events.Where(ZigbeeDeviceName.OfficeButton, ZigbeeButtonCommands.Press).Subscribe(_ =>
        {
            entities.InputBoolean.OfficeMotionAllowed.TurnOff();
            entities.Light.OfficeLights.TurnOn(brightnessPct: dimmer.Next());
            entities.Light.Shapes7b48.TurnOn();
            logger.LogInformation("New brightness is {@brightness}", dimmer.Current);
            scheduler.Schedule(TimeSpan.FromHours(1), () => entities.InputBoolean.OfficeMotionAllowed.TurnOn());
        });

        ha.Events.Where(ZigbeeDeviceName.OfficeButton, ZigbeeButtonCommands.DoublePress).Subscribe(_ =>
        {
            entities.Switch.Outlet1.Toggle();
        });

        ha.Events.Where(ZigbeeDeviceName.OfficeButton, ZigbeeButtonCommands.LongPress).Subscribe(_ =>
        {
            entities.InputBoolean.OfficeMotionAllowed.TurnOff();
            entities.Switch.Outlet1.TurnOff();
            entities.Light.OfficeLights.TurnOff();
            entities.Light.Shapes7b48.TurnOff();

            dimmer.SetStep(2);
            scheduler.Schedule(TimeSpan.FromHours(2), () => entities.InputBoolean.OfficeMotionAllowed.TurnOn());
        });
    }
}
