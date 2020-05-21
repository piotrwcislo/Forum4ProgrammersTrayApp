using System.Configuration;

namespace Forum4Programmers.TrayApp.Configuration
{
    internal class UserConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("username", IsRequired = false, DefaultValue = "")]
        public string UserName
        {
            get => (string)base["username"];
            set => base["username"] = value;
        }
    }
}
