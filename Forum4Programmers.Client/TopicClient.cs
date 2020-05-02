using Forum4Programmers.Client.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Forum4Programmers.Client
{
    public class TopicClient
    {
        private const int MaxPageSniffCount = 15;
        private readonly HttpClient _client = new HttpClient();
        private readonly Uri _topicsUri = new Uri("http://api.4programmers.net/v1/topics?sort=last_post_id");
        public async Task<List<Topic>> GetLatestTopics(int count, string forumName = "JavaScript")
        {
            return await GetLatestTopics(count, topic => topic.Forum.Name.Contains(forumName));
        }

        public async Task<List<Topic>> GetLatestTopics(int count, int forumId = 52)
        {
            return await GetLatestTopics(count, topic => topic.Forum.Id == forumId);
        }

        private async Task<List<Topic>> GetLatestTopics(int count, Func<Topic, bool> topicPredicate)
        {
            var latestTopics = new List<Topic>();
            var uri = _topicsUri;

            var page = 0;
            while (latestTopics.Count < count && page++ < MaxPageSniffCount)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                HttpResponseMessage response = await _client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    TopicResponse topicsResponse = JsonConvert.DeserializeObject<TopicResponse>(json);
                    IEnumerable<Topic> topics = topicsResponse.Topics.Where(topicPredicate);
                    foreach (var topic in topics)
                    {
                        if (!latestTopics.Any(t => t.Id == topic.Id))
                        {
                            latestTopics.Add(topic);
                        }
                    }
                    uri = new Uri(topicsResponse.Navigation.NextPage + "&sort=last_post_id");
                }
            }

            return latestTopics.Take(count).ToList();
        }
    }
}
