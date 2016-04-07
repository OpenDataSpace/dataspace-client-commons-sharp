//-----------------------------------------------------------------------
// <copyright file="ProxySettingsFactory.cs" company="GRAU DATA AG">
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
namespace DataSpace.Common.Settings.Connection {
    using System;
    using System.Configuration;

    public class ProxySettingsFactory : IProxySettingsFactory {
        public ProxySettingsFactory() {
        }

        public IProxySettings GetInstance(Configuration config) {
            if (config == null) {
                throw new ArgumentNullException("config");
            }

            switch (Environment.OSVersion.Platform) {
                case PlatformID.Unix:
                    return new Generic.ProxySettings(config);
                case PlatformID.MacOSX:
                    return new MacOS.ProxySettings(config);
                default:
                    return new W32.ProxySettings();
            }
        }
    }
}