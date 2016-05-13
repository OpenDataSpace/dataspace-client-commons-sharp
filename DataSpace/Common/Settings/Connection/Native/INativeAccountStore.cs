
namespace DataSpace.Common.Settings.Connection.Native {
    using System.Security;
    public interface INativeAccountStore {
        void Add(string url, string userName, SecureString Password);
        SecureString Get(string url, string userName);
        void Remove(string url, string userName);
    }
}