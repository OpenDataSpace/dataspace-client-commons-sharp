//-----------------------------------------------------------------------
// <copyright file="ConfigurationLoader.cs" company="GRAU DATA AG">
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
namespace DataSpace.Common.Settings {
    using System;
    using System.Configuration;

    /// <summary>
    /// Persistence Helper class
    /// </summary>
    public class ConfigurationLoader {
        /// <summary>
        /// the Logger  object
        /// </summary>
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="ConfigFilePath">Full path to Configuration file</param>
        public ConfigurationLoader(string configFilePath) {
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
            ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
            // if _ConfigFilePath hdoes not exist, it will be automaticly created at first save operation
            configMap.ExeConfigFilename = configFilePath;
            Configuration = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
        }

        public Configuration Configuration {
            get; private set;
        }
    }
}