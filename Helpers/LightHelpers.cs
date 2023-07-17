using HomeAssistantGenerated;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeskeHomeAssistant.Helpers
{
    public static class LightHelpers
    {
        public static void TurnOn(IEnumerable<LightEntity> lights)
        {
            var turnOnData = new LightTurnOnParameters
            {
                Transition = 1,
                BrightnessPct = GlobalConfiguration.GetBrightness(),
            };
            foreach (var light in lights)
            {
                light.TurnOn(turnOnData);
            }
        }

        public static void TurnOff(IEnumerable<LightEntity> lights)
        {
            var turnOffData = new LightTurnOffParameters
            {
                Transition = 3
            };
            foreach (var light in lights)
            {
                light.TurnOff(turnOffData);
            }
        }
    }
}
