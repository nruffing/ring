namespace RingSharp.Contracts
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    internal sealed class SnapshotTimestampRequest
    {
        [JsonProperty("doorbot_ids")]
        public IEnumerable<int> DeviceIds { get; set; }
    }
}