namespace RingSharp.Contracts
{
    using Newtonsoft.Json;

    internal sealed class Doorbot
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}