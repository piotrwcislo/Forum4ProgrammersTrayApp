using Forum4Programmers.Client;
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
using Application = System.Windows.Application;

namespace Forum4Programmers.UI
{
    public partial class MainWindow : Window
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly ToolStripItemCollection _contextMenuItems;
        private readonly TopicClient _client = new TopicClient();
        private TimeSpan _checkInterval = TimeSpan.FromMinutes(15);

        public MainWindow()
        {
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
            CheckForNewTopicsAsync(_checkInterval, OnTopicsChecked);
        }

        private async Task CheckForNewTopicsAsync(TimeSpan checkInterval, Action<IEnumerable<Topic>> onTopicsChecked)
        {
            while (true)
            {
                await RefreshLatestTopics(onTopicsChecked);
                await Task.Delay(checkInterval);
            }
        }

        private async Task RefreshLatestTopics(Action<IEnumerable<Topic>> onTopicsChecked)
        {
            try
            {
                List<Topic> lastTopics = await _client.GetLatestTopics(10, forumId: 52);
                onTopicsChecked?.Invoke(lastTopics);
            }
            catch (Exception)
            {
                _notifyIcon.ShowBalloonTip(0, "Błąd sieciowy", "Błąd pobierania nowych tematów na forum", ToolTipIcon.Error);
            }
        }

        private void OnTopicsChecked(IEnumerable<Topic> lastTopics)
        {
            UpdateRefreshedAtText();
            if (lastTopics != null && lastTopics.Count() > 0)
            {
                var topicsByLastPostCreatedAt = lastTopics.OrderByDescending(topic => topic.LastPostCreatedAt);
                ShowBalloonTipIfAnyNew(topicsByLastPostCreatedAt);
                UpdateContextMenuItems(topicsByLastPostCreatedAt);
            }
        }

        private void UpdateRefreshedAtText()
        {
            _notifyIcon.Text = $"Odświeżono o {DateTime.Now.ToShortTimeString()}";
        }

        private void ShowBalloonTipIfAnyNew(IOrderedEnumerable<Topic> lastTopics)
        {
            if (lastTopics.Any(topic => (DateTime.Now - topic.LastPostCreatedAt) < _checkInterval))
            {
                Topic mostRecent = lastTopics.First();
                _notifyIcon.ShowBalloonTip(1, "Nowe posty", mostRecent.Subject, ToolTipIcon.Info);
            }
        }

        private void UpdateContextMenuItems(IOrderedEnumerable<Topic> lastTopics)
        {
            _contextMenuItems.Clear();
            _contextMenuItems.Add("Odśwież", null, async (_, __) => await RefreshLatestTopics(OnTopicsChecked));
            _contextMenuItems.Add(new ToolStripSeparator());
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
            _contextMenuItems.Add("Zamknij", null, (_, __) => Application.Current.Shutdown());
        }
    }
}
