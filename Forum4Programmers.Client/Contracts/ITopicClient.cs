using Forum4Programmers.Client.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forum4Programmers.Client.Contracts
{
    public interface ITopicClient
    {
        Task<List<Topic>> GetLastTopics(int count, int forumId = 52);
        Task<List<Topic>> GetLastTopics(int count, string forumName = "JavaScript");
        Task<List<Topic>> GetLastTopicsByLastPostCreatedAt(int count, int forumId = 52);
        Task<List<Topic>> GetLastTopicsByLastPostCreatedAt(int count, string forumName = "JavaScript");
    }
}