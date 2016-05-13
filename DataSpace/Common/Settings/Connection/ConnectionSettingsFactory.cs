//-----------------------------------------------------------------------
// <copyright file="ConnectionSettingsFactory.cs" company="GRAU DATA AG">
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
namespace DataSpace.Common.Settings.Connection.W32 {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Common.Settings.Connection;
    public class ConnectionSettingsFactory : IConnectionSettingsFactory {
        /// <summary>
        /// Configuration filepath for all filebased shared Configparts
        /// </summary>
        public static IUserConfigPathBuilder ConfigFilePath { get; set; }
        /// <summary>
        /// the only one AccountSettings object
        /// </summary>
        private static IAccountSettings[] accounts;
        private static object AccLock = new object();

        static ConnectionSettingsFactory() {
            ConfigFilePath = new UserConfigPathBuilder{ Company = "GrauData", Product = "DataSpace", FileName = "SharedConfig", SettingsType = ConfigurationUserLevel.PerUserRoamingAndLocal };
        }

        public IProxySettings ProxySettings {
            get {
                return GetProxySettings();
            }
        }

        /// <summary>
        /// the only one ProxySettings object
        /// </summary>
        private static ProxySettings _ProxySettings;
        private static object _ProxyLock = new object();
        /// <summary>
        /// ProxySettings Factory returns new or cached ProxySettings object
        /// </summary>
        /// <returns>new or cached ProxySettings object</returns>
        private static IProxySettings GetProxySettings() {
            if (_ProxySettings == null) {
                lock (_ProxyLock) {
                    if (_ProxySettings == null) {
                        _ProxySettings = new ProxySettings(new ConfigurationLoader(ConfigFilePath).Configuration) {
                            SectionName = "ProxySettings"
                        };
                        _ProxySettings.Load();
                    }
                }
            }

            return _ProxySettings;
        }

        public IDictionary<string, IAccountSettings> DataSpaceAccounts {
            get {
                lock(AccLock) {
                    return new ConfigurationLoader(ConfigFilePath).Configuration.GetDataSpaceAccounts();
                }
            }
        }
    }
}