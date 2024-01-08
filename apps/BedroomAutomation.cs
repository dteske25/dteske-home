namespace TeskeHomeAssistant.apps;

[NetDaemonApp]
public class BedroomAutomation
{
    public BedroomAutomation(IHaContext ha, Entities entities)
    {

        var dimmer = new Dimmer(100, 20, 2);

        ha.Events.Where(ZigbeeDeviceName.MasterBedroomButton, ZigbeeButtonCommands.LongPress).Subscribe(_ =>
        {
            entities.Light.BedroomFan.TurnOff();
            entities.Light.BedroomLamp.TurnOff();
            entities.Light.BedroomLamp2.TurnOff();
            dimmer.SetStep(1);
        });

        ha.Events.Where(ZigbeeDeviceName.MasterBedroomButton, ZigbeeButtonCommands.Press).Subscribe(_ =>
        {
            var brightnessPercentage = dimmer.Next();
            entities.Light.BedroomLamp.TurnOn(brightnessPct:  brightnessPercentage);
            entities.Light.BedroomLamp2.TurnOn(brightnessPct:  brightnessPercentage);
        });

        ha.Events.Where(ZigbeeDeviceName.MasterBedroomButton, ZigbeeButtonCommands.DoublePress).Subscribe(_ =>
        {
            entities.Light.BedroomFan.Toggle();
        });
    }
}
