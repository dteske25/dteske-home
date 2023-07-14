using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }


}
