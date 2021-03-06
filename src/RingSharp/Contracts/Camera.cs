﻿namespace RingSharp.Contracts
{
    using Newtonsoft.Json;

    internal sealed class Camera
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
