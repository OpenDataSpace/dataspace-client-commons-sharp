//-----------------------------------------------------------------------
// <copyright file="ProxyConfigSection.cs" company="GRAU DATA AG">
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General private License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//   GNU General private License for more details.
//
//   You should have received a copy of the GNU General private License
//   along with this program. If not, see http://www.gnu.org/licenses/.
//
// </copyright>
//-----------------------------------------------------------------------

namespace DataSpace.Common.Settings.Connection.W32 {
    using System.Configuration;
    using DataSpace.Common.Utils;

    /// <summary>
    /// Define a custom configuration section containing proxy settings
    /// </summary>
    internal class ProxyConfigSection : ConfigurationSection {
        /// <summary>
        /// Proxy Type
        /// </summary>
        [ConfigurationProperty("ProxyType", DefaultValue = ProxyType.None, IsRequired = true)]
        public ProxyType ProxyType {
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

        [ConfigurationProperty("Credentials", DefaultValue = null, IsRequired = false)]
        public string Credentials {
            get { return (string)this[Property.NameOf(() => this.Credentials)]; }
            set { this [Property.NameOf(() => this.Credentials)] = value; }
        }
    }
}