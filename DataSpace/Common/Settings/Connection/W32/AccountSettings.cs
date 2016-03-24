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

namespace DataSpace.Common.Settings.Connection.W32 {
    ﻿using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;

    using DataSpace.Common.Crypto;
    using DataSpace.Common.Utils;

    /// <summary>
    /// Read/Store Account information in Windows Credential Store
    /// </summary>
    public class AccountSettings : IAccountSettingsRead, IAccountSettings {
        /// <summary>
        /// the Logger  object
        /// </summary>
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Time of last load/save call
        /// </summary>
        private DateTime _LastRefreshTime = DateTime.Now;
        /// <summary>
        /// Multithreading lock object
        /// </summary>
        private object _Lock = new object();

        private TimeSpan _PropsRefreshSpan = new TimeSpan(0, 2, 0);

        private SecureString _Password = new SecureString();

        // url loaded from credential store -- needed for delete operation when Url changes occure
        private string _LoadedUrl = string.Empty;
        private string _Url = string.Empty;
        private string _UserName = string.Empty;
        private bool _IsDirty = false;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SettingsLoaded = delegate { };
        public event EventHandler SettingsSaved = delegate { };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="UrlPrefix">optional Url Prefix to differentiate Account types</param>
        public AccountSettings(string UrlPrefix = "") {
            this.UrlPrefix = UrlPrefix;
            this.UrlPrefix.Trim();
        }

        /// <summary>
        /// Span between automatic Property refresh from store
        /// on external property reads
        /// ! 0 disables refresh
        /// </summary>
        public TimeSpan PropsRefreshSpan {
            get {
                lock (_Lock) {
                    return _PropsRefreshSpan; 
                }
            }

            set {
                lock (_Lock) {
                    // disable refresh?
                    if (value != new TimeSpan(0)) {
                        //no; minimum 5 seconds
                        value = value > new TimeSpan(0, 0, 5) ? value : new TimeSpan(0, 0, 5);
                    }
                    _PropsRefreshSpan = value;
                }
            }
        }

        /// <summary>
        /// Prefix used for saved Urls
        /// </summary>
        public string UrlPrefix {
            get; private set;
        }

        public SecureString Password {
            get {
                lock (_Lock) {
                    RefreshProps();
                    return _Password.Copy();
                }
            }

            set {
                lock (_Lock) {
                    value = value ?? new SecureString();
                    if (_Password.ConvertToUnsecureString().CompareTo(value.ConvertToUnsecureString()) != 0) {
                        _Password = value.Copy();
                        OnPropertyChanged(Property.NameOf(() => this.Password));
                    }
                }
            }
        }

        public string Url {
            get {
                lock (_Lock) {
                    RefreshProps();
                    return _Url;
                }
            }

            set {
                lock (_Lock) {
                    value = value ?? string.Empty;
                    value.Trim();
                    if (string.Compare(_Url, value) != 0) {
                        _Url = value;
                        OnPropertyChanged(Property.NameOf(() => this.Url));
                    }
                }
            }
        }

        public string UserName {
            get {
                lock (_Lock) {
                    RefreshProps();
                    return _UserName;
                }
            }

            set {
                lock (_Lock) {
                    value = value ?? string.Empty;
                    value.Trim();
                    if (_UserName.CompareTo(value) != 0) {
                        _UserName = value;
                        OnPropertyChanged(Property.NameOf(() => this.UserName));
                    }
                }
            }
        }

        public bool IsDirty {
            get {
                lock (_Lock) {
                    return _IsDirty;
                }
            }

            private set {
                lock (_Lock) {
                    if (_IsDirty != value) {
                        _IsDirty = value;
                        OnPropertyChanged(Property.NameOf(() => this.IsDirty));
                    }
                }
            }
        }

        /// <summary>
        /// Loads Accountinformation from secure OS Credential Store
        /// </summary>
        public void Load() {
            // load all Credentials with matching Prefix
            IList<Credential> AccountsList = CredentialManager.EnumerateCrendentials(UrlPrefix + "*");
            // using local variables -- so we can keep the following lock block short
            bool bFound = false;
            string NewUrl = string.Empty;
            string NewUserName = string.Empty;
            SecureString NewPassword = new SecureString();
            // find first credential with correct prefix and Type -> use it
            foreach (var item in AccountsList) {
                if (item.CredentialType == CredentialType.Generic) {
                    // remove prefix part for easy usage and
                    string NewUrlForTest = item.ApplicationName.Substring(UrlPrefix.Length);
                    // kill 'whitespace only' urls
                    if (string.IsNullOrWhiteSpace(NewUrlForTest)) {
                        CredentialManager.Delete(item.ApplicationName);
                        _logger.WarnFormat("Deleted whitespace corrupted account information -> '{0}'", item.ApplicationName);
                        continue;
                    }

                    // only log additional account informations
                    if (bFound) {
                        _logger.WarnFormat("Found more then one matching account information -> '{0}'", item.ApplicationName);
                        continue;
                    }

                    NewUrl = NewUrlForTest;
                    NewUserName = item.UserName;
                    NewPassword = new SecureString().Init(item.Password);
                    bFound = true;
                    _logger.InfoFormat("Account information loaded for '{0}'", item.ApplicationName);
                }
            }

            lock (_Lock) {
                // update refresh time and Properties
                _LastRefreshTime = DateTime.Now;
                // store loaded url for potential delete operation
                _LoadedUrl = NewUrl;
                // store as new aktive Url
                Url = NewUrl;
                UserName = NewUserName;
                Password = NewPassword;

                // leads potentially to PropertyChanged "IsDirty" == false Event
                IsDirty = false;
            }

            // fire load event
            SettingsLoaded.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Saves Accountinformation to secure OS Credential Store
        /// </summary>
        public void Save() {
            lock (_Lock) {
                // Save if Props were changed
                if (IsDirty) {
                    //difference between stored and object url?
                    if (_LoadedUrl.CompareTo(_Url) != 0 && _LoadedUrl.Length != 0) {
                        // yes; delete first and store then (if we don't we produce a new entry!)
                        CredentialManager.Delete(UrlPrefix + _LoadedUrl);
                    }

                    //new Url empty?
                    if (string.IsNullOrWhiteSpace(_Url)) {
                        //yes;  we already deleted the old entry, so we only have to reset the props
                        Url = string.Empty;
                        UserName = string.Empty;
                        Password = new SecureString();
                        _LoadedUrl = string.Empty;
                    } else {
                        // no; decorate url with Prefix and store data in credential store
                        CredentialManager.WriteCredential(UrlPrefix + _Url, _UserName, _Password.ConvertToUnsecureString());
                        //update stored url as preparation for next url change
                        _LoadedUrl = _Url;
                    }

                    // update the refreh time
                    _LastRefreshTime = DateTime.Now;
                    // Signal  clean saved state
                    IsDirty = false;
                } 
            }

            // fire saved Event
            SettingsSaved.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Deletes Accountinformation from secure OS Credential Store
        /// </summary>
        public void Delete() {
            lock (_Lock) {
                // delete the decoreted Url based entry in OS store 
                // use the _LoadedUrl for deletion! (if we don't, Url may have been changed and not saved and we delete the wrong / nothing)
                CredentialManager.Delete(UrlPrefix + _LoadedUrl);
                //reinitialize our Properties
                Url = string.Empty;
                UserName = string.Empty;
                Password = new SecureString();
                _LoadedUrl = string.Empty;
                // update the refreh time
                _LastRefreshTime = DateTime.Now;

                IsDirty = false; 
            }

            // fire saved Event
            SettingsSaved.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// triggers a load operation if object is not in edit mode and 
        /// and load is more then <c>PropsRefreshSpan</c> ago
        /// </summary>
        private void RefreshProps() {
            lock (_Lock) {
                // is disabled? 
                if (PropsRefreshSpan == new TimeSpan(0)) {
                    return; // yes
                }

                if (IsDirty == false && (DateTime.Now - _LastRefreshTime > PropsRefreshSpan)) {
                    Load();
                }
            }
        }

        private void OnPropertyChanged(string property) {
            if (PropertyChanged != null) {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
            }
            //all Property changes should trigger "IsDirty = true" exept "IsDirty" itself 
            if (string.Compare(property, Property.NameOf(() => this.IsDirty)) != 0) {
                IsDirty = true;
            }
        }
    }
}