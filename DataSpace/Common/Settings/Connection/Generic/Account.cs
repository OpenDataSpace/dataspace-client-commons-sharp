
namespace DataSpace.Common.Settings.Connection.Generic {
    using System.Configuration;
    using System.Security;

    using DataSpace.Common.Crypto;
    using DataSpace.Common.Utils;

    public class Account : AbstractAccount {
        [ConfigurationProperty("Password", DefaultValue = "", IsRequired = true)]
        public string StoredPassword {
            get {
                var obfuscatedPassword = (string)this[Property.NameOf(() => Password)];
                return string.IsNullOrEmpty(obfuscatedPassword) ? string.Empty : obfuscatedPassword.Deobfuscate();
            }

            set { this[Property.NameOf(() => Password)] = value.Obfuscate(); }
        }

        public override SecureString Password {
            get {
                return new SecureString().Init(StoredPassword);
            }

            set {
                var unsecureString = value != null ? value.ConvertToUnsecureString() : string.Empty;
                if (!StoredPassword.Equals(unsecureString)) {
                    StoredPassword = unsecureString;
                    OnPropertyChanged(Property.NameOf(() => Password));
                }
            }
        }
    }
}