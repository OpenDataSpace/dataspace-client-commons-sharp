//-----------------------------------------------------------------------
// <copyright file="DefaultProxy.cs" company="GRAU DATA AG">
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

namespace DataSpace.Common.Proxy {
    using System;
    using System.Net;

    using DataSpace.Common.Crypto;

    /// <summary>
    /// Default Http proxy utils.
    /// </summary>
    public static class DefaultProxy {

        /// <summary>
        /// Sets the default proxy for every HTTP request.
        /// If the caller would like to know if the call throws any exception, the second parameter should be set to true
        /// </summary>
        /// <param name="settings">proxy settings.</param>
        /// <param name="throwExceptions">If set to <c>true</c> throw exceptions.</param>
        public static void SetDefaultProxy(ProxySettings settings) {
            IWebProxy proxy = null;
            switch (settings.Selection) {
                case Type.SYSTEM:
                    proxy = WebRequest.GetSystemWebProxy();
                    break;
                case Type.CUSTOM:
                    if (settings.Server == null) {
                        throw new ArgumentNullException("settings.Server");
                    }

                    proxy = new WebProxy(settings.Server);
                    break;
            }

            if (settings.LoginRequired && proxy != null) {
                if (string.IsNullOrEmpty(settings.Username)) {
                    throw new ArgumentException("username is null or empty");
                }

                proxy.Credentials = new NetworkCredential(settings.Username, settings.ObfuscatedPassword.Deobfuscate());
            }

            WebRequest.DefaultWebProxy = proxy;
        }
    }
}