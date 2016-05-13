
namespace DataSpace.Common.NativeKeyStore {
    using System;
    using System.Collections.Generic;

    using DataSpace.Common.Settings.Connection;
    using DataSpace.Common.Settings.Connection.Native;

    public class Register {
        private static readonly Dictionary<PlatformID, INativeAccountStore> registry = new Dictionary<PlatformID, INativeAccountStore>();
        static Register() {
            var windowsStore = new DataSpace.Common.Settings.Connection.W32.NativeAccountStore();
            registry[PlatformID.Win32Windows] = windowsStore;
            registry[PlatformID.Win32NT] = windowsStore;
            registry[PlatformID.Win32S] = windowsStore;
            registry[PlatformID.WinCE] = windowsStore;
        }

        public INativeAccountStore GetAccountStoreFor(PlatformID platform) {
            return registry[platform];
        }
    }
}