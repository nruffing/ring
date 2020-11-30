namespace RingSharp.Contracts
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    internal sealed class SnapshotTimestampResponse
    {
        [JsonProperty("timestamps")]
        public IEnumerable<SnapshotTimestamp> Timestamps { get; set; }
    }
}