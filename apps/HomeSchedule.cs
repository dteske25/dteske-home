namespace TeskeHomeAssistant.apps;

[NetDaemonApp]
public class HomeSchedule
{
    public HomeSchedule(IScheduler scheduler, Entities entities, IHaContext ha, ILogger<HomeSchedule> logger)
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

        List<IDisposable> alarmSchedule = new();
        entities.Sensor.DteskePixelNextAlarm.StateChanges().Subscribe(e =>
        {
            var alarmString = e.New?.State;
            var result = DateTimeOffset.TryParse(alarmString, out DateTimeOffset nextAlarm);
            nextAlarm = nextAlarm.ToOffset(DateTimeOffset.Now.Offset);

            logger.LogInformation("Detected alarm state change, new alarm time detected as {@alarmString}, parsed as {@nextAlarm}", alarmString, nextAlarm.LocalDateTime);
            // Clean up previously scheduled items
            foreach (var scheduled in alarmSchedule)
            {
                scheduled.Dispose();
            }
            logger.LogDebug("Cleaned up {@count} previously scheduled events", alarmSchedule.Count);
            alarmSchedule.Clear();

            if (result)
            {
                alarmSchedule.Add(scheduler.Schedule(nextAlarm.LocalDateTime, () =>
                {
                    bedroomLights.TurnOn(GlobalConfiguration.BRIGHTNESS_HIGH);
                    ha.Message("Home Schedule", "Bedroom Lights On", entities.InputBoolean.NetdaemonAutomationsHomeSchedule.EntityId);
                }));

                nextAlarm = nextAlarm.Subtract(TimeSpan.FromMinutes(5));
                alarmSchedule.Add(scheduler.Schedule(nextAlarm.LocalDateTime, () =>
                {
                    bedroomLights.TurnOn(GlobalConfiguration.BRIGHTNESS_HIGH, fadeDuration);
                }));
                ha.Message("Home Schedule", $"Lights scheduled to turn on at {nextAlarm.LocalDateTime}", entities.InputBoolean.NetdaemonAutomationsHomeSchedule.EntityId);

                nextAlarm = nextAlarm.AddMinutes(5).AddHours(2);
                alarmSchedule.Add(scheduler.Schedule(nextAlarm.LocalDateTime, () =>
                {
                    bedroomLights.TurnOff();
                    ha.Message("Home Schedule", "Bedroom Lignts Off", entities.InputBoolean.NetdaemonAutomationsHomeSchedule.EntityId);
                }));
            }
            else
            {
                ha.Message("Home Schedule", $"Detected alarm state change, but unable to parse {alarmString}", entities.InputBoolean.NetdaemonAutomationsHomeSchedule.EntityId);
            }
        });

        List<IDisposable> sunSchedule = new();
        entities.Sun.Sun.StateChanges().Subscribe(e =>
        {
            var sunsetString = e.New?.Attributes?.NextSetting;
            var result = DateTimeOffset.TryParse(sunsetString, out DateTimeOffset sunset);

            logger.LogInformation("Detected sun state change, new sun time detected as {@sunString}, parsed as {@nextAlarm}", sunsetString, sunset.LocalDateTime);
            // Clean up previously scheduled items
            foreach (var scheduled in sunSchedule)
            {
                scheduled.Dispose();
            }
            logger.LogDebug("Cleaned up {@count} previously scheduled events", sunSchedule.Count);
            sunSchedule.Clear();

            if (result)
            {
                // If we got a sunset from HA, set the schedule for 30 min before that
                sunset = sunset.Subtract(TimeSpan.FromMinutes(30));
                sunSchedule.Add(scheduler.Schedule(sunset.LocalDateTime, () =>
                {
                    drivewayLights.TurnOn(GlobalConfiguration.BRIGHTNESS_HIGH);
                    bedroomLights.TurnOn(GlobalConfiguration.BRIGHTNESS_HIGH);
                    ha.Message("Home Schedule", "Outside & Bedroom Lights On", entities.InputBoolean.NetdaemonAutomationsHomeSchedule.EntityId);
                }));
                ha.Message("Home Schedule", $"Lights scheduled to turn on at {sunset.LocalDateTime}", entities.InputBoolean.NetdaemonAutomationsHomeSchedule.EntityId);
            }
            else
            {
                // Otherwise fall back to 7 pm
                var fallbackTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 0, 0);
                sunSchedule.Add(scheduler.Schedule(fallbackTime, (_sched, _offset) =>
                {
                    drivewayLights.TurnOn(GlobalConfiguration.BRIGHTNESS_HIGH);
                    bedroomLights.TurnOn(GlobalConfiguration.BRIGHTNESS_HIGH);
                    ha.Message("Home Schedule", "Outside & Bedroom Lights On", entities.InputBoolean.NetdaemonAutomationsHomeSchedule.EntityId);
                }));
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
