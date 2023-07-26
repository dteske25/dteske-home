using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using HomeAssistantGenerated;

namespace Automations;

[NetDaemonApp]
public class HomeSchedule
{
    public HomeSchedule(IScheduler scheduler, Entities entities)
    {

        // 06:30 AM Mon-Fri
        scheduler.ScheduleCron("30 06 * * 1-5", () =>
        {
            entities.Light.Lamp.TurnOn();
            entities.Light.Nightstand.TurnOn();
            entities.Light.ElementsAc4b.TurnOn();
        });

        // 07:30 AM Sat & Sun
        scheduler.ScheduleCron("30 07 * * 6,0", () =>
        {
            entities.Light.Lamp.TurnOn();
            entities.Light.Nightstand.TurnOn();
            entities.Light.ElementsAc4b.TurnOn();
        });


        // 08:30 AM
        scheduler.ScheduleCron("30 08 * * *", () =>
        {
            entities.Light.Lamp.TurnOff();
            entities.Light.Nightstand.TurnOff();
            entities.Light.ElementsAc4b.TurnOff();
        });

        // 07:00 PM
        scheduler.ScheduleCron("00 19 * * *", () =>
        {
            entities.Light.OutsideFrontSwitch.TurnOn();
            entities.Light.HueFilamentBulb1.TurnOn();
            entities.Light.HueFilamentBulb2.TurnOn();
            entities.Light.HueFilamentBulb3.TurnOn();
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
        });

        // 11:30 PM
        scheduler.ScheduleCron("15 23 * * *", () =>
        {
            var fadeDuration = TimeSpan.FromMinutes(5).Seconds;
            entities.Light.ElementsAc4b.TurnOff(fadeDuration);
        });

    }
}
