//-----------------------------------------------------------------------
// <copyright file="UserConfigPathBuilder.cs" company="GRAU DATA AG">
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
    using System.IO;
    using System.Configuration;

    public class UserConfigPathBuilder {
        public string Company { get; set; }
        public string Product { get; set; }
        public string FileName { get; set; }
        public ConfigurationUserLevel SettingsType { get; set; }
        public UserConfigPathBuilder() {
            this.Company = "GrauData";
            this.Product = "DataSpace";
            this.FileName = "SharedConfig";
            this.SettingsType = ConfigurationUserLevel.PerUserRoamingAndLocal;
        }

        public string CreatePath() {
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
}