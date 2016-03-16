using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace DataSpace.Common.Settings.Connection
{
    /// <summary>
    /// Account read access
    /// </summary>
    public interface IAccountSettingsRead : INotifySettingsChanged
    {
        string Url { get; }
        string UserName { get;}
        SecureString Password { get;}
    }

    /// <summary>
    /// Account read/write access
    /// </summary>
    public interface IAccountSettings  : INotifySettingsChanged, ISettingsPersist
    {
        string Url { get; set; }
        string UserName { get; set; }
        SecureString Password { get; set; }
    }
}
