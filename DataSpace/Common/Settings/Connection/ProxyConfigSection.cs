using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSpace.Common.Utils;

namespace DataSpace.Common.Settings.Connection.W32
{
    /// <summary>
    /// Define a custom configuration section containing proxy settings
    /// </summary>
    internal class ProxyConfigSection : ConfigurationSection
    {
        /// <summary>
        /// Proxy Type 
        /// </summary>
        [ConfigurationProperty("ProxyType",DefaultValue = ProxyType.None,IsRequired = true)]
        public ProxyType ProxyType
        {
            get { return (ProxyType)this[Property.NameOf(() => this.ProxyType)]; }
            set { this [Property.NameOf(() => this.ProxyType)] = value; }
        }
        /// <summary>
        /// Are credentials required to use custom proxy
        /// </summary>
        [ConfigurationProperty("NeedLogin", DefaultValue = false, IsRequired = false)]
        public bool NeedLogin {
            get { return (bool)this[Property.NameOf(() => this.NeedLogin)]; }
            set { this [Property.NameOf(() => this.NeedLogin)] = value; }
        }
    }
}
