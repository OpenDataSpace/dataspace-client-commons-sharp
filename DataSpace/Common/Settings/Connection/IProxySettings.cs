//-----------------------------------------------------------------------
// <copyright file="IProxySettings.cs" company="GRAU DATA AG">
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
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace DataSpace.Common.Settings.Connection
{
    /// <summary>
    /// Proxytype Enumeration
    /// </summary>
    public enum ProxyType
    {
        /// <summary>
        /// connect without proxy.
        /// </summary>
        None,
        /// <summary>
        /// connect with system proxy
        /// </summary>
        Default,
        /// <summary>
        /// connect with custom proxy
        /// </summary>
        Custom
    }
    /// <summary>
    /// Proxy settings read access
    /// </summary>
    public interface IProxySettingsRead : INotifySettingsChanged
    {
        ProxyType ProxyType { get; }
        string Url { get; }
        bool NeedLogin { get; }
        string UserName { get; }
        SecureString Password { get; }
    }
    /// <summary>
    /// Proxy settings read/write access
    /// </summary>
    public interface IProxySettings : INotifySettingsChanged, ISettingsPersist
    {
        ProxyType ProxyType { get; set; }
        string Url { get; set; }
        bool NeedLogin { get; set; }
        string UserName { get; set; }
        SecureString Password { get; set; }
    }
}
