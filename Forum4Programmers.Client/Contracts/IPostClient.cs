using Forum4Programmers.Client.Model;
using System.Threading.Tasks;

namespace Forum4Programmers.Client.Contracts
{
    public interface IPostClient
    {
        Task<Post> GetPostById(int postId);
    }
}
