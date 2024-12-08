using System.Reactive.Concurrency;

namespace TeskeHomeAssistant.apps;

[NetDaemonApp]
public class LivingRoomAutomation
{
    public LivingRoomAutomation(IHaContext ha, Entities entities, IScheduler scheduler)
    {

        scheduler.ScheduleCron("00 18 * * *", () =>
        {
            entities.Switch.Outlet1.TurnOn();
            ha.Message("Living Room Light Schedule", "06:00 PM Christmas Tree On");
        });

        scheduler.ScheduleCron("00 01 * * *", () =>
        {
            entities.Switch.Outlet1.TurnOff();
            ha.Message("Living Room Light Schedule", "01:00 AM Christmas Tree Off");
        });
    }
}
