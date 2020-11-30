namespace RingSharp.Contracts
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    internal sealed class UpdateSnapshotRequest
    {
        [JsonProperty("doorbot_ids")]
        public IEnumerable<int> DeviceIds { get; set; }

        [JsonProperty("refresh")]
        public bool Refresh => true;
    }
}
