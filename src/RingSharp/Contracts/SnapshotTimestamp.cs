namespace RingSharp.Contracts
{
    using Newtonsoft.Json;
    using System;

    internal sealed class SnapshotTimestamp
    {
        [JsonProperty("timestamp")]
        public ulong MillisecondsSinceEpoch { get; set; }

        [JsonProperty("doorbot_id")]
        public int DoorbotId { get; set; }

        // public DateTimeOffset Timestamp => new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, new TimeSpan()).AddMilliseconds(this.MillisecondsSinceEpoch);
    }
}