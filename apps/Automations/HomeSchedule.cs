namespace Automations;

[NetDaemonApp]
public class HomeSchedule
{
    public HomeSchedule(IScheduler scheduler, Entities entities, IHaContext ha)
    {

        // 06:30 AM Mon-Fri
        scheduler.ScheduleCron("30 06 * * 1-5", () =>
        {
            entities.Light.Lamp.TurnOn();
            entities.Light.Nightstand.TurnOn();
            entities.Light.ElementsAc4b.TurnOn();
            ha.Message("Home Schedule", "6:30 AM Bedroom Lignts On");
        });

        // 07:30 AM Sat & Sun
        scheduler.ScheduleCron("30 07 * * 6,0", () =>
        {
            entities.Light.Lamp.TurnOn();
            entities.Light.Nightstand.TurnOn();
            entities.Light.ElementsAc4b.TurnOn();
            ha.Message("Home Schedule", "7:30 AM Bedroom Lignts On");
        });


        // 08:30 AM
        scheduler.ScheduleCron("30 08 * * *", () =>
        {
            entities.Light.Lamp.TurnOff();
            entities.Light.Nightstand.TurnOff();
            entities.Light.ElementsAc4b.TurnOff();
            ha.Message("Home Schedule", "8:30 AM Bedroom Lignts Off");

        });

        // 07:00 PM
        scheduler.ScheduleCron("00 19 * * *", () =>
        {
            entities.Light.DrivewaySwitchLight.TurnOn();
            entities.Light.HueFilamentBulb1.TurnOn();
            entities.Light.HueFilamentBulb2.TurnOn();
            entities.Light.HueFilamentBulb3.TurnOn();
            ha.Message("Home Schedule", "7:30 PM Outside Lights On");
        });

        // 11:00 PM
        scheduler.ScheduleCron("00 23 * * *", () =>
        {
            entities.Light.HueFilamentBulb1.TurnOff();
            entities.Light.HueFilamentBulb2.TurnOff();
            entities.Light.HueFilamentBulb3.TurnOff();

            var fadeDuration = TimeSpan.FromMinutes(15).Seconds;

            entities.Light.Lamp.TurnOff(fadeDuration);
            entities.Light.Nightstand.TurnOff(fadeDuration);
            ha.Message("Home Schedule", "11 PM Outside Lights Off, Fade Bedroom Lights");

        });

        // 11:30 PM
        scheduler.ScheduleCron("15 23 * * *", () =>
        {
            var fadeDuration = TimeSpan.FromMinutes(5).Seconds;
            entities.Light.ElementsAc4b.TurnOff(fadeDuration);
            ha.Message("Home Schedule", "11:15 PM Fade Bedroom Nanoleaf");
        });

    }
}
