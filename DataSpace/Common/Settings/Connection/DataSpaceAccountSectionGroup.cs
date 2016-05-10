//-----------------------------------------------------------------------
// <copyright file="DataSpaceAccountSectionGroup.cs" company="GRAU DATA AG">
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
namespace DataSpace.Common.Settings.Connection {
    using System;
    using System.Configuration;

    /// <summary>
    /// Data space account section group contains all available accounts.
    /// </summary>
    public class DataSpaceAccountSectionGroup : ConfigurationSectionGroup {
        /// <summary>
        /// The default name of the section group.
        /// </summary>
        public static readonly string DefaultSectionGroupName = "Accounts";
    }
}