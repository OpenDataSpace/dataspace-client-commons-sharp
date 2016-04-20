//-----------------------------------------------------------------------
// <copyright file="KeyStoreSupportsAttribute.cs" company="GRAU DATA AG">
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
namespace DataSpace.Common.NativeKeyStore {
    using System;

    /// <summary>
    /// This attribute shows enables a <see cref="NativeKeyStore"/> to mark which platforms it supports. A creator of a tagged class can check if the actual platform is supported.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class KeyStoreSupportsAttribute : Attribute {
        /// <summary>
        /// Gets the supported platforms of the tagged <see cref="NativeKeyStore"/>.
        /// </summary>
        /// <value>The supported platforms.</value>
        public PlatformID[] Platforms { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSpace.Common.NativeKeyStore.KeyStoreSupportsAttribute"/> class.
        /// </summary>
        /// <param name="platforms">Supported Platforms.</param>
        public KeyStoreSupportsAttribute(params PlatformID[] platforms) {
            Platforms = platforms ?? new PlatformID[0];
        }
    }
}