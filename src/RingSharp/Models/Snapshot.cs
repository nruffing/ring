namespace RingSharp.Models
{
    using System;   
    
    public sealed class Snapshot
    {
        public DateTimeOffset Timestamp { get; internal set; }

        public byte[] RawJpg { get; internal set; }
    }
}