using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using HomeAssistantGenerated;
using Microsoft.Extensions.Logging;
using NetDaemon.Extensions.Scheduler;

namespace Automations;

[NetDaemonApp]
public class MasterBedroomAutomation
{
    public MasterBedroomAutomation(IHaContext ha, IScheduler scheduler, Entities entities, ILogger<MasterBedroomAutomation> logger)
    {
        var closetLights = new List<LightEntity>
        {
            entities.Light.Closet1,
            entities.Light.Closet2
        };

        var bedroomLights = new List<LightEntity>
        {
            entities.Light.Lamp,
            entities.Light.Nightstand,
            entities.Light.ElementsAc4b
        };

        new MotionBuilder(entities.BinarySensor.ClosetSensorMotion, scheduler, logger)
            .WithMotionAllowed(entities.Switch.ClosetSensorMotion)
            .WithOnAction(_ => LightHelpers.TurnOn(closetLights))
            .WithOffAction(_ => LightHelpers.TurnOff(closetLights))
            .Build();

        ha.Events.Where(ZigbeeDeviceName.MasterBedroomButton, ZigbeeButtonCommands.LongPress).Subscribe(_ =>
        {
            entities.Switch.ClosetSensorMotion.TurnOff();
            LightHelpers.TurnOff(closetLights);
            LightHelpers.TurnOff(bedroomLights);
            scheduler.Schedule(TimeSpan.FromHours(2), () => entities.Switch.ClosetSensorMotion.TurnOn());
        });

        ha.Events.Where(ZigbeeDeviceName.OfficeButton, ZigbeeButtonCommands.Press).Subscribe(_ =>
        {
            entities.Switch.ClosetSensorMotion.TurnOff();
            LightHelpers.TurnOn(closetLights);
            LightHelpers.TurnOn(bedroomLights);
            scheduler.Schedule(TimeSpan.FromHours(1), () => entities.Switch.ClosetSensorMotion.TurnOn());
        });
    }
}
