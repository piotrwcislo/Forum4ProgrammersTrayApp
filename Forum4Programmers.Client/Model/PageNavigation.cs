using Newtonsoft.Json;

namespace Forum4Programmers.Client.Model
{
    public class PageNavigation
    {
        [JsonProperty("next")]
        public string NextPage { get; set; }
    }
}
