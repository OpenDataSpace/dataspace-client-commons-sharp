//-----------------------------------------------------------------------
// <copyright file="NativeAccountStore.cs" company="GRAU DATA AG">
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
namespace DataSpace.Common.Settings.Connection.W32 {
    using System;
    using System.Security;

    using DataSpace.Common.Crypto;
    using DataSpace.Common.Settings.Connection.Native;

    public class NativeAccountStore : INativeAccountStore {
        private readonly string applicationName;
        public NativeAccountStore(string applicationName = "DataSpace") {
            if (string.IsNullOrWhiteSpace(applicationName)) {
                if (applicationName == null) {
                    throw new ArgumentNullException("applicationName");
                } else if (applicationName.Equals(string.Empty)) {
                    throw new ArgumentException("Given appName is empty", "applicationName");
                } else {
                    throw new ArgumentException("Given appName contains only whitespaces", "applicationName");
                }
            }

            this.applicationName = applicationName;
        }

        public void Add(string url, string userName, SecureString password) {
            CredentialManager.Delete(GetApplicationName(url, userName));
            CredentialManager.WriteCredential(GetApplicationName(url, userName), userName, password.ConvertToUnsecureString());
        }

        public SecureString Get(string url, string userName) {
            foreach (var cred in CredentialManager.EnumerateCrendentials(GetApplicationName(url, userName) + "*")) {
                if (cred.CredentialType == CredentialType.Generic &&
                    cred.UserName.Equals(userName))
                {
                    return new SecureString().Init(cred.Password);
                }
            }

            return new SecureString();
        }

        public void Remove(string url, string userName) {
            CredentialManager.Delete(GetApplicationName(url, userName));
        }

        private string GetApplicationName(string url, string userName) {
            return string.Format("{0}_{1}@{2}", applicationName, userName, url);
        }
    }
}