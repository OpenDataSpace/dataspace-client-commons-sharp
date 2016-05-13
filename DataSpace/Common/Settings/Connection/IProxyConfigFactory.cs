
namespace DataSpace.Common.Settings.Connection {
    using System.Security;
    public interface IProxyConfigFactory {
        AbstractProxyConfig CreateInstance();
    }
}