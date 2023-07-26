using Microsoft.Extensions.Logging;

namespace Automations;

/// <summary>
///     Showcase using the new HassModel API and turn on light on movement
/// </summary>
[NetDaemonApp]
public class LivingRoomAutomation
{
    public LivingRoomAutomation(IHaContext ha, IScheduler scheduler, Entities entities, ILogger<LivingRoomAutomation> logger)
    {
        var lights = new List<LightEntity>
        {
            entities.Light.Bulb2,
        };

        ha.Events.Where(ZigbeeDeviceName.LivingRoomButton, ZigbeeButtonCommands.Press).Subscribe(_ =>
        {
            LightHelpers.TurnOn(lights);

            entities.Switch.LivingRoomTable.TurnOn();
            entities.Switch.LivingRoomSquareLamp.TurnOn();
        });

        ha.Events.Where(ZigbeeDeviceName.LivingRoomButton, ZigbeeButtonCommands.DoublePress).Subscribe(_ =>
        {
            entities.Switch.LivingRoomSquareLamp.TurnOff();
        });

        ha.Events.Where(ZigbeeDeviceName.LivingRoomButton, ZigbeeButtonCommands.LongPress).Subscribe(_ =>
        {
            LightHelpers.TurnOff(lights);
            entities.Switch.LivingRoomTable.TurnOff();
            entities.Switch.LivingRoomSquareLamp.TurnOff();
        });

        entities.Light.LivingRoomSwitch.StateChanges()
            .Subscribe(e =>
            {
                if (e.New?.State == "off")
                {
                    entities.Light.LivingRoomSwitch.TurnOn();
                    scheduler.Schedule(TimeSpan.FromSeconds(1), () =>
                    {
                        LightHelpers.TurnOff(lights);
                    });

                }
            });

    }
}
