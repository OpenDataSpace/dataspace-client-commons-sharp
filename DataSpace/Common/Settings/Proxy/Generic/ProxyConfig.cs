//-----------------------------------------------------------------------
// <copyright file="ProxyConfig.cs" company="GRAU DATA AG">
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

namespace DataSpace.Common.Settings.Proxy.Generic {
    using System.Configuration;
    using System.Security;

    using Crypto;
    using Utils;

    public class ProxyConfig : AbstractProxyConfig {
        [ConfigurationProperty("Password", DefaultValue = "", IsRequired = true)]
        public string StoredPassword {
            get {
                var obfuscatedPassword = (string)this[Property.NameOf(() => Password)];
                return string.IsNullOrEmpty(obfuscatedPassword) ? string.Empty : obfuscatedPassword.Deobfuscate();
            }

            set { this[Property.NameOf(() => Password)] = value.Obfuscate(); }
        }

        public override SecureString Password {
            get {
                return new SecureString().Init(StoredPassword);
            }

            set {
                var unsecureString = value != null ? value.ConvertToUnsecureString() : string.Empty;
                if (!StoredPassword.Equals(unsecureString)) {
                    StoredPassword = unsecureString;
                    OnPropertyChanged(Property.NameOf(() => Password));
                }
            }
        }

        public override void Delete() {
            StoredPassword = string.Empty;
            base.Delete();
        }
    }
}