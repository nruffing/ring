namespace RingSharp.Contracts
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    
    internal sealed class DevicesResponse
    {
        [JsonProperty("doorbots")]
        public IEnumerable<Doorbot> Doorbots { get; set; }

        [JsonProperty("stickup_cams")]
        public IEnumerable<Camera> Cameras { get; set; }
    }
}