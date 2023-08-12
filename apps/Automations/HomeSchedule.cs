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

        // 06:30 AM Mon-Fri - turn on bedroom lights
        scheduler.ScheduleCron("25 06 * * 1-5", () =>
        {
            bedroomLights.TurnOn(GlobalConfiguration.BRIGHTNESS_HIGH, fadeDuration);
            ha.Message("Home Schedule", "6:30 AM Bedroom Lignts On", entities.InputBoolean.NetdaemonAutomationsHomeSchedule.EntityId);
        });

        // 07:30 AM Sat & Sun - turn on bedroom lights
        scheduler.ScheduleCron("25 07 * * 6,0", () =>
        {
            bedroomLights.TurnOn(GlobalConfiguration.BRIGHTNESS_HIGH, fadeDuration);
            ha.Message("Home Schedule", "7:30 AM Bedroom Lignts On", entities.InputBoolean.NetdaemonAutomationsHomeSchedule.EntityId);
        });

        // 08:30 AM - turn off bedroom lights
        scheduler.ScheduleCron("30 08 * * *", () =>
        {
            bedroomLights.TurnOff();
            ha.Message("Home Schedule", "8:30 AM Bedroom Lignts Off", entities.InputBoolean.NetdaemonAutomationsHomeSchedule.EntityId);
        });

        // 12:00 PM - Calculate sunset
        scheduler.ScheduleCron("00 12 * * *", () =>
        {
            var sunsetString = entities.Sun.Sun.Attributes?.NextSetting;
            var result = DateTimeOffset.TryParse(sunsetString, out DateTimeOffset sunset);
            if (result)
            {
                // If we got a sunset from HA, set the schedule for 30 min before that
                sunset = sunset.Subtract(TimeSpan.FromMinutes(30));
                scheduler.Schedule(sunset, () =>
                {
                    drivewayLights.TurnOn(GlobalConfiguration.BRIGHTNESS_HIGH);
                    bedroomLights.TurnOn(GlobalConfiguration.BRIGHTNESS_HIGH);
                    ha.Message("Home Schedule", "Outside & Bedroom Lights On", entities.InputBoolean.NetdaemonAutomationsHomeSchedule.EntityId);
                });
                ha.Message("Home Schedule", $"Lights scheduled to turn on at {sunset.LocalDateTime}", entities.InputBoolean.NetdaemonAutomationsHomeSchedule.EntityId);
            }
            else
            {
                // Otherwise fall back to 7 pm;
                var fallbackTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 0, 0);
                scheduler.Schedule(fallbackTime, (_sched, _offset) =>
                {
                    drivewayLights.TurnOn(GlobalConfiguration.BRIGHTNESS_HIGH);
                    bedroomLights.TurnOn(GlobalConfiguration.BRIGHTNESS_HIGH);
                    ha.Message("Home Schedule", "Outside & Bedroom Lights On", entities.InputBoolean.NetdaemonAutomationsHomeSchedule.EntityId);
                });
                ha.Message("Home Schedule", $"**Unable to parse sunset!** Lights scheduled to turn on at {fallbackTime}", entities.InputBoolean.NetdaemonAutomationsHomeSchedule.EntityId);
            }
        });

        // 11:00 PM
        scheduler.ScheduleCron("00 23 * * *", () =>
        {
            drivewayLights.TurnOff();
            ha.Message("Home Schedule", "11:00 PM Outside Lights Off", entities.InputBoolean.NetdaemonAutomationsHomeSchedule.EntityId);
        });

        // 11:15 PM
        scheduler.ScheduleCron("15 23 * * *", () =>
        {
            entities.Light.Lamp.TurnOff(fadeDuration);
            entities.Light.Nightstand.TurnOff(fadeDuration);
            ha.Message("Home Schedule", "11:15 PM Fade Bedroom Lights", entities.InputBoolean.NetdaemonAutomationsHomeSchedule.EntityId);
        });

        // 11:30 PM
        scheduler.ScheduleCron("25 23 * * *", () =>
        {
            entities.Light.ElementsAc4b.TurnOff(fadeDuration);
            ha.Message("Home Schedule", "11:25 PM Fade Bedroom Nanoleaf", entities.InputBoolean.NetdaemonAutomationsHomeSchedule.EntityId);
        });

    }
}
