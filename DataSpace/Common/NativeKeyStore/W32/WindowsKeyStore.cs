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

    using DataSpace.Common.Settings.Connection.W32;

    public class WindowsKeyStore : NativeKeyStore {
        public WindowsKeyStore(string appName) : base(appName) {
        }

        public override ICollection<string> Keys {
            get {
                var results = new List<string>();
                foreach(var entry in this) {
                    results.Add(entry.Key);
                }

                return results;
            }
        }

        public override ICollection<string> Values {
            get {
                var results = new List<string>();
                foreach(var entry in this) {
                    results.Add(entry.Value);
                }

                return results;
            }
        }

        public override string this [string key] {
            get {
                throw new NotSupportedException();
            }
            set {
                throw new NotSupportedException();
            }
        }

        public override void Add(string key, string value) {
            CredentialManager.WriteCredential(ApplicationName, key, value);
        }

        public override bool Contains(string key) {
            throw new NotSupportedException();
        }

        public override bool Remove(string key) {
            throw new NotSupportedException();
        }

        public override IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
            IList<Credential> accounts = CredentialManager.EnumerateCrendentials(ApplicationName + "*");
            IList<KeyValuePair<string, string>> results = new List<KeyValuePair<string, string>>();
            foreach (var account in accounts) {
                if (account.CredentialType != CredentialType.Generic || !account.ApplicationName.Equals(ApplicationName)) {
                    continue;
                }

                results.Add(new KeyValuePair<string, string>(account.UserName, account.Password));
            }

            return results.GetEnumerator();
        }
    }
}