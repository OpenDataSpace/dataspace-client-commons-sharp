
namespace DataSpace.Common.Settings.Connection {
    using System;

    /// <summary>
    /// Proxytype Enumeration
    /// </summary>
    public enum ProxyType {
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
}