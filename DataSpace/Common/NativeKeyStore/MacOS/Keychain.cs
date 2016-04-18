//-----------------------------------------------------------------------
// <copyright file="Keychain.cs" company="GRAU DATA AG">
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
namespace DataSpace.Common.NativeKeyStore.MacOS {
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Text;

    using MonoMac.Security;

    public class Keychain : IDictionary<string, string> {
        private readonly string serviceName;

        public Keychain(string appName = "DataSpace") {
            if (string.IsNullOrWhiteSpace(appName)) {
                if (appName == null) {
                    throw new ArgumentNullException("appName");
                } else if (appName.Equals(string.Empty)) {
                    throw new ArgumentException("Given appName is empty", "appName");
                } else {
                    throw new ArgumentException("Given appName contains only whitespaces", "appName");
                }
            }

            serviceName = appName;
        }

        public bool IsReadOnly {
            get {
                return false;
            }
        }

        public ICollection<string> Keys {
            get {
                var results = new List<string>();
                SecStatusCode status;
                using (var query = new SecRecord(SecKind.GenericPassword) { Label = serviceName }) {
                    var records = SecKeyChain.QueryAsRecord(query, 1000, out status);
                    if (status == SecStatusCode.ItemNotFound) {
                        return results;
                    }

                    status.AndThrowExceptionOnFailure();
                    foreach (var item in records) {
                        results.Add(item.Account);
                        item.Dispose();
                    }
                }

                return results;
            }
        }

        public ICollection<string> Values {
            get {
                var results = new List<string>();
                SecStatusCode status;
                using (var query = new SecRecord(SecKind.GenericPassword) { Label = serviceName }) {
                    var records = SecKeyChain.QueryAsRecord(query, 1000, out status);
                    if (status == SecStatusCode.ItemNotFound) {
                        return results;
                    }

                    status.AndThrowExceptionOnFailure();
                    foreach (var item in records) {
                        results.Add(this [item.Account]);
                        item.Dispose();
                    }
                }

                return results;

            }
        }

        public string this [string key] {
            get {
                byte[] pw;
                var status = SecKeyChain.FindGenericPassword(serviceName, key, out pw);
                switch (status) {
                    case SecStatusCode.Success:
                        return Encoding.UTF8.GetString(pw);
                    case SecStatusCode.ItemNotFound:
                        return null;
                    default:
                        status.AndThrowExceptionOnFailure();
                        return null;
                }
            }
            set {
                if (Contains(key)) {
                    Remove(key);
                }

                Add(key, value);
            }
        }

        public void Add(string key, string value) {
            SecKeyChain.AddGenericPassword(serviceName, key, Encoding.UTF8.GetBytes(value)).AndThrowExceptionOnFailure();
        }

        public void Clear() {
            foreach (var key in Keys) {
                Remove(key);
            }
        }

        public bool Contains(string key) {
            byte[] pw;
            var status = SecKeyChain.FindGenericPassword(serviceName, key, out pw);
            switch (status) {
                case SecStatusCode.Success:
                    return true;
                case SecStatusCode.ItemNotFound:
                    return false;
                default:
                    status.AndThrowExceptionOnFailure();
                    return false;
            }
        }

        public bool Remove(string key) {
            try {
                using (var query = new SecRecord(SecKind.GenericPassword) { Account = key, Label = serviceName }) {
                    SecKeyChain.Remove(query).AndThrowExceptionOnFailure();
                }

                return true;
            } catch (ArgumentException) {
                return false;
            }
        }

        public bool ContainsKey(string key) {
            return Contains(key);
        }

        public int Count {
            get {
                return Keys.Count;
            }
        }

        public bool TryGetValue(string key, out string value) {
            if (Contains(key)) {
                value = this [key];
                return true;
            } else {
                value = null;
                return false;
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
            var results = new Dictionary<string, string>();
            SecStatusCode status;
            var query = new SecRecord(SecKind.GenericPassword) { Label = serviceName };
            var records = SecKeyChain.QueryAsRecord(query, 1000, out status);
            if (status == SecStatusCode.ItemNotFound) {
                return results.GetEnumerator();
            }

            status.AndThrowExceptionOnFailure();
            foreach (var item in records) {
                results.Add(item.Account, this[item.Account]);
            }

            return results.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public void Add(KeyValuePair<string, string> item) {
            Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<string, string> item) {
            if (Contains(item.Key)) {
                var entry = this [item.Key];
                return string.Equals(entry, item.Value);
            } else {
                return false;
            }
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) {
            foreach (var entry in this) {
                array [arrayIndex] = new KeyValuePair<string, string>(entry.Key, entry.Value);
                arrayIndex++;
            }
        }

        public bool Remove(KeyValuePair<string, string> item) {
            return Remove(item.Key);
        }
    }

    internal static class KeyChainExceptionExtension {
        public static void AndThrowExceptionOnFailure(this SecStatusCode status) {
            if (status != SecStatusCode.Success) {
                throw new ArgumentException(status.ToString());
            }
        }
    }
}