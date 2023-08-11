namespace Automations;

[NetDaemonApp]
public class HomeSchedule
{
    public HomeSchedule(IScheduler scheduler, Entities entities, IHaContext ha)
    {
        var bedroomLights = new List<Entity>
        {
            entities.Light.Lamp,
            entities.Light.Nightstand,
            entities.Light.ElementsAc4b,
        };

        var drivewayLights = new List<Entity>
        {
            entities.Light.DrivewaySwitchLight,
            entities.Light.HueFilamentBulb1,
            entities.Light.HueFilamentBulb2,
            entities.Light.HueFilamentBulb3,
        };

        var fadeDuration = (int)TimeSpan.FromMinutes(5).TotalSeconds;

        // 06:30 AM Mon-Fri
        scheduler.ScheduleCron("25 06 * * 1-5", () =>
        {
            bedroomLights.TurnOn(GlobalConfiguration.BRIGHTNESS_HIGH, fadeDuration);
            ha.Message("Home Schedule", "6:30 AM Bedroom Lignts On");
        });

        // 07:30 AM Sat & Sun
        scheduler.ScheduleCron("25 07 * * 6,0", () =>
        {
            bedroomLights.TurnOn(GlobalConfiguration.BRIGHTNESS_HIGH, fadeDuration);
            ha.Message("Home Schedule", "7:30 AM Bedroom Lignts On");
        });

        // 08:30 AM
        scheduler.ScheduleCron("30 08 * * *", () =>
        {
            bedroomLights.TurnOff();
            ha.Message("Home Schedule", "8:30 AM Bedroom Lignts Off");
        });

        // 07:00 PM
        scheduler.ScheduleCron("00 19 * * *", () =>
        {
            drivewayLights.TurnOn();
            ha.Message("Home Schedule", "7:30 PM Outside Lights On");
        });

        // 11:00 PM
        scheduler.ScheduleCron("00 23 * * *", () =>
        {
            drivewayLights.TurnOff();
            ha.Message("Home Schedule", "11:00 PM Outside Lights Off");
        });

        // 11:15 PM
        scheduler.ScheduleCron("15 23 * * *", () =>
        {
            entities.Light.Lamp.TurnOff(fadeDuration);
            entities.Light.Nightstand.TurnOff(fadeDuration);
            ha.Message("Home Schedule", "11:15 PM Fade Bedroom Lights");
        });

        // 11:30 PM
        scheduler.ScheduleCron("25 23 * * *", () =>
        {
            entities.Light.ElementsAc4b.TurnOff(fadeDuration);
            ha.Message("Home Schedule", "11:15 PM Fade Bedroom Nanoleaf");
        });

    }
}
