
namespace DataSpace.Common.Settings.Connection {
    using System.Configuration;
    using Utils;
    public abstract class AbstractAccountSettingsSection : ConfigurationSection {

        [ConfigurationProperty("Id", IsRequired = true)]
        public string Id
        {
            get { return (string)this[Property.NameOf(() => this.Id)]; }
            set { this[Property.NameOf(() => this.Id)] = value; }
        }
    }
}
