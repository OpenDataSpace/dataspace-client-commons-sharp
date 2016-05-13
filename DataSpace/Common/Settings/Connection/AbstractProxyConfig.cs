//-----------------------------------------------------------------------
// <copyright file="AbstractProxyConfigSection.cs" company="GRAU DATA AG">
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

﻿namespace DataSpace.Common.Settings.Connection {
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Security;

    using DataSpace.Common.Utils;

    public abstract class AbstractProxyConfig : ConfigurationSection, IProxySettings, IProxySettingsRead {
        public static readonly string SectionName = "HTTP.Proxy";
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SettingsDeleted;

        /// <summary>
        /// Proxy Type
        /// </summary>
        [ConfigurationProperty("ProxyType", DefaultValue = ProxyType.Default, IsRequired = true)]
        public ProxyType ProxyType {
            get { return (ProxyType)this[Property.NameOf(() => ProxyType)]; }
            set {
                this [Property.NameOf(() => ProxyType)] = value;
                OnPropertyChanged(Property.NameOf(() => ProxyType));
            }
        }

        /// <summary>
        /// Are credentials required to use custom proxy
        /// </summary>
        [ConfigurationProperty("NeedLogin", DefaultValue = false, IsRequired = false)]
        public bool NeedLogin {
            get { return (bool)this[Property.NameOf(() => NeedLogin)]; }
            set {
                this [Property.NameOf(() => NeedLogin)] = value;
                OnPropertyChanged(Property.NameOf(() => NeedLogin));
            }
        }

        [ConfigurationProperty("Url", DefaultValue = "", IsRequired = true)]
        public string Url {
            get { return (string)this[Property.NameOf(() => Url)]; }
            set {
                this[Property.NameOf(() => Url)] = value;
                OnPropertyChanged(Property.NameOf(() => Url));
            }
        }

        [ConfigurationProperty("UserName", DefaultValue = "", IsRequired = true)]
        public string UserName {
            get { return (string)this[Property.NameOf(() => UserName)]; }
            set {
                this[Property.NameOf(() => UserName)] = value;
                OnPropertyChanged(Property.NameOf(() => UserName));
            }
        }

        public abstract SecureString Password { get; set; }

        protected void OnPropertyChanged(string property) {
            if (PropertyChanged != null) {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }

        public virtual void Delete() {
            NeedLogin = false;
            ProxyType = ProxyType.Default;
            Url = string.Empty;
            UserName = string.Empty;

            if (SettingsDeleted != null) {
                SettingsDeleted.Invoke(this, new EventArgs());
            }
        }
    }
}