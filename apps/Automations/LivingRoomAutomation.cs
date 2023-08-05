using HomeAssistantGenerated;
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
            entities.Light.LivingRoomLamp,
        };
        var dimmer = new Dimmer(100, 20, 2);

        ha.Events.Where(ZigbeeDeviceName.LivingRoomButton, ZigbeeButtonCommands.Press).Subscribe(_ =>
        {
            entities.Light.LivingRoomSwitchLight.TurnOn();
            entities.Switch.LivingRoomTableSwitch.TurnOn();
            entities.Switch.LivingRoomSquareLampSwitch.TurnOn();

            scheduler.Schedule(TimeSpan.FromSeconds(1), () =>
            {
                LightHelpers.TurnOn(lights, dimmer.Next());
            });
        });

        ha.Events.Where(ZigbeeDeviceName.LivingRoomButton, ZigbeeButtonCommands.DoublePress).Subscribe(_ =>
        {
            entities.Switch.LivingRoomSquareLampSwitch.TurnOff();
        });

        ha.Events.Where(ZigbeeDeviceName.LivingRoomButton, ZigbeeButtonCommands.LongPress).Subscribe(_ =>
        {
            LightHelpers.TurnOff(lights);
            entities.Switch.LivingRoomTableSwitch.TurnOff();
            entities.Switch.LivingRoomSquareLampSwitch.TurnOff();
            entities.Light.LivingRoomSwitchLight.TurnOff();
        });

        entities.Light.LivingRoomSwitchLight.StateChanges()
            .Subscribe(e =>
            {
                if (e.New?.State == "on")
                {
                    dimmer.SetStep(2);
                    entities.Switch.LivingRoomTableSwitch.TurnOn();
                    entities.Switch.LivingRoomSquareLampSwitch.TurnOn();
                    scheduler.Schedule(TimeSpan.FromSeconds(1), () =>
                    {
                        LightHelpers.TurnOn(lights, dimmer.Current);
                    });
                }
                if (e.New?.State == "off")
                {
                    LightHelpers.TurnOff(lights);
                    entities.Switch.LivingRoomTableSwitch.TurnOff();
                    entities.Switch.LivingRoomSquareLampSwitch.TurnOff();
                    entities.Light.LivingRoomSwitchLight.TurnOff();
                }
            });

    }
}
