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
    using System.Configuration;
    using System.Security;

    /// <summary>
    /// Account settings factory is used to initialize platform depedend IAccountSettings instances.
    /// </summary>
    public class AccountSettingsFactory : IAccountSettingsFactory {
        public IAccountSettings CreateInstance(Configuration config, string sectionName, string url, string userName, SecureString password) {
            switch (Environment.OSVersion.Platform) {
                case PlatformID.WinCE:
                    goto case PlatformID.Win32Windows;
                case PlatformID.Win32S:
                    goto case PlatformID.Win32Windows;
                case PlatformID.Win32NT:
                    goto case PlatformID.Win32Windows;
                case PlatformID.Win32Windows:
                    return new W32.AccountSettings(config, sectionName, url, userName, password);
                default:
                    var defaultSection = config.GetOrCreateSection<Generic.AccountSettingsSection>(sectionName);
                    defaultSection.Url = url;
                    defaultSection.UserName = userName;
                    var defaultAccount = LoadInstance(config, defaultSection);
                    defaultAccount.Password = password;
                    return defaultAccount;
            }
        }

        public IAccountSettings LoadInstance(Configuration config, AbstractAccountSettingsSection section) {
            switch (Environment.OSVersion.Platform) {
                case PlatformID.Unix:
                    return new Generic.AccountSettings(config, section as Generic.AccountSettingsSection);
                case PlatformID.MacOSX:
                    throw new NotImplementedException();
                default:
                    return new W32.AccountSettings(config, section as W32.AccountSettingsSection);
            }
        }
    }
}