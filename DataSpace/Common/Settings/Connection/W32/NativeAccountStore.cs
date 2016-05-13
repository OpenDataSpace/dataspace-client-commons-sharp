
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

        public void Add(string url, string userName, SecureString Password) {
            throw new NotImplementedException();
        }
        public SecureString Get(string url, string userName) {
            throw new NotImplementedException();
        }

        public void Remove(string url, string userName) {
            throw new NotImplementedException();
        }
    }
}