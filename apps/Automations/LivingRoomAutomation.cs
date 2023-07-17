namespace Automations;

/// <summary>
///     Showcase using the new HassModel API and turn on light on movement
/// </summary>
[NetDaemonApp]
public class LivingRoomAutomation
{
    public LivingRoomAutomation(IHaContext ha, IScheduler scheduler, Entities entities, ZhaServices zhaServices)
    {
        var lights = new List<LightEntity>
        {
            entities.Light.Bulb2,


        };
        ha.Events.Where(ZigbeeDeviceName.LivingRoomButton, ZigbeeButtonCommands.Press).Subscribe(_ =>
        {
            LightHelpers.TurnOn(lights);

            entities.Switch.SonoffS31LiteZb086a3d25OnOff.TurnOn();
            entities.Switch.SonoffS31LiteZb7f673d25OnOff.TurnOn();
        });

        ha.Events.Where(ZigbeeDeviceName.LivingRoomButton, ZigbeeButtonCommands.DoublePress).Subscribe(_ =>
        {
            entities.Switch.SonoffS31LiteZb086a3d25OnOff.TurnOff();
        });

        ha.Events.Where(ZigbeeDeviceName.LivingRoomButton, ZigbeeButtonCommands.LongPress).Subscribe(_ =>
        {
            LightHelpers.TurnOff(lights);
            entities.Switch.SonoffS31LiteZb086a3d25OnOff.TurnOff();
            entities.Switch.SonoffS31LiteZb7f673d25OnOff.TurnOff();
        });

        entities.Light.JascoProducts430848eccbcfeOnOff.StateChanges()
            .Subscribe(e =>
            {
                if (e.New?.State == "off")
                {
                    entities.Light.JascoProducts430848eccbcfeOnOff.TurnOn();
                    scheduler.Schedule(TimeSpan.FromSeconds(1), () =>
                    {
                        LightHelpers.TurnOff(lights);
                    });

                }
            });

    }
}
