
namespace DataSpace.Common.Settings.Connection.Generic {
    using System;
    using System.Configuration;
    using System.Security;

    using DataSpace.Common.Crypto;
    using DataSpace.Common.Utils;

    public class AccountSettingsSection : ConfigurationSection {
        [ConfigurationProperty("Url", DefaultValue = "", IsRequired = true)]
        public string Url {
            get { return (string)this[Property.NameOf(() => this.Url)]; }
            set { this [Property.NameOf(() => this.Url)] = value; }
        }

        [ConfigurationProperty("UserName", DefaultValue = "", IsRequired = true)]
        public string UserName {
            get { return (string)this[Property.NameOf(() => this.UserName)]; }
            set { this [Property.NameOf(() => this.UserName)] = value; }
        }

        [ConfigurationProperty("Password", IsRequired = true)]
        public SecureString Password {
            get {
                var obfuscatedPassword = (string)this[Property.NameOf(() => this.Password)];
                return new SecureString().Init(obfuscatedPassword == null ? obfuscatedPassword : obfuscatedPassword.Deobfuscate());
            }

            set { this [Property.NameOf(() => this.Password)] = value.ConvertToUnsecureString().Obfuscate(); }
        }

    }
}