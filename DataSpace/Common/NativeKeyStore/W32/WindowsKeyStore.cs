//-----------------------------------------------------------------------
// <copyright file="WindowsKeyStore.cs" company="GRAU DATA AG">
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
namespace DataSpace.Common.NativeKeyStore.W32 {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DataSpace.Common.Settings.Connection.W32;

    /// <summary>
    /// Native windows credential manager wrapper.
    /// On Windows it is not possible to store multiple credentials for one "targetName" or in our terminology: "ApplicationName".
    /// To support multiple accounts for one application, we use the ApplicationName as targetName prefix, concatinated with a Random Guid.
    /// </summary>
    [KeyStoreSupports(PlatformID.Win32NT, PlatformID.Win32S, PlatformID.Win32Windows, PlatformID.WinCE)]
    public class WindowsKeyStore : NativeKeyStore {
        public WindowsKeyStore() : base("DataSpace") {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsKeyStore"/> class.
        /// </summary>
        /// <param name="appName">Application name.</param>
        public WindowsKeyStore(string appName) : base(appName) {
        }

        /// <summary>
        /// Gets the usernames.
        /// </summary>
        /// <value>The keys.</value>
        public override ICollection<string> Keys {
            get {
                var results = new List<string>();
                foreach (var entry in this) {
                    results.Add(entry.Key);
                }

                return results;
            }
        }

        public override ICollection<string> Values {
            get {
                var results = new List<string>();
                foreach (var entry in this) {
                    results.Add(entry.Value);
                }

                return results;
            }
        }

        public override string this [string key] {
            get {
                return getAccounts().FirstOrDefault(a => a.UserName.Equals(key)).Password;
            }

            set {
                if (Contains(key)) {
                    Remove(key);
                }

                Add(key, value);
            }
        }

        public override void Add(string key, string value) {
            if (Contains(key)) {
                throw new ArgumentException(string.Format("Entry with key {} already exists", key));
            }

            CredentialManager.WriteCredential(ApplicationName + "@" + Guid.NewGuid().ToString(), key, value);
        }

        public override bool Contains(string key) {
            return Keys.Contains(key);
        }

        public override bool Remove(string key) {
            var result = false;
            foreach (var account in getAccounts()) {
                if (account.UserName.Equals(key)) {
                    CredentialManager.Delete(account.ApplicationName);
                    result = true;
                }
            }

            return result;
        }

        public override IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
            IList<KeyValuePair<string, string>> results = new List<KeyValuePair<string, string>>();
            foreach (var account in getAccounts()) {
                results.Add(new KeyValuePair<string, string>(account.UserName, account.Password));
            }

            return results.GetEnumerator();
        }

        private IList<Credential> getAccounts() {
            IList<Credential> result = new List<Credential>();
            foreach (var cred in CredentialManager.EnumerateCrendentials(ApplicationName + "@*")) {
                if (cred.CredentialType == CredentialType.Generic && cred.ApplicationName.StartsWith(ApplicationName + "@")) {
                    result.Add(cred);
                }
            }

            return result;
        }
    }
}