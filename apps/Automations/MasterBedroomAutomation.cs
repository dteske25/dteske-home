﻿using System.Linq;
using Microsoft.Extensions.Logging;

namespace Automations;

[NetDaemonApp]
public class MasterBedroomAutomation
{
    public MasterBedroomAutomation(IHaContext ha, IScheduler scheduler, Entities entities, ILogger<MasterBedroomAutomation> logger)
    {
        var closetLights = new List<LightEntity>
        {
            entities.Light.MasterCloset1,
            entities.Light.MasterCloset2
        };

        var bedroomLights = new List<LightEntity>
        {
            entities.Light.Lamp,
            entities.Light.Nightstand,
            entities.Light.ElementsAc4b
        };

        new MotionBuilder(entities.BinarySensor.MasterClosetMotion, scheduler, logger)
            .WithMotionAllowed(entities.Switch.MasterClosetMotion)
            .WithOnAction(_ => LightHelpers.TurnOn(closetLights))
            .WithOffAction(_ => LightHelpers.TurnOff(closetLights), TimeSpan.FromMinutes(1))
            .Build();

        ha.Events.Where(ZigbeeDeviceName.MasterBedroomButton, ZigbeeButtonCommands.LongPress).Subscribe(_ =>
        {
            entities.Switch.MasterClosetMotion.TurnOff();
            LightHelpers.TurnOff(closetLights);
            LightHelpers.TurnOff(bedroomLights);
            scheduler.Schedule(TimeSpan.FromHours(2), () => entities.Switch.MasterClosetMotion.TurnOn());
        });

        ha.Events.Where(ZigbeeDeviceName.OfficeButton, ZigbeeButtonCommands.Press).Subscribe(_ =>
        {
            entities.Switch.MasterClosetMotion.TurnOff();
            LightHelpers.TurnOn(closetLights);
            LightHelpers.TurnOn(bedroomLights);
            scheduler.Schedule(TimeSpan.FromHours(1), () => entities.Switch.MasterClosetMotion.TurnOn());
        });
    }
}