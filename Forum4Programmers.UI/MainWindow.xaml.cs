using Forum4Programmers.Client;
using Forum4Programmers.Client.Contracts;
using Forum4Programmers.Client.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Forum4Programmers.TrayApp.Configuration;
using Application = System.Windows.Application;

namespace Forum4Programmers.UI
{
    public partial class MainWindow : Window
    {
        private const int LastTopicsToGetCount = 5;
        private readonly NotifyIcon _notifyIcon;
        private readonly ToolStripItemCollection _contextMenuItems;

        private readonly ITopicClient _topicClient;
        private readonly IPostClient _postClient;

        private readonly ForumConfigurationSection _configuration;
        private readonly IEnumerable<ForumConfigElement> _forumsToCheck;

        public MainWindow(ITopicClient topicClient, IPostClient postClient)
        {
            _configuration = ForumConfigurationSection.GetForumConfiguration();
            _forumsToCheck = _configuration.Forums.Where(forumConfig => forumConfig.Enabled);

            _topicClient = topicClient;
            _postClient = postClient;

            InitializeComponent();
            ShowInTaskbar = false;

            _notifyIcon = new NotifyIcon();
            Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/icon.ico")).Stream;
            _notifyIcon.Icon = new Icon(iconStream);
            _notifyIcon.Visible = true;
            _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            _contextMenuItems = _notifyIcon.ContextMenuStrip.Items;
            _notifyIcon.MouseUp += (_, __) => _notifyIcon.ContextMenuStrip.Show(Control.MousePosition);
            _notifyIcon.BalloonTipClicked += (_, __) => _notifyIcon.ContextMenuStrip.Show(Control.MousePosition);
            CheckForNewTopicsAsync( OnTopicsChecked);
        }

        private async Task CheckForNewTopicsAsync(Action<IEnumerable<List<Topic>>> onTopicsChecked)
        {
            while (true)
            {
                await RefreshLatestTopics(onTopicsChecked);
                await Task.Delay(_configuration.RefreshInterval.Interval);
            }
        }

        private async Task RefreshLatestTopics(Action<IEnumerable<List<Topic>>> onTopicsChecked)
        {
            try
            {
                IEnumerable<List<Topic>> lastTopics = await Task.WhenAll(_forumsToCheck.Select(forum
                    => _topicClient.GetLastTopicsByLastPostCreatedAt(LastTopicsToGetCount, forum.Id)));
                onTopicsChecked?.Invoke(lastTopics);
            }
            catch (Exception)
            {
                _notifyIcon.ShowBalloonTip(0, "Błąd sieciowy", "Błąd pobierania nowych tematów na forum", ToolTipIcon.Error);
            }
        }

        private async void OnTopicsChecked(IEnumerable<List<Topic>> lastTopicsByForum)
        {
            UpdateRefreshedAtText();
            ClearContextMenu();
            InsertTopContextMenuItems();

            var allTopics = new List<Topic>();
            foreach (List<Topic> lastTopics in lastTopicsByForum)
            {
                if (lastTopics != null && lastTopics.Any())
                {
                    IOrderedEnumerable<Topic> topicsByLastPostCreatedAt = lastTopics.OrderByDescending(topic => topic.LastPostCreatedAt);
                    UpdateContextMenuItems(topicsByLastPostCreatedAt);
                    allTopics.AddRange(topicsByLastPostCreatedAt);
                }
            }
            InsertBottomContextMenuItems();
            await ShowBalloonTipIfAnyNew(allTopics.OrderBy(t => t.LastPostCreatedAt));
        }

        private async Task ShowBalloonTipIfAnyNew(IOrderedEnumerable<Topic> lastTopics)
        {
            IEnumerable<Topic> newTopics = lastTopics.Where(topic => topic.IsNewerThan(DateTime.Now - _configuration.RefreshInterval.Interval));
            if (!string.IsNullOrEmpty(_configuration.User.UserName))
            {
                IEnumerable<Post> newTopicsPosts = await Task.WhenAll(newTopics.Select(async topic => await _postClient.GetPostById(topic.LastPostId)));
                newTopics = newTopics.Where(topic => !newTopicsPosts.Any(post => post.TopicId == topic.Id && post.User.Name == _configuration.User.UserName));
            }

            if (newTopics.Any())
            {
                Topic mostRecent = newTopics.First();
                _notifyIcon.ShowBalloonTip(1, "Nowe posty", mostRecent.Subject, ToolTipIcon.Info);
            }
        }

        private void UpdateContextMenuItems(IOrderedEnumerable<Topic> lastTopics)
        {
            lastTopics.ToList().ForEach(topic =>
            {
                ToolStripItem newItem = _contextMenuItems.Add(TopicPrinter.PrintShort(topic), null, (_, __) => Process.Start(topic.Url));
                if (topic.Replies == 0)
                {
                    Font oldFont = newItem.Font;
                    newItem.Font = new Font(oldFont, System.Drawing.FontStyle.Bold);
                }
            });
            _contextMenuItems.Add(new ToolStripSeparator());
        }

        private void InsertBottomContextMenuItems()
        {
            _contextMenuItems.Add("Zamknij", null, (_, __) => Application.Current.Shutdown());
        }

        private void InsertTopContextMenuItems()
        {
            _contextMenuItems.Add("Odśwież", null, async (_, __) => await RefreshLatestTopics(OnTopicsChecked));
            _contextMenuItems.Add(new ToolStripSeparator());
        }

        private void ClearContextMenu()
        {
            _contextMenuItems.Clear();
        }

        private void UpdateRefreshedAtText()
        {
            _notifyIcon.Text = $"Odświeżono o {DateTime.Now.ToShortTimeString()}";
        }
    }
}
