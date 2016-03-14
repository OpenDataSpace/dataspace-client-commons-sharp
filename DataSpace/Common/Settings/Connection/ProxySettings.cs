using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;


namespace DataSpace.Common.Settings.Connection.W32
{
    /// <summary>
    /// Read/Store DataSpace Proxy information in Windows Credential Store / Config File
    /// </summary>
    public class ProxySettings : IProxySettings, IProxySettingsRead
    {
        /// <summary>
        /// the Logger  object
        /// </summary>
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Constructor
        /// </summary>
        public ProxySettings()
        {
            // catch property change events and relay them 
            // we can do this because we have identical property names
            _ProxyAccount.PropertyChanged += (sender, e) =>
            {
                // filter out "IsDirty"; preventing double notification
                if (string.Compare("IsDirty", e.PropertyName) != 0)
                    OnPropertyChanged(e.PropertyName);
            };
        }
        private TimeSpan _PropsRefreshSpan = new TimeSpan(0, 2, 0);
        /// <summary>
        /// Span between automatic Property refresh from Config file / store
        /// on external property reads
        /// ! 0 disables auto refresh
        /// </summary>
        public TimeSpan PropsRefreshSpan
        {
            get
            {
                lock (_Lock)
                {
                    return _PropsRefreshSpan;
                }
            }

            set
            {
                lock (_Lock)
                {
                    // disable refresh?
                    if (value != new TimeSpan(0))
                    {
                        //no; minimum 5 seconds
                        value = value > new TimeSpan(0, 0, 5) ? value : new TimeSpan(0, 0, 5);
                    }
                    _PropsRefreshSpan = value;
                }
            }
        }
        /// <summary>
        /// Time of last load/save call
        /// </summary>
        private DateTime _LastRefreshTime = DateTime.Now;
        /// <summary>
        /// Multithreading lock object
        /// </summary>
        private object _Lock = new object();
        /// <summary>
        /// Proxy Account login data 
        /// </summary>
        private AccountSettings _ProxyAccount = new AccountSettings("DataSpaceProxy@")
        {
            // disable auto refresh -- we trigger it manually
            PropsRefreshSpan = new TimeSpan(0)
        };
        /// <summary>
        /// delegate -- must retrieve the config file loction
        /// </summary>
        internal Func<string> GetConfigFilePath;
        /// <summary>
        /// Section name in config file
        /// </summary>
        internal string SectionName = "";
        private bool _IsDirty = false;
        public bool IsDirty
        {
            get
            {
                lock (_Lock)
                {
                    return _IsDirty; 
                }
            }
            private set
            {
                lock (_Lock)
                {
                    if (_IsDirty != value)
                    {
                        _IsDirty = value;
                        OnPropertyChanged("IsDirty");
                    } 
                }
            }
        }
        private bool _NeedLogin = false;
        public bool NeedLogin
        {
            get
            {
                lock (_Lock)
                {
                    RefreshProps();
                    return _NeedLogin; 
                }
            }
            set
            {
                lock (_Lock)
                {
                    if (_NeedLogin != value)
                    {
                        _NeedLogin = value;
                        OnPropertyChanged("NeedLogin");
                    } 
                }
            }
        }
        public SecureString Password
        {
            get
            {
                RefreshProps();
                return _ProxyAccount.Password;
            }
            set { _ProxyAccount.Password = value; }
        }

        ProxyType _ProxyType = ProxyType.None;
        public ProxyType ProxyType
        {
            get
            {
                lock (_Lock)
                {
                    RefreshProps();
                    return _ProxyType; 
                }
            }
            set
            {
                lock (_Lock)
                {
                    if (_ProxyType != value)
                    {
                        _ProxyType = value;
                        OnPropertyChanged("ProxyType");
                    } 
                }
            }
        }
        public string Url
        {
            get
            {
                RefreshProps();
                return _ProxyAccount.Url;
            }
            set { _ProxyAccount.Url = value; }
        }
        public string UserName
        {
            get
            {
                RefreshProps();
                return _ProxyAccount.UserName;
            }
            set { _ProxyAccount.UserName = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SettingsLoaded = delegate { };
        public event EventHandler SettingsSaved = delegate { };

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
            //all Property changes trigger "IsDirty = true" exept "IsDirty" itself 
            if (string.Compare(property, "IsDirty") != 0)
                IsDirty = true;
        }
        public void Delete()
        {
            // get the Configfile Section object
            ConfigurationSectionLoader Loader = new ConfigurationSectionLoader(GetConfigFilePath());
            ProxyConfigSection StoredSection = (ProxyConfigSection)Loader.GetSection(SectionName, typeof(ProxyConfigSection));
            lock (_Lock)
            {
                // find our Section in Collection and delete it
                StoredSection.CurrentConfiguration.Sections.Remove(StoredSection.SectionInformation.SectionName);

                // delete the Accountinformation part
                _ProxyAccount.Delete();
                // save changes
                StoredSection.CurrentConfiguration.Save();
                // reinit our properties
                ProxyType = ProxyType.None;
                NeedLogin = false;
                // update refresh time 
                _LastRefreshTime = DateTime.Now;

                IsDirty = false; 
            }

            SettingsSaved.Invoke(this,new EventArgs());
        }

        public void Load()
        {
            // get the Configfile Section object
            ConfigurationSectionLoader Loader = new ConfigurationSectionLoader(GetConfigFilePath());
            ProxyConfigSection StoredSection = (ProxyConfigSection)Loader.GetSection(SectionName,typeof(ProxyConfigSection));

            // using local variables -- so we can keep the following lock block short
            // initialize with fallback values
            ProxyType NewProxyType = ProxyType.None;
            bool NewNeedLogin = false;

            if (StoredSection.ElementInformation.Errors.Count == 0)
            {
                // retrieve the data from Configfile
                NewProxyType = StoredSection.ProxyType;
                NewNeedLogin = StoredSection.NeedLogin;
            }
            else
            {
                _logger.WarnFormat("Found {0} Errors in Configuration.", StoredSection.ElementInformation.Errors.Count);
                foreach (var err in StoredSection.ElementInformation.Errors)
                {
                    _logger.Warn(err.ToString());
                }
                _logger.Warn("Using default Configuration.");
            }


            lock (_Lock)
            {
                // update refresh time and Properties
                _LastRefreshTime = DateTime.Now;
                ProxyType = NewProxyType;
                NeedLogin = NewNeedLogin;
                //  Load the Accountinformation part
                _ProxyAccount.Load();
                IsDirty = false;
            }


            SettingsLoaded.Invoke(this, new EventArgs());
        }

        public void Save()
        {
            // get the Configfile Section object
            ConfigurationSectionLoader Loader = new ConfigurationSectionLoader(GetConfigFilePath());

            ProxyConfigSection StoredSection = (ProxyConfigSection)Loader.GetSection(SectionName, typeof(ProxyConfigSection));
            // store if changes were pending or no config file is present
            if (IsDirty || StoredSection.CurrentConfiguration.HasFile == false)
            {
                // transfer local properties to section object and save Configuration
                lock (_Lock)
                {
                    // use backing field values to prevent autoupdate
                    StoredSection.ProxyType = _ProxyType;
                    StoredSection.NeedLogin = _NeedLogin;
                    StoredSection.CurrentConfiguration.Save();
                    // save the Accountinformation part
                    _ProxyAccount.Save();
                    // update refresh time 
                    _LastRefreshTime = DateTime.Now;

                    IsDirty = false; 
                }
            }
            SettingsSaved.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// triggers a load operation if object is not in edit mode and 
        /// and load is more then <c>PropsRefreshSpan</c> ago
        /// </summary>
        private void RefreshProps()
        {
            lock (_Lock)
            {
                // is disabled? 
                if (PropsRefreshSpan == new TimeSpan(0))
                    return; // yes
                // Don't try loading if config path delegate isn't set
                if (GetConfigFilePath != null &&
                    IsDirty == false && 
                    (DateTime.Now - _LastRefreshTime > PropsRefreshSpan))
                    Load();
            }
        }

    }
}
