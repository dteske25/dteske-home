namespace TeskeHomeAssistant.apps
{
    public class AlarmLightSchedule
    {
        const int FADE_DURATION_SEC = 10;


        private readonly IScheduler _scheduler;
        private readonly Entities _entities;
        private readonly IHaContext _ha;
        private readonly ILogger<AlarmLightSchedule> _logger;

        private Dictionary<string, IDisposable> ScheduledAlarms { get; set; } = new Dictionary<string, IDisposable>();

        private DateTimeOffset? DaricNextAlarm { get; set; }
        private DateTimeOffset? MeganNextAlarm { get; set; }

        public AlarmLightSchedule(IScheduler scheduler, Entities entities, IHaContext ha, ILogger<AlarmLightSchedule> logger)
        {
            _scheduler = scheduler;
            _entities = entities;
            _ha = ha;
            _logger = logger;

            entities.Sensor.DteskePixelNextAlarm.StateChanges().Subscribe(e =>
            {
                var nextAlarmTime = GetNextAlarm(e);
                if (nextAlarmTime.HasValue)
                {
                    ClearScheduledAlarms("Daric_");
                    DaricNextAlarm = nextAlarmTime;
                    RecalculateSchedule(DaricNextAlarm, MeganNextAlarm);
                }
            });
        }
        private void RecalculateSchedule(DateTimeOffset? daricNextAlarm, DateTimeOffset? meganNextAlarm)
        {
            if (daricNextAlarm != null)
            {
                ScheduledAlarms.Add($"Daric_{daricNextAlarm.Value}", _scheduler.Schedule(daricNextAlarm.Value, () =>
                {
                    var daricBrightness = PickBrightnessPercentage(daricNextAlarm.Value, meganNextAlarm);
                    _logger.LogInformation("Picked Daric's target brightness as {@daricBrightness}, turning on.", daricBrightness);
                    _entities.Light.BedroomLamp1.TurnOn(new LightTurnOnParameters { BrightnessPct = daricBrightness });

                    if (daricBrightness == GlobalConfiguration.BRIGHTNESS_HIGH)
                    {
                        _logger.LogInformation("Turning on Megan's to full brightness as well");
                        _entities.Light.BedroomLamp2.TurnOn(new LightTurnOnParameters { BrightnessPct = GlobalConfiguration.BRIGHTNESS_HIGH, Transition = FADE_DURATION_SEC });
                    }
                    _ha.Message("Alarm Schedule", "Daric Bedroom Light On");
                }));
            }

            if (meganNextAlarm != null)
            {
                ScheduledAlarms.Add($"Megan_{meganNextAlarm.Value}", _scheduler.Schedule(meganNextAlarm.Value, () =>
                {
                    var meganBrightness = PickBrightnessPercentage(meganNextAlarm.Value, daricNextAlarm);
                    _logger.LogInformation("Picked Megan's target brightness as {@meganBrightness}, turning on.", meganBrightness);
                    _entities.Light.BedroomLamp2.TurnOn(new LightTurnOnParameters { BrightnessPct = meganBrightness });

                    if (meganBrightness == GlobalConfiguration.BRIGHTNESS_HIGH)
                    {
                        _logger.LogInformation("Turning on Daric's to full brightness as well");
                        _entities.Light.BedroomLamp1.TurnOn(new LightTurnOnParameters { BrightnessPct = GlobalConfiguration.BRIGHTNESS_HIGH, Transition = FADE_DURATION_SEC });
                    }
                    _ha.Message("Alarm Schedule", "Megan Bedroom Light On");
                }));
            }
        }

        private DateTimeOffset? GetNextAlarm(StateChange<SensorEntity, EntityState<SensorAttributes>> stateChange)
        {
            var alarmString = stateChange.New?.State;
            var result = DateTimeOffset.TryParse(alarmString, out DateTimeOffset nextAlarm);
            if (result)
            {
                nextAlarm = nextAlarm.ToOffset(DateTimeOffset.Now.Offset);
                _logger.LogInformation("Detected alarm state change, new alarm time detected as {@alarmString}, parsed as {@nextAlarm}", alarmString, nextAlarm.LocalDateTime);
                return nextAlarm;
            }
            _ha.Message("Alarm Schedule", $"**Unable to parse alarm string!**");
            return null;
        }

        private int PickBrightnessPercentage(DateTimeOffset myAlarm, DateTimeOffset? otherAlarm)
        {
            if (otherAlarm == null)
            {
                return GlobalConfiguration.BRIGHTNESS_LOW;
            }

            var comparison = myAlarm.CompareTo(otherAlarm.Value);
            if (comparison < 0)
            {
                return GlobalConfiguration.BRIGHTNESS_LOW;
            }

            if (comparison >= 0)
            {
                return GlobalConfiguration.BRIGHTNESS_HIGH;
            }

            return GlobalConfiguration.BRIGHTNESS_MED;
        }

        private void ClearScheduledAlarms(string prefix)
        {
            var scheduled = ScheduledAlarms.Keys.Where(s => s.StartsWith(prefix)).ToList();
            _logger.LogDebug("Cleaning up {@count} previously scheduled events with prefix {@prefix}", scheduled.Count, prefix);
            foreach (var key in scheduled)
            {
                ScheduledAlarms[key].Dispose();
                ScheduledAlarms.Remove(key);
                _logger.LogDebug("Disposed and removed {@key}", key);
            }
        }
    }
}
