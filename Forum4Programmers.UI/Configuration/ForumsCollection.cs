using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Forum4Programmers.TrayApp.Configuration
{
    internal class ForumsCollection : ConfigurationElementCollection, IEnumerable<ForumConfigElement>
    {
        public override ConfigurationElementCollectionType CollectionType
            => ConfigurationElementCollectionType.AddRemoveClearMap;

        protected override ConfigurationElement CreateNewElement()
            => new ForumConfigElement();

        protected override object GetElementKey(ConfigurationElement element)
            => ((ForumConfigElement)element).Id;

        public new IEnumerator<ForumConfigElement> GetEnumerator()
            => Enumerable.Range(0, Count).Select(i => (ForumConfigElement)BaseGet(i)).GetEnumerator();
    }
}
