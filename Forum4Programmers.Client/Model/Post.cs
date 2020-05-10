using Newtonsoft.Json;
using System;

namespace Forum4Programmers.Client.Model
{
    public class Post
    {
        public int Id { get; set; }

        [JsonProperty("user_name")]
        public string AnonymousUserName { get; set; }

        public int Score { get; set; }

        [JsonProperty("forum_id")]
        public int ForumId { get; set; }

        [JsonProperty("topic_id")]
        public int TopicId { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }

        public string Excerpt { get; set; }

        public string Url { get; set; }
    }
}
