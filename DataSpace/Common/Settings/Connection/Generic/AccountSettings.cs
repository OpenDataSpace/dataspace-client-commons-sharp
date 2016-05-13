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
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Security;

    using Crypto;
    using Utils;

    /// <summary>
    /// Read/Store Account information Configuration file.
    /// </summary>
    public class AccountSettings : IAccountSettingsRead, IAccountSettings {
        private readonly Configuration config;
        private readonly string sectionName;
        private AccountSettingsSection section;
        private bool isDirty;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SettingsLoaded = delegate { };
        public event EventHandler SettingsSaved = delegate { };
        public event EventHandler SettingsDeleted = delegate { };

        public AccountSettings(Configuration config, AccountSettingsSection section) {
            if (config == null) {
                throw new ArgumentNullException("config");
            }

            if (section == null) {
                throw new ArgumentNullException("section");
            }

            this.config = config;
            this.section = section;
            this.sectionName = section.SectionInformation.SectionName;
            Load();
        }

        public string Id {
            get {
                return string.Format("{0}@{1}", UserName, Url);
            }
        }

        public bool IsDirty {
            get {
                return isDirty;
            }

            private set {
                if (isDirty != value) {
                    isDirty = value;
                    OnPropertyChanged(Property.NameOf(() => IsDirty));
                }
            }
        }

        public string Url {
            get {
                return section.Url;
            }

            private set {
                if (!section.Url.Equals(value)) {
                    section.Url = value;
                    OnPropertyChanged(Property.NameOf(() => Url));
                }
            }
        }

        public string UserName {
            get {
                return section.UserName;
            }

            private set {
                if (!section.UserName.Equals(value)) {
                    section.UserName = value;
                    OnPropertyChanged(Property.NameOf(() => UserName));
                }
            }
        }

        public SecureString Password {
            get {
                return new SecureString().Init(section.Password);
            }

            set {
                if (!section.Password.Equals(value.ConvertToUnsecureString())) {
                    section.Password = value.ConvertToUnsecureString();
                    OnPropertyChanged(Property.NameOf(() => Password));
                }
            }
        }

        public void Load() {
            section = config.GetOrCreateSection<AccountSettingsSection>(section.SectionInformation.SectionName);
            SettingsLoaded.Invoke(this, new EventArgs());
            IsDirty = false;
        }

        public void Save() {
            config.Save();
            SettingsSaved.Invoke(this, new EventArgs());
            IsDirty = false;
        }

        public void Delete() {
            config.Sections.Remove(sectionName);
            config.Save();
            IsDirty = false;
        }

        private void OnPropertyChanged(string property) {
            if (PropertyChanged != null) {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
            }

            IsDirty = true;
        }
    }
}