//-----------------------------------------------------------------------
// <copyright file="Register.cs" company="GRAU DATA AG">
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

namespace DataSpace.Common.Settings.NativeKeyStore {
    using System;
    using System.Collections.Generic;

    using Accounts.Native;

    public class Register {
        private static readonly Dictionary<PlatformID, INativeAccountStore> registry = new Dictionary<PlatformID, INativeAccountStore>();
        static Register() {
            var windowsStore = new Accounts.Native.W32.NativeAccountStore();
            registry[PlatformID.Win32Windows] = windowsStore;
            registry[PlatformID.Win32NT] = windowsStore;
            registry[PlatformID.Win32S] = windowsStore;
            registry[PlatformID.WinCE] = windowsStore;
        }

        public INativeAccountStore GetAccountStoreFor(PlatformID platform) {
            return registry[platform];
        }

        public void SetAccountStoreFor(PlatformID platform, INativeAccountStore store) {
            registry[platform] = store;
        }
    }
}