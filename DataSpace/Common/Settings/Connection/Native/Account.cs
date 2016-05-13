//-----------------------------------------------------------------------
// <copyright file="Account.cs" company="GRAU DATA AG">
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