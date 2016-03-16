using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSpace.Common.Settings.Connection
{
    public interface ISettingsPersist
    {
        void Load();
        void Save();
        void Delete();
        /// <summary>
        /// flag indicates unsaved changes
        /// </summary>
        bool IsDirty { get; }
    }
    /// <summary>
    /// Settings Change Notifications
    /// </summary>
    public interface INotifySettingsChanged : INotifyPropertyChanged
    {
        event EventHandler SettingsLoaded;
        event EventHandler SettingsSaved;
    }

}
