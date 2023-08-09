namespace Automations;

[NetDaemonApp]
public class HallwayAutomation
{
    public HallwayAutomation(IHaContext ha, IScheduler scheduler, Entities entities, ILogger<HallwayAutomation> logger)
    {
        var hallwayLights = new List<LightEntity>
        {
            entities.Light.Hallway1,
            entities.Light.Hallway2,
        };

        new MotionBuilder(entities.BinarySensor.HallwaySensorMotion, scheduler, logger)
            .WithMotionAllowed(entities.Switch.HallwaySensorMotion)
            .WithOnAction(_ => 
            {
                entities.Light.HallwaySwitch.TurnOn();
                hallwayLights.TurnOn();
            })
            .WithOffAction(_ => hallwayLights.TurnOff(), TimeSpan.FromMinutes(2))
            .Build();

        ha.Events.Where(ZigbeeDeviceName.HallwaySwitch, ZigbeeSwitchCommands.On).Subscribe(_ =>
        {
            entities.Switch.HallwaySensorMotion.TurnOff();
            scheduler.Schedule(TimeSpan.FromHours(2), () => entities.Switch.HallwaySensorMotion.TurnOn());
            hallwayLights.TurnOn();
        });

        ha.Events.Where(ZigbeeDeviceName.HallwaySwitch, ZigbeeSwitchCommands.Off).Subscribe(_ =>
        {
            entities.Switch.HallwaySensorMotion.TurnOff();
            scheduler.Schedule(TimeSpan.FromHours(6), () => entities.Switch.HallwaySensorMotion.TurnOn());
            hallwayLights.TurnOff();
        });

    }
}
