namespace TeskeHomeAssistant.apps;

[NetDaemonApp(Id = "LivingRoom")]
public class LivingRoomAutomation
{
    public LivingRoomAutomation(IHaContext ha, Entities entities)
    {
        var livingRoomLights = new List<Entity>
        {
            entities.Light.LivingRoomSwitchLight,
            entities.Light.LivingRoomLamp,
            entities.Switch.LivingRoomTableSwitch
        };
        var dimmer = new Dimmer(100, 20, 2);

        ha.Events.Where(ZigbeeDeviceName.LivingRoomButton, ZigbeeButtonCommands.Press).Subscribe(_ =>
        {
            livingRoomLights.TurnOn(dimmer.Next());
        });

        ha.Events.Where(ZigbeeDeviceName.LivingRoomButton, ZigbeeButtonCommands.DoublePress).Subscribe(_ =>
        {
            entities.Switch.LivingRoomSquareLampSwitch.Toggle();
        });

        ha.Events.Where(ZigbeeDeviceName.LivingRoomButton, ZigbeeButtonCommands.LongPress).Subscribe(_ =>
        {
            livingRoomLights.TurnOff();
        });

        entities.Light.LivingRoomSwitchLight.StateChanges()
            .Subscribe(e =>
            {
                if (e.New?.State == "on")
                {
                    dimmer.SetStep(2);
                    livingRoomLights.TurnOn(dimmer.Current);
                    entities.Switch.LivingRoomSquareLampSwitch.TurnOn();
                }
                if (e.New?.State == "off")
                {
                    livingRoomLights.TurnOff();
                    entities.Switch.LivingRoomSquareLampSwitch.TurnOff();
                }
            });

    }
}
