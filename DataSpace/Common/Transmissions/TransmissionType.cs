//-----------------------------------------------------------------------
// <copyright file="TransmissionType.cs" company="GRAU DATA AG">
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
namespace DataSpace.Common.Transmissions {
    using System;

    /// <summary>
    /// File transmission types.
    /// </summary>
    public enum TransmissionType {
        /// <summary>
        /// A new file is uploaded
        /// </summary>
        UploadNewFile,

        /// <summary>
        /// A locally modified file is uploaded
        /// </summary>
        UploadModifiedFile,

        /// <summary>
        /// A new remote file is downloaded
        /// </summary>
        DownloadNewFile,

        /// <summary>
        /// A remotely modified file is downloaded
        /// </summary>
        DownloadModifiedFile
    }
}