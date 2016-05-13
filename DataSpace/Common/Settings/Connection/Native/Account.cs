
namespace DataSpace.Common.Settings.Connection.Native {
    using System;
    using System.Security;

    using DataSpace.Common.Crypto;
    using DataSpace.Common.Utils;

    public class Account : AbstractAccount {
        private readonly INativeAccountStore nativeStore;
        public Account(INativeAccountStore store = null) {
            this.nativeStore = store ?? ConfigurationConvenienceExtender.GetRegisteredStore();
        }

        public override SecureString Password {
            get {
                return nativeStore.Get(Url, UserName) ?? new SecureString();
            }

            set {
                if (!Password.Equals(value.ConvertToUnsecureString())) {
                    nativeStore.Remove(Url, UserName);
                    nativeStore.Add(Url, UserName, value);
                    OnPropertyChanged(Property.NameOf(() => Password));
                }
            }
        }

        public override void Delete() {
            nativeStore.Remove(Url, UserName);
            base.Delete();
        }
    }
}