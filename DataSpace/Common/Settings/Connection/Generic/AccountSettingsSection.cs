//-----------------------------------------------------------------------
// <copyright file="AccountSettingsSection.cs" company="GRAU DATA AG">
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
﻿
namespace DataSpace.Common.Settings.Connection.Generic {
    using System;
    using System.Configuration;
    using System.Security;

    using DataSpace.Common.Crypto;
    using DataSpace.Common.Utils;

    public class AccountSettingsSection : ConfigurationSection {
        [ConfigurationProperty("Url", DefaultValue = "", IsRequired = true)]
        public string Url {
            get { return (string)this[Property.NameOf(() => this.Url)]; }
            set { this [Property.NameOf(() => this.Url)] = value; }
        }

        [ConfigurationProperty("UserName", DefaultValue = "", IsRequired = true)]
        public string UserName {
            get { return (string)this[Property.NameOf(() => this.UserName)]; }
            set { this [Property.NameOf(() => this.UserName)] = value; }
        }

        [ConfigurationProperty("Password", DefaultValue = "", IsRequired = true)]
        public string Password {
            get {
                var obfuscatedPassword = (string)this[Property.NameOf(() => this.Password)];
                return string.IsNullOrEmpty(obfuscatedPassword) ? string.Empty : obfuscatedPassword.Deobfuscate();
            }

            set { this [Property.NameOf(() => this.Password)] = value.Obfuscate(); }
        }

    }
}