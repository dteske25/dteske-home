namespace TeskeHomeAssistant.apps;

[NetDaemonApp(Id = "MasterBedroom")]
public class MasterBedroomAutomation
{
    public MasterBedroomAutomation(IHaContext ha, IScheduler scheduler, Entities entities, ILogger<MasterBedroomAutomation> logger)
    {
        var closetLights = new List<Entity>
        {
            entities.Light.MasterCloset1,
            entities.Light.MasterCloset2
        };

        var bedroomLights = new List<Entity>
        {
            entities.Light.Lamp,
            entities.Light.Nightstand,
            entities.Light.ElementsAc4b
        };

        var dimmer = new Dimmer(100, 20, 2);

        new MotionBuilder(entities.BinarySensor.MasterClosetMotion, scheduler, logger)
            .WithMotionAllowed(entities.Switch.MasterClosetMotion)
            .WithOnAction(_ => closetLights.TurnOn())
            .WithOffAction(_ => closetLights.TurnOff(), TimeSpan.FromMinutes(1))
            .Build();

        ha.Events.Where(ZigbeeDeviceName.MasterBedroomButton, ZigbeeButtonCommands.LongPress).Subscribe(_ =>
        {
            bedroomLights.TurnOff();
            closetLights.TurnOff(); ;
            entities.Switch.MasterClosetMotion.TurnOff();
            scheduler.Schedule(TimeSpan.FromHours(6), () => entities.Switch.MasterClosetMotion.TurnOn());
            dimmer.SetStep(1);
        });

        ha.Events.Where(ZigbeeDeviceName.MasterBedroomButton, ZigbeeButtonCommands.Press).Subscribe(_ =>
        {
            bedroomLights.TurnOn(dimmer.Next());
            entities.Switch.MasterClosetMotion.TurnOn();
        });
    }
}
