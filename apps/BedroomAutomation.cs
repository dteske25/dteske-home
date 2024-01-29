using System.Reactive.Concurrency;

namespace TeskeHomeAssistant.apps;

[NetDaemonApp]
public class BedroomAutomation
{
    public BedroomAutomation(IScheduler scheduler, IHaContext ha, Entities entities)
    {
        var dimmer = new Dimmer(100, 20, 2);

        var buttons = new List<ZigbeeDeviceName>
        {
            ZigbeeDeviceName.DaricBedroomButton,
            ZigbeeDeviceName.MeganBedroomButton
        };

        ha.Events.Where(buttons, ZigbeeButtonCommands.LongPress).Subscribe(_ =>
        {
            entities.Light.BedroomFan.TurnOff();
            entities.Light.BedroomLamp.TurnOff();
            entities.Light.BedroomLamp2.TurnOff();
            dimmer.SetStep(1);
        });

        ha.Events.Where(buttons, ZigbeeButtonCommands.Press).Subscribe(_ =>
        {
            var brightnessPercentage = dimmer.Next();
            entities.Light.BedroomLamp.TurnOn(brightnessPct:  brightnessPercentage);
            entities.Light.BedroomLamp2.TurnOn(brightnessPct:  brightnessPercentage);
        });

        ha.Events.Where(buttons, ZigbeeButtonCommands.DoublePress).Subscribe(_ =>
        {
            entities.Light.BedroomFan.Toggle();
        });

        scheduler.ScheduleCron("00 09 * * *", () =>
        {
            entities.Light.BedroomLamp.TurnOff();
            entities.Light.BedroomLamp2.TurnOff();
            ha.Message("Alarm Light Schedule", "09:00 AM Bedroom Lights Off");
        });

        scheduler.ScheduleCron("00 20 * * *", () =>
        {
            entities.Light.BedroomLamp.TurnOn();
            entities.Light.BedroomLamp2.TurnOn();
            ha.Message("Alarm Light Schedule", "08:00 PM Bedroom Lights On");
        });

        scheduler.ScheduleCron("00 22 * * *", () =>
        {
            entities.Light.BedroomLamp.TurnOff(transition: 60);
            entities.Light.BedroomLamp2.TurnOff(transition: 60);
            ha.Message("Alarm Light Schedule", "10:00 PM Bedroom Lights Off");
        });
    }
}
