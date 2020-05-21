using System.Configuration;

namespace Forum4Programmers.TrayApp.Configuration
{
    internal class ForumConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("id", IsRequired = true, IsKey = true)]
        public int Id
        {
            get => (int)this["id"];
            set => this["id"] = value;
        }

        [ConfigurationProperty("name", IsRequired = true, IsKey = false)]
        public string Name
        {
            get => (string)this["name"];
            set => this["name"] = value;
        }

        [ConfigurationProperty("enabled", IsRequired = true, IsKey = false, DefaultValue = false)]
        public bool Enabled
        {
            get => (bool)this["enabled"];
            set => this["enabled"] = value;
        }

        public ForumConfigElement()
        {
        }

        public ForumConfigElement(string name, int id, bool enabled)
        {
            Name = name;
            Id = id;
            Enabled = enabled;
        }

        public override string ToString()
            => $"{Name}({Id}): {Enabled}";
    }
}
