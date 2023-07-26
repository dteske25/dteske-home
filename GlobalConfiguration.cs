using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeskeHomeAssistant.Enums;

namespace TeskeHomeAssistant
{


    public class GlobalConfiguration
    {
        public const int BRIGHTNESS_DIM = 15;
        public const int BRIGHTNESS_LOW = 30;
        public const int BRIGHTNESS_MED = 60;
        public const int BRIGHTNESS_HIGH = 90;



        public static int GetBrightness()
        {
            var hour = DateTime.Now.Hour;
            return hour switch
            {
                int when hour < 5 => BRIGHTNESS_DIM,
                int when hour >= 8 && hour < 20 => BRIGHTNESS_HIGH,
                int when hour >= 22 => BRIGHTNESS_LOW,

                _ => BRIGHTNESS_MED
            };

        }

        public static string GetZigbeeDeviceId(ZigbeeDeviceName device) => device switch
        {
            ZigbeeDeviceName.LivingRoomButton => "00:12:4b:00:25:16:bd:64",
            ZigbeeDeviceName.LivingRoomSwitch => "84:fd:27:ff:fe:bc:cc:8e",
            ZigbeeDeviceName.LivingRoomOutlet => "00:12:4b:00:25:3d:67:7f",
            ZigbeeDeviceName.LivingRoomTable => "00:12:4b:00:25:3d:6a:08",
            ZigbeeDeviceName.OfficeButton => "00:12:4b:00:25:14:6f:60",
            ZigbeeDeviceName.HallwaySwitch => "84:fd:27:ff:fe:bc:cc:65",
            ZigbeeDeviceName.MasterBedroomButton => "00:12:4b:00:25:16:b2:ca",
            _ => ""
        };

        public static string GetZigbeeCommand(ZigbeeButtonCommands buttonCommand) => buttonCommand switch
        {
            ZigbeeButtonCommands.Press => "toggle",
            ZigbeeButtonCommands.DoublePress => "on",
            ZigbeeButtonCommands.LongPress => "off",
            _ => ""
        };

        public static string GetZigbeeCommand(ZigbeeSwitchCommands switchCommand) => switchCommand switch
        {
            ZigbeeSwitchCommands.On => "on",
            ZigbeeSwitchCommands.Off => "off",
            _ => ""
        };

    }
}
