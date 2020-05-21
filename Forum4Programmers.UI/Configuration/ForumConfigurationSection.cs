using System.Configuration;

namespace Forum4Programmers.TrayApp.Configuration
{
    class ForumConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("Forums", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(ForumsCollection), AddItemName = "Forum")]
        public ForumsCollection Forums
        {
            get => (ForumsCollection)base["Forums"];
            set => Forums = value;
        }

        [ConfigurationProperty("User")]
        public UserConfigElement User
        {
            get => (UserConfigElement)base["User"];
            set => User = value;
        }

        [ConfigurationProperty("RefreshInterval")]
        public RefreshIntervalConfigElement RefreshInterval
        {
            get => (RefreshIntervalConfigElement)base["RefreshInterval"];
            set => RefreshInterval = value;
        }

        public static ForumConfigurationSection GetForumConfiguration() => (ForumConfigurationSection)ConfigurationManager.GetSection("ForumConfiguration");
    }
}
