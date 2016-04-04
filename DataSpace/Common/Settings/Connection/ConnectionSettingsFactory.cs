//-----------------------------------------------------------------------
// <copyright file="ConnectionSettingsFactory.cs" company="GRAU DATA AG">
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
﻿
namespace DataSpace.Common.Settings.Connection.W32 {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Common.Settings.Connection;
    public class ConnectionSettingsFactory : IConnectionSettingsFactory {
        /// <summary>
        /// Configuration filepath for all filebased shared Configparts
        /// </summary>
        public static string ConfigFilePath { get; set; }
        /// <summary>
        /// the only one AccountSettings object
        /// </summary>
        private static IAccountSettings _AccountSettings;
        private static object AccLock = new object();

        static ConnectionSettingsFactory() {
            ConfigFilePath = ConnectionSettingsFactory.BuildUserConfigPath("GrauData", "DataSpace", "SharedConfig", ConfigurationUserLevel.PerUserRoamingAndLocal);
        }

        public IAccountSettings AccountSettings {
            get {
                return GetAccountSettings();
            }
        }

        public IProxySettings ProxySettings {
            get {
                return GetProxySettings();
            }
        }

        /// <summary>
        /// AccountSettings Factory returns new or cached AccountSettings object
        /// </summary>
        /// <returns>new or cached AccountSettings object</returns>
        private static IAccountSettings GetAccountSettings() {
            if (_AccountSettings == null) {
                lock (AccLock) {
                    if (_AccountSettings == null) {
                        _AccountSettings = new AccountSettingsFactory().CreateInstance("DataSpace@", new ConfigurationLoader(ConfigFilePath).Configuration);
                        _AccountSettings.Load();
                    }
                }
            }

            return _AccountSettings;
        }

        /// <summary>
        /// the only one ProxySettings object
        /// </summary>
        private static ProxySettings _ProxySettings;
        private static object _ProxyLock = new object();
        /// <summary>
        /// ProxySettings Factory returns new or cached ProxySettings object
        /// </summary>
        /// <returns>new or cached ProxySettings object</returns>
        private static IProxySettings GetProxySettings() {
            if (_ProxySettings == null) {
                lock (_ProxyLock) {
                    if (_ProxySettings == null) {
                        _ProxySettings = new ProxySettings() {
                            SectionName = "ProxySettings",
                            GetConfigFilePath = () => { return ConnectionSettingsFactory.ConfigFilePath; }
                        };
                        _ProxySettings.Load();
                    }
                }
            }

            return _ProxySettings;
        }

        /// <summary>
        /// configuration path builder 
        /// </summary>
        /// <param name="Company">Company pathpart</param>
        /// <param name="Product">Produkt pathpart</param>
        /// <param name="FileName">Settings filename w.o. extension</param>
        /// <param name="SettingsType">Type of Settings (all user, local, roaming)</param>
        /// <returns>generated path</returns>
        public static string BuildUserConfigPath(string Company, string Product, string FileName, ConfigurationUserLevel SettingsType) {
            string BasePath = string.Empty;
            switch (SettingsType) {
                case ConfigurationUserLevel.None:
                    BasePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    break;
                case ConfigurationUserLevel.PerUserRoaming:
                    BasePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    break;
                case ConfigurationUserLevel.PerUserRoamingAndLocal:
                    BasePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    break;
                default:
                    break;
            }

            return Path.Combine(BasePath, Company, Product, string.Concat(FileName, ".config"));
        }
    }

    /// <summary>
    /// Persistence Helper class
    /// </summary>
    internal class ConfigurationSectionLoader {
        /// <summary>
        /// the Logger  object
        /// </summary>
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="ConfigFilePath">Full path to Configuration file</param>
        public ConfigurationSectionLoader(string ConfigFilePath) {
            _ConfigFilePath = ConfigFilePath;
        }

        /// <summary>
        /// Reads or Creates ConfigurationSection derived objects
        /// </summary>
        /// <param name="SectionName">Name Tag in <code>Configuration.Sections</code></param>
        /// <param name="StoreSectionType">ConfigurationSection derived Type for load/save operations</param>
        /// <returns></returns>
        public ConfigurationSection GetSection(string SectionName, Type StoreSectionType) {
            // Über ConfigurationManager.OpenMappedExeConfiguration(ExeConfigurationFileMap)
            // können Konfigurationsdateien ins selbst fegeleten Pfaden geladen werden

            // "ExeConfigurationFileMap" verlangt dateipfade in aufsteigender geschlossener Folge für die verschiedenen "ConfigurationUserLevel"
            // 1. ConfigurationUserLevel.None -> ExeConfigFilename
            // 2. ConfigurationUserLevel.PerUserRoaming -> ExeConfigFilename, RoamingUserConfigFilename
            // 3. ConfigurationUserLevel.PerUserRoamingAndLocal -> ExeConfigFilename, RoamingUserConfigFilename, LocalUserConfigFilename

            // die einzelnen Konfigurationsdateien werden dabei gemischt und bilden die dann die auf der angeforderten "ConfigurationUserLevel" Ebene verfügbaren informationen

            // neue Sections können nur auf ConfigurationUserLevel.None angelegt werden
            // Teile unserer Settings (Accountdaten) funktionieren nicht mit roaming -> daher benutzen wir eine lokale Konfiguration im Verzeichnis Environment.SpecialFolder.LocalApplicationData
            // daher die etwas seltsame Kombination aus ExeConfigFilename Property mit Environment.SpecialFolder.LocalApplicationData Pfad und ConfigurationUserLevel.None Zugriff
            // siehe auch default Belegung von ConnectionSettingsFactory.BuildUserConfigPath
            if (_Config == null) {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                // if _ConfigFilePath hdoes not exist, it will be automaticly created at first save operation
                configMap.ExeConfigFilename = _ConfigFilePath;
                _Config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            }

            ConfigurationSection Section = null;
            try {
                // try to get our section -- can except in case of version diff or something else
                // if section does not exist no exception is thrown an returns zero 
                Section = _Config.GetSection(SectionName);
            } catch (ConfigurationErrorsException eConf) {
                _logger.ErrorFormat("{0} -- Failed to load Section '{1}' - Exception: {2}", System.Reflection.MethodBase.GetCurrentMethod().Name, SectionName , eConf.Message);
                // assume it already exists  -> delete it
                _Config.Sections.Remove(SectionName);
            } catch (Exception e) {
                // something else .. log it and eat it
                _logger.ErrorFormat("{0} -- Failed to load Section {1} - Exception: {2}", System.Reflection.MethodBase.GetCurrentMethod().Name, SectionName, e.Message);
            };

            if (Section == null) {
                // Config without our section -> create and add it
                try {
                    Section = (ConfigurationSection)Activator.CreateInstance(StoreSectionType);
                    _Config.Sections.Add(SectionName, Section);
                } catch (Exception e) {
                    _logger.ErrorFormat("{0} -- Failed to create Section {1} - Exception: {2}", System.Reflection.MethodBase.GetCurrentMethod().Name, SectionName, e.Message);
                    throw;
                }
            }

            return Section;
        }

        private string _ConfigFilePath = string.Empty;
        private Configuration _Config = null;

        public Configuration Configuration {
            get { return _Config; }
        }
    }
}