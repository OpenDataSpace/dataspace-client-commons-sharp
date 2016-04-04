//-----------------------------------------------------------------------
// <copyright file="AccountSettings.cs" company="GRAU DATA AG">
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

namespace DataSpace.Common.Settings.Connection.Generic {
    ﻿using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Security;

    using DataSpace.Common.Crypto;
    using DataSpace.Common.Utils;

    /// <summary>
    /// Read/Store Account information in Windows Credential Store
    /// </summary>
    public class AccountSettings : ConfigurationSection, IAccountSettingsRead, IAccountSettings {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SettingsLoaded = delegate { };
        public event EventHandler SettingsSaved = delegate { };

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

        [ConfigurationProperty("Password", IsRequired = true)]
        public SecureString Password {
            get { return (SecureString)this[Property.NameOf(() => this.Password)]; }
            set { this [Property.NameOf(() => this.Password)] = value; }
        }

        public AccountSettings() {
        }

        public bool IsDirty {
            get {
                return this.IsModified();
            }
        }

        public void Load() {
        }

        public void Save() {
        }

        public void Delete() {}
    }
}