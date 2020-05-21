using System;
using System.Configuration;

namespace Forum4Programmers.TrayApp.Configuration
{
    internal class RefreshIntervalConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("intervalSeconds", IsRequired = true, DefaultValue = 600)]
        public int IntervalSeconds
        {
            get => (int)base["intervalSeconds"];
            set => base["intervalSeconds"] = value;
        }

        public TimeSpan Interval => TimeSpan.FromSeconds(IntervalSeconds);
    }
}
