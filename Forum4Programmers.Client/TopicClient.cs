using Forum4Programmers.Client.Contracts;
using Forum4Programmers.Client.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Forum4Programmers.Client
{
    public class TopicClient : ITopicClient
    {
        private const int MaxPageSniffCount = 15;
        private readonly HttpClient _client = new HttpClient();
        private readonly string _baseUrl = "http://api.4programmers.net/v1/topics";

        private Func<Topic, bool> ForumIdFilter(int forumId) => (Topic topic) => topic.Forum.Id == forumId;
        private Func<Topic, bool> ForumNameFilter(string forumName) => (Topic topic) => topic.Forum.Name == forumName;

        public async Task<List<Topic>> GetLastTopics(int count, string forumName = "JavaScript")
        {
            return await GetTopics(count, ForumNameFilter(forumName), string.Empty);
        }

        public async Task<List<Topic>> GetLastTopics(int count, int forumId = 52)
        {
            return await GetTopics(count, ForumIdFilter(forumId), string.Empty);
        }

        public async Task<List<Topic>> GetLastTopicsByLastPostCreatedAt(int count, string forumName = "JavaScript")
        {
            return await GetTopics(count, ForumNameFilter(forumName), "sort=last_post_id");
        }

        public async Task<List<Topic>> GetLastTopicsByLastPostCreatedAt(int count, int forumId = 52)
        {
            return await GetTopics(count, ForumIdFilter(forumId), "sort=last_post_id");
        }

        private async Task<List<Topic>> GetTopics(int count, Func<Topic, bool> topicPredicate, string queryString)
        {
            var latestTopics = new List<Topic>();
            var uri = new Uri(_baseUrl + "?" + queryString);

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
                    uri = new Uri(topicsResponse.Navigation.NextPage + "&" + queryString);
                }
            }

            return latestTopics.Take(count).ToList();
        }
    }
}
