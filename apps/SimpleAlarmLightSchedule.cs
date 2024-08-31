namespace TeskeHomeAssistant.apps
{
    [NetDaemonApp]
    public class SimpleAlarmLightSchedule
    {
        private readonly IScheduler _scheduler;
        private readonly Entities _entities;
        private readonly IHaContext _ha;
        private readonly ILogger<AlarmLightSchedule> _logger;

        private Dictionary<string, IDisposable> ScheduledAlarms { get; set; } = new Dictionary<string, IDisposable>();

        private DateTimeOffset? NextAlarm { get; set; }

        public SimpleAlarmLightSchedule(IScheduler scheduler, Entities entities, IHaContext ha, ILogger<AlarmLightSchedule> logger)
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
                    NextAlarm = nextAlarmTime;
                    RecalculateSchedule(NextAlarm);
                }
            });
        }
        private void RecalculateSchedule(DateTimeOffset? nextAlarm)
        {
            if (nextAlarm != null)
            {
                ScheduledAlarms.Add($"Daric_{nextAlarm.Value}", _scheduler.Schedule(nextAlarm.Value, () =>
                {
                    var brightness = GlobalConfiguration.GetBrightness();
                    _logger.LogInformation("Picked target brightness as {@brightness}, turning on.", brightness);
                    _entities.Light.BedroomLamp1.TurnOn(new LightTurnOnParameters { BrightnessPct = brightness });
                    _entities.Light.BedroomLamp2.TurnOn(new LightTurnOnParameters { BrightnessPct = brightness });

                    _ha.Message("Alarm Schedule", "Bedroom Lamps On");
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
