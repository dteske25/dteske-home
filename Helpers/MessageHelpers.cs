namespace TeskeHomeAssistant.Helpers
{
    public static class MessageHelpers
    {
        public static void Message(this IHaContext ha, string name, string message, string? entityId = null, string? domain = null)
        {
            ha.CallService("logbook", "log", data: new
            {
                name,
                message,
                entity_id = entityId,
                domain
            });
        }
    }
}
