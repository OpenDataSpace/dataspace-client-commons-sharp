//-----------------------------------------------------------------------
// <copyright file="ProxySettings.cs" company="GRAU DATA AG">
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
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;

    using DataSpace.Common.Utils;
    using DataSpace.Common.Crypto;

    /// <summary>
    /// Read/Store DataSpace Proxy information in Windows Credential Store / Config File
    /// </summary>
    public class ProxySettings : IProxySettings, IProxySettingsRead {
        private readonly Configuration config;
        private ProxyConfigSection section;
        /// <summary>
        /// Constructor
        /// </summary>
        public ProxySettings(Configuration config) {
            if (config == null) {
                throw new ArgumentNullException("config");
            }

            this.config = config;
            Load();
        }

        /// <summary>
        /// Multithreading lock object
        /// </summary>
        private object l = new object();

        /// <summary>
        /// Section name in config file
        /// </summary>
        private bool isDirty = false;
        public bool IsDirty {
            get {
                lock (l) {
                    return isDirty;
                }
            }

            private set {
                lock (l) {
                    if (isDirty != value) {
                        isDirty = value;
                        OnPropertyChanged(Property.NameOf(() => IsDirty));
                    }
                }
            }
        }

        public bool NeedLogin {
            get {
                lock (l) {
                    return section.NeedLogin;
                }
            }

            set {
                lock (l) {
                    if (section.NeedLogin != value) {
                        section.NeedLogin = value;
                        OnPropertyChanged(Property.NameOf(() => NeedLogin));
                    }
                }
            }
        }

        public SecureString Password {
            get {
                lock (l) {
                    return new SecureString().Init(section.Password);
                }
            }

            set {
                lock (l) {
                    if (!section.Password.Equals(value.ConvertToUnsecureString())) {
                        section.Password = value.ConvertToUnsecureString();
                        OnPropertyChanged(Property.NameOf(() => Password));
                    }
                }
            }
        }

        public ProxyType ProxyType {
            get {
                lock (l) {
                    return section.ProxyType;
                }
            }

            set {
                lock (l) {
                    if (section.ProxyType != value) {
                        section.ProxyType = value;
                        OnPropertyChanged(Property.NameOf(() => ProxyType));
                    }
                }
            }
        }

        public string Url {
            get {
                lock (l) {
                    return section.Url;
                }
            }

            set {
                lock (l) {
                    if (!section.Url.Equals(value)) {
                        section.Url = value;
                        OnPropertyChanged(Property.NameOf(() => Url));
                    }
                }
            }
        }

        public string UserName {
            get {
                lock (l) {
                    return section.UserName;
                }
            }

            set {
                lock (l) {
                    if (!section.UserName.Equals(value)) {
                        section.UserName = value;
                        OnPropertyChanged(Property.NameOf(() => UserName));
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SettingsLoaded = delegate { };
        public event EventHandler SettingsSaved = delegate { };

        public void Delete() {
            lock (l) {
                config.Sections.Remove(AbstractProxyConfigSection.SectionName);
                config.Save();
            }

            SettingsSaved.Invoke(this, new EventArgs());
        }

        public void Load() {
            lock (l) {
                section = config.GetOrCreateSection<ProxyConfigSection>(AbstractProxyConfigSection.SectionName);
            }

            SettingsLoaded.Invoke(this, new EventArgs());
            IsDirty = false;
        }

        public void Save() {
            lock (l) {
                config.Save();
                SettingsSaved.Invoke(this, new EventArgs());
                IsDirty = false;
            }
        }

        private void OnPropertyChanged(string property) {
            if (PropertyChanged != null) {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
            }

            IsDirty = true;
        }
    }
}