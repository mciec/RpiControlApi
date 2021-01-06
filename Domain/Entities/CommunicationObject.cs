using System.Threading;
using System;

namespace RpiControl.Domain.Entities
{
    public enum BorderCrossed
    {
        Outside,
        Inside
    }
    public class CommunicationObject
    {
        public EventWaitHandle ObjectInside { get; } = new EventWaitHandle(false, EventResetMode.AutoReset);
        public EventWaitHandle ObjectOutside { get; } = new EventWaitHandle(false, EventResetMode.AutoReset);
        public EventHandler<BorderCrossed> BorderCrossedEvent { get; set; }
        public double Distance { get; internal set; }

    }
}