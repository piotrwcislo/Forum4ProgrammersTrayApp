using Forum4Programmers.Client.Contracts;
using Forum4Programmers.Client.Model;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Forum4Programmers.Client
{
    public class PostClient : IPostClient
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string BaseUrl = "https://api.4programmers.net/v1/posts/";

        public async Task<Post> GetPostById(int postId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + postId.ToString());
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<Post>(await response.Content.ReadAsStringAsync());
            }

            return null;
        }
    }
}
