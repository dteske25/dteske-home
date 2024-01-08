namespace TeskeHomeAssistant.Helpers
{
    public static class EntityHelpers
    {
        public static void TurnOn(this IEnumerable<Entity> entities, long? brightnessPct = null, long transitionSeconds = 1)
        {
            var turnOnData = new LightTurnOnParameters
            {
                Transition = transitionSeconds,
                BrightnessPct = brightnessPct ?? GlobalConfiguration.GetBrightness(),
            };
            foreach (var entity in entities)
            {
                if (entity is LightEntity light)
                {
                    light.TurnOn(turnOnData);
                }
                else if (entity is SwitchEntity switchEntity)
                {
                    switchEntity.TurnOn();
                }
                else if (entity is InputBooleanEntity inputBooleanEntity)
                {
                    inputBooleanEntity.TurnOn();
                }
            }
        }

        public static void TurnOff(this IEnumerable<Entity> entities)
        {
            var turnOffData = new LightTurnOffParameters
            {
                Transition = 1
            };
            foreach (var entity in entities)
            {
                if (entity is LightEntity light)
                {
                    light.TurnOff(turnOffData);
                }
                else if (entity is SwitchEntity switchEntity)
                {
                    switchEntity.TurnOff();
                }
                else if (entity is InputBooleanEntity inputBooleanEntity)
                {
                    inputBooleanEntity.TurnOff();
                }
            }
        }
    }
}
