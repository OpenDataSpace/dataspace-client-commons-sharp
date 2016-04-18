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

    public class Keychain : NativeKeyStore {
        public Keychain(string appName = "DataSpace") : base(appName) {
        }

        public override ICollection<string> Keys {
            get {
                var results = new List<string>();
                SecStatusCode status;
                using (var query = new SecRecord(SecKind.GenericPassword) { Label = ApplicationName }) {
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

        public override ICollection<string> Values {
            get {
                var results = new List<string>();
                SecStatusCode status;
                using (var query = new SecRecord(SecKind.GenericPassword) { Label = ApplicationName }) {
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

        public override string this [string key] {
            get {
                byte[] pw;
                var status = SecKeyChain.FindGenericPassword(ApplicationName, key, out pw);
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

        public override void Add(string key, string value) {
            SecKeyChain.AddGenericPassword(ApplicationName, key, Encoding.UTF8.GetBytes(value)).AndThrowExceptionOnFailure();
        }

        public override bool Contains(string key) {
            byte[] pw;
            var status = SecKeyChain.FindGenericPassword(ApplicationName, key, out pw);
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

        public override bool Remove(string key) {
            try {
                using (var query = new SecRecord(SecKind.GenericPassword) { Account = key, Label = ApplicationName }) {
                    SecKeyChain.Remove(query).AndThrowExceptionOnFailure();
                }

                return true;
            } catch (ArgumentException) {
                return false;
            }
        }

        public override IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
            var results = new Dictionary<string, string>();
            SecStatusCode status;
            var query = new SecRecord(SecKind.GenericPassword) { Label = ApplicationName };
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
    }

    internal static class KeyChainExceptionExtension {
        public static void AndThrowExceptionOnFailure(this SecStatusCode status) {
            if (status != SecStatusCode.Success) {
                throw new ArgumentException(status.ToString());
            }
        }
    }
}