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
    /// Default Http proxy utils and .Net API wrapper to ensure system default proxy usage with NTLM and Kerberos Authentication.
    /// </summary>
    public static class DefaultProxy {
        private static IWebProxy systemDefault;
        private static bool isSystemDefaultSet = false;
        private static object l = new object();

        /// <summary>
        /// Inits the proxy switching support. Must be called before any default proxy manipulation is done.
        /// If this class is the only class, which manipulates the proxy settings, it is not needed to be called.
        /// </summary>
        public static void InitProxySwitchingSupport() {
            if (!isSystemDefaultSet) {
                lock (l) {
                    if (!isSystemDefaultSet) {
                        systemDefault = WebRequest.DefaultWebProxy;
                        isSystemDefaultSet = true;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the system default proxy.
        /// </summary>
        public static void SetSystemDefaultProxy() {
            InitProxySwitchingSupport();
            WebRequest.DefaultWebProxy = systemDefault;
        }

        /// <summary>
        /// Removes all potentially existing proxies and enforces direct internet usage.
        /// </summary>
        public static void SetNoProxy() {
            InitProxySwitchingSupport();
            WebRequest.DefaultWebProxy = null;
        }

        /// <summary>
        /// Sets the given custom proxy as default proxy for all web requests.
        /// </summary>
        /// <param name="to">Custom proxy. Must not be null. Credentials can be null if no authentication is required on the proxy server.</param>
        public static void SetCustomProxy(IWebProxy to) {
            if (to == null) {
                throw new ArgumentNullException("to");
            }

            InitProxySwitchingSupport();
            WebRequest.DefaultWebProxy = to;
        }
    }
}