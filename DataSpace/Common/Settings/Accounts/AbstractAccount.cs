//-----------------------------------------------------------------------
// <copyright file="AbstractAccount.cs" company="GRAU DATA AG">
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
namespace DataSpace.Common.Settings.Accounts {
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Security;

    using Utils;

    public abstract class AbstractAccount : ConfigurationSection, IAccountReadOnly, IAccount {
        [ConfigurationProperty("Url", DefaultValue = "", IsRequired = true)]
        public string Url {
            get { return (string)this[Property.NameOf(() => Url)]; }
            set { this[Property.NameOf(() => Url)] = value; }
        }

        [ConfigurationProperty("UserName", DefaultValue = "", IsRequired = true)]
        public string UserName {
            get { return (string)this[Property.NameOf(() => UserName)]; }
            set { this[Property.NameOf(() => UserName)] = value; }
        }

        public virtual string Id {
            get {
                return string.Format("{0} @ {1}", UserName, Url);
            }
        }

        public abstract SecureString Password { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SettingsDeleted;

        protected void OnPropertyChanged(string property) {
            if (PropertyChanged != null) {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }

        public virtual void Delete() {
            if (SettingsDeleted != null) {
                SettingsDeleted.Invoke(this, new EventArgs());
            }
        }
    }
}