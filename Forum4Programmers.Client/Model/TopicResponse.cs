using Newtonsoft.Json;
using System.Collections.Generic;

namespace Forum4Programmers.Client.Model
{
    public class TopicResponse
    {
        [JsonProperty("data")]
        public IEnumerable<Topic> Topics { get; set; }

        [JsonProperty("links")]
        public PageNavigation Navigation { get; set; }
    }
}
