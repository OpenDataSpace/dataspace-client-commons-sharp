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
    /// Read/Store Account information Configuration file.
    /// </summary>
    public class AccountSettings : IAccountSettingsRead, IAccountSettings {
        private Configuration parent;
        private string configPath;
        private bool isDirty;
        private AccountSettingsSection section;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SettingsLoaded = delegate { };
        public event EventHandler SettingsSaved = delegate { };

        public AccountSettings(string urlPrefix, Configuration parent) {
            if (parent == null) {
                throw new ArgumentNullException("parent");
            }

            this.parent = parent;
            this.configPath = urlPrefix;
        }

        public bool IsDirty {
            get {
                return this.isDirty;
            }

            private set {
                if (this.isDirty != value) {
                    this.isDirty = value;
                    OnPropertyChanged(Property.NameOf(() => this.IsDirty));
                }
            }
        }

        public string Id {
            get {
                return section.Id;
            }

            set {
                if (!this.section.Id.Equals(value)) {
                    this.section.Id = value;
                    OnPropertyChanged(Property.NameOf(() => this.Id));
                }
            }
        }

        public string Url {
            get {
                return this.section.Url;
            }

            set {
                if (!this.section.Url.Equals(value)) {
                    this.section.Url = value;
                    OnPropertyChanged(Property.NameOf(() => this.Url));
                }
            }
        }

        public string UserName {
            get {
                return section.UserName;
            }

            set {
                if (!this.section.UserName.Equals(value)) {
                    this.section.UserName = value;
                    OnPropertyChanged(Property.NameOf(() => this.UserName));
                }
            }
        }

        public SecureString Password {
            get {
                return new SecureString().Init(this.section.Password);
            }

            set {
                if (!this.section.Password.Equals(value.ConvertToUnsecureString())) {
                    this.section.Password = value.ConvertToUnsecureString();
                    OnPropertyChanged(Property.NameOf(() => this.Password));
                }
            }
        }

        public void Load() {
            this.section = this.parent.GetOrCreateSection<AccountSettingsSection>(this.configPath);
            SettingsLoaded.Invoke(this, new EventArgs());
            this.IsDirty = false;
        }

        public void Save() {
            this.parent.Save();
            SettingsSaved.Invoke(this, new EventArgs());
            this.IsDirty = false;
        }

        public void Delete() {
            this.parent.Sections.Remove(this.configPath);
            this.parent.Save();
            this.IsDirty = false;
        }

        private void OnPropertyChanged(string property) {
            if (PropertyChanged != null) {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
            }

            this.IsDirty = true;
        }
    }
}