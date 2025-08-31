using VitalRouter;

namespace GASSample.Message
{
    public static class MessageContext
    {
        public static Router UIRouter { get; } = new();
        public static Router Gameplay { get; } = new();
    }
}