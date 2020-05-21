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

        public static ForumConfigurationSection GetForumConfiguration() => (ForumConfigurationSection)ConfigurationManager.GetSection("ForumConfiguration");
    }
}
