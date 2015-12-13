using Newtonsoft.Json;

namespace DigiAeon.EvilApiClient.Services.Interfaces.EvilApi
{
    public class UploadCustomerResponse
    {
        [JsonProperty(PropertyName = "customer")]
        public string Customer { get; set; }

        [JsonProperty(PropertyName = "value")]
        public int Value { get; set; }

        [JsonProperty(PropertyName = "added")]
        public bool Added { get; set; }

        [JsonProperty(PropertyName = "hash")]
        public string Hash { get; set; }

        [JsonProperty(PropertyName = "errors")]
        public string[] Errors { get; set; }
    }
}
