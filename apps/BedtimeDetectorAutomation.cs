//namespace TeskeHomeAssistant.apps;

//[NetDaemonApp]
//public class BedtimeDetectorAutomation
//{
//    const double IsSleepingConfidenceThreshold = .85;

//    public BedtimeDetectorAutomation(Entities entities)
//    {

//        entities.Sensor.DteskePixelSleepConfidence.StateChanges().Subscribe(state =>
//        {
//            if (state.New?.State >= IsSleepingConfidenceThreshold)
//            {
//                entities.Switch.Outlet1.TurnOff();
//            }
//        });

//    }
//}

