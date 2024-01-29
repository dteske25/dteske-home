using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeskeHomeAssistant.Enums
{
    public enum ZigbeeSwitchCommands
    {
        On,
        Off
    }

    public enum ZigbeeButtonCommands
    {
        Press,
        DoublePress,
        LongPress,
    }

    public enum ZigbeeDeviceName
    {
        Unknown = 0,
        MeganBedroomButton,
        LivingRoomSwitch,
        LivingRoomOutlet,
        LivingRoomTable,
        OfficeButton,
        HallwaySwitch,
        DaricBedroomButton
    }
}
