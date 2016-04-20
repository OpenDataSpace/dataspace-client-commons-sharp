//-----------------------------------------------------------------------
// <copyright file="NativeKeyStoreFactory.cs" company="GRAU DATA AG">
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
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Native key store factory. This can be used to create new instances of the given generic type T.
    /// The factory checks if the type T supports the actual OS, otherwise a creation will fail.
    /// </summary>
    public class NativeKeyStoreFactory<T> : INativeKeyStoreFactory where T : NativeKeyStore {
        /// <summary>
        /// Creates a new instance of the type T if the given type T has been marked by <see cref="KeyStoreSupportsAttribute"/> to support the actual platform.
        /// </summary>
        /// <returns>The instance.</returns>
        /// <param name="args">Arguments needed to create a new instance.</param>
        public NativeKeyStore CreateInstance(params object[] args) {
            try {
                var attributes = typeof(T).GetCustomAttributes(true);
                foreach (var attribute in attributes) {
                    if (attribute is KeyStoreSupportsAttribute) {
                        if (!new List<PlatformID>((attribute as KeyStoreSupportsAttribute).Platforms).Contains(Environment.OSVersion.Platform)) {
                            throw new PlatformNotSupportedException();
                        }
                    }
                }

                return Activator.CreateInstance(typeof(T), args) as NativeKeyStore;
            } catch (TargetInvocationException ex) {
                throw new NotSupportedException(ex.Message, ex);
            }
        }
    }
}