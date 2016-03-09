//-----------------------------------------------------------------------
// <copyright file="IAuthProviderFactory.cs" company="GRAU DATA AG">
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

namespace DataSpace.Authentication {
    using System;

    /// <summary>
    /// Interface for authentication provider factories.
    /// </summary>
    public interface IAuthProviderFactory {
        /// <summary>
        /// Creates the auth provider fitting to the given type.
        /// </summary>
        /// <returns>
        /// The auth provider.
        /// </returns>
        /// <param name='type'>Authentication type.</param>
        IDisposableAuthProvider CreateAuthProvider(AuthenticationType type);
    }
}