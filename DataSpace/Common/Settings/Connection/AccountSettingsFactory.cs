//-----------------------------------------------------------------------
// <copyright file="AccountSettingsFactory.cs" company="GRAU DATA AG">
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
﻿namespace DataSpace.Common.Settings.Connection {
    using System;
    public class AccountSettingsFactory : IAccountSettingsFactory {
        private IAccountSettings account;
        public AccountSettingsFactory(string urlPrefix = "") {
            var os = Environment.OSVersion;
            switch (os.Platform) {
                case PlatformID.Unix:
                    account = new Generic.AccountSettings();
                    break;
                case PlatformID.MacOSX:
                    account = new MacOS.AccountSettings(urlPrefix);
                    break;
                default:
                    account = new W32.AccountSettings(urlPrefix);
                    break;
            }
        }

        public IAccountSettings AccountSettings {
            get {
                return this.account;
            }
        }
    }
}