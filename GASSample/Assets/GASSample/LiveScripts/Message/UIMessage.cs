using System;
using System.Runtime.CompilerServices;
using VitalRouter;

namespace GASSample.Message
{
    public interface IUICommand : ICommand
    {
        string CommandID { get; }
    }
    public readonly struct UIMessage<T> : IUICommand where T : struct
    {
        public string CommandID { get; }
        public readonly T Arg;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UIMessage(string inCommandId, in T arg)
        {
            CommandID = inCommandId;
            Arg = arg;
        }
    }

    public sealed class UIMessageClass<T> : IUICommand where T : class
    {
        public string CommandID { get; }
        public readonly T Arg;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UIMessageClass(string inCommandId, T arg)
        {
            CommandID = inCommandId;
            Arg = arg ?? throw new ArgumentNullException(nameof(arg));
        }
    }
}