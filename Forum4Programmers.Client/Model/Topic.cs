using Newtonsoft.Json;
using System;

namespace Forum4Programmers.Client.Model
{
    public class Topic
    {
        public int Id { get; set; }
        public string Subject { get; set; }

        [JsonProperty("replies")]
        public int Replies { get; set; }
        public string Url { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("last_post_id")]
        public int LastPostId { get; set; }

        [JsonProperty("last_post_created_at")]
        public DateTime LastPostCreatedAt { get; set; }

        public Forum Forum { get; set; }
    }
}
