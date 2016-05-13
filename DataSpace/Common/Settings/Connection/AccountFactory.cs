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
    /// Account factory is used to initialize platform depedend IAccount instances.
    /// </summary>
    public class AccountFactory : IAccountFactory {
        public AbstractAccount CreateInstance(string url, string userName, SecureString password) {
            switch (Environment.OSVersion.Platform) {
                case PlatformID.WinCE:
                    goto case PlatformID.Win32Windows;
                case PlatformID.Win32S:
                    goto case PlatformID.Win32Windows;
                case PlatformID.Win32NT:
                    goto case PlatformID.Win32Windows;
                case PlatformID.Win32Windows:
                    return new DataSpace.Common.Settings.Connection.Native.Account() {
                        Url = url,
                        UserName = userName,
                        Password = password
                    };
                default:
                    return new DataSpace.Common.Settings.Connection.Generic.Account() {
                        Url = url,
                        UserName = userName,
                        Password = password
                    };
            }
        }
    }
}