using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
            get { return (ProxyType) this["ProxyType"]; }
            set { this ["ProxyType"] = value; }
        }
        /// <summary>
        /// Are credential required to use custom proxy
        /// </summary>
        [ConfigurationProperty("NeedLogin", DefaultValue = false, IsRequired = false)]
        public bool NeedLogin {
            get { return (bool)this["NeedLogin"]; }
            set { this ["NeedLogin"] = value; }
        }
    }
}
