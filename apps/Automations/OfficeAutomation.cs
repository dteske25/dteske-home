namespace Automations;

/// <summary>
///     Showcase using the new HassModel API and turn on light on movement
/// </summary>
[NetDaemonApp]
public class OfficeAutomation
{
    private bool _motionEnabled;
    public OfficeAutomation(IHaContext ha, IScheduler scheduler, Entities entities)
    {
        var officeLights = new List<LightEntity>
        {
            entities.Light.HueBloom,
            entities.Light.HueBloom2,
            entities.Light.Shapes7b48
        };

        entities.BinarySensor.OfficeMotionIasZone
            .StateChanges()
            .Where(obv => obv.New?.State == "on")
            .Subscribe(_ =>
            {
                if (!_motionEnabled)
                {
                    return;
                }
                LightHelpers.TurnOn(officeLights);
            });

        entities.BinarySensor.OfficeMotionIasZone
            .StateChanges()
            .WhenStateIsFor(obv => obv?.State == "off", TimeSpan.FromMinutes(15), scheduler)
            .Subscribe(_ =>
            {
                if (!_motionEnabled)
                {
                    return;
                }
                LightHelpers.TurnOff(officeLights);
            });

        ha.Events.Where(ZigbeeDeviceName.Office, ZigbeeButtonCommands.LongPress).Subscribe(_ =>
        {
            _motionEnabled = false;
            LightHelpers.TurnOff(officeLights);
            scheduler.Schedule(TimeSpan.FromHours(6), () => _motionEnabled = true);
        });

        ha.Events.Where(ZigbeeDeviceName.Office, ZigbeeButtonCommands.Press).Subscribe(_ =>
        {
            _motionEnabled = false;
            LightHelpers.TurnOn(officeLights);
            scheduler.Schedule(TimeSpan.FromMinutes(15), () => _motionEnabled = true);
        });
    }
}
