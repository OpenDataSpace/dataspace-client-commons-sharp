using System;
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
