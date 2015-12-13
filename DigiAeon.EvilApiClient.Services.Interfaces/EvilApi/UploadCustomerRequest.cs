using Newtonsoft.Json;

namespace DigiAeon.EvilApiClient.Services.Interfaces.EvilApi
{
    public class UploadCustomerRequest
    {

        [JsonProperty(PropertyName = "property")]
        public string Property { get; set; }

        [JsonProperty(PropertyName = "customer")]
        public string Customer { get; set; }

        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        [JsonProperty(PropertyName = "value")]
        public int Value { get; set; }

        [JsonProperty(PropertyName = "file")]
        public string File { get; set; }
    }
}