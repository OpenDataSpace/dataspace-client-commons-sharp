//-----------------------------------------------------------------------
// <copyright file="AuthProviderFactory.cs" company="GRAU DATA AG">
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
    using System.Net;

    /// <summary>
    /// Auth provider factory.
    /// </summary>
    public class AuthProviderFactory : IAuthProviderFactory {
        private ICookieStorage cookieStorage;
        private Uri uri;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSpace.Authentication.AuthProviderFactory"/> class.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="cookieStorage">Cookie storage.</param>
        public AuthProviderFactory(Uri url, ICookieStorage cookieStorage) {
            if (url == null) {
                throw new ArgumentNullException("url");
            }

            if (cookieStorage == null) {
                throw new ArgumentNullException("cookieStorage");
            }

            this.uri = url;
            this.cookieStorage = cookieStorage;
        }

        /// <summary>
        /// Creates the auth provider fitting to the given type.
        /// </summary>
        /// <returns>
        /// The auth provider.
        /// </returns>
        /// <param name='type'>Authentication type.</param>
        public IDisposableAuthProvider CreateAuthProvider(Type type) {
            switch (type) {
                case Type.BASIC:
                    return new PersistentStandardAuthenticationProvider(cookieStorage, uri);
                case Type.KERBEROS:
                    goto case Type.NTLM;
                case Type.NTLM:
                    return new PersistentNtlmAuthenticationProvider(cookieStorage, uri);
                default:
                    return new StandardAuthenticationProviderWrapper();
            }
        }
    }
}