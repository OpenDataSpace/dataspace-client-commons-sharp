
namespace DataSpace.Common.Settings.Connection {
    using System;
    using System.Security;

    public class ProxyConfigFactory : IProxyConfigFactory {
        public AbstractProxyConfig CreateInstance() {
            switch (Environment.OSVersion.Platform) {
                case PlatformID.WinCE:
                    goto case PlatformID.Win32Windows;
                case PlatformID.Win32S:
                    goto case PlatformID.Win32Windows;
                case PlatformID.Win32NT:
                    goto case PlatformID.Win32Windows;
                case PlatformID.Win32Windows:
                    return new DataSpace.Common.Settings.Connection.Native.ProxyConfig();
                default:
                    return new DataSpace.Common.Settings.Connection.Generic.ProxyConfig();
            }
        }
    }
}