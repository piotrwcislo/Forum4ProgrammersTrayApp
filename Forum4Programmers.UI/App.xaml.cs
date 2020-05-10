using Forum4Programmers.Client;
using Forum4Programmers.Client.Contracts;
using System.Windows;
using Unity;

namespace Forum4Programmers.UI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var container = new UnityContainer();
            container.RegisterType<IPostClient, PostClient>();
            container.RegisterType<ITopicClient, TopicClient>();
        }
    }
}
