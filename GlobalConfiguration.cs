namespace TeskeHomeAssistant;

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
            int when hour >= 6 && hour < 8 => BRIGHTNESS_LOW,
            int when hour >= 8 && hour < 10 => BRIGHTNESS_MED,
            int when hour >= 10 && hour < 18 => BRIGHTNESS_HIGH,
            int when hour >= 18 && hour < 21 => BRIGHTNESS_MED,
            int when hour >= 21 && hour < 23 => BRIGHTNESS_LOW,

            _ => BRIGHTNESS_DIM
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
