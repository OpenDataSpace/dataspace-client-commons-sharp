//-----------------------------------------------------------------------
// <copyright file="PersistentStandardAuthenticationProvider.cs" company="GRAU DATA AG">
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
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;

    using DotCMIS.Binding;

    /// <summary>
    /// Persistent standard authentication provider.
    /// </summary>
    public class PersistentStandardAuthenticationProvider : DotCMIS.Binding.StandardAuthenticationProvider, IDisposableAuthProvider {
        private ICookieStorage storage;
        private bool disposed;
        private Uri url;

        /// <summary>
        /// Initializes a new instance of the <see cref="CmisSync.Lib.Cmis.PersistentStandardAuthenticationProvider"/> class.
        /// </summary>
        /// <param name="storage">Storage of saved cookies</param>
        /// <param name="url">corresponding URL of the cookies</param>
        public PersistentStandardAuthenticationProvider(ICookieStorage storage, Uri url) {
            if (storage == null) {
                throw new ArgumentNullException("storage");
            }

            if (url == null) {
                throw new ArgumentNullException("url");
            }

            this.storage = storage;
            this.url = url;
            if (storage.Cookies != null) {
                foreach (Cookie c in storage.Cookies) {
                    this.Cookies.Add(c);
                }
            }
        }

        /// <summary>
        /// Handles the HttpWebResponse by extracting the cookies.
        /// </summary>
        /// <param name="connection">Connection instance of the response</param>
        public override void HandleResponse(object connection) {
            HttpWebResponse response = connection as HttpWebResponse;
            if (response != null) {
                // AtomPub and browser binding authentication
                this.Cookies.Add(response.Cookies);
            }
        }

        /// <summary>
        /// Releases all resource used by the <see cref="CmisSync.Lib.Cmis.PersistentStandardAuthenticationProvider"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the
        /// <see cref="CmisSync.Lib.Cmis.PersistentStandardAuthenticationProvider"/>. The <see cref="Dispose"/> method
        /// leaves the <see cref="CmisSync.Lib.Cmis.PersistentStandardAuthenticationProvider"/> in an unusable state.
        /// After calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="CmisSync.Lib.Cmis.PersistentStandardAuthenticationProvider"/> so the garbage collector can
        /// reclaim the memory that the <see cref="CmisSync.Lib.Cmis.PersistentStandardAuthenticationProvider"/> was occupying.</remarks>
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Deletes all cookies.
        /// </summary>
        public void DeleteAllCookies() {
            this.Cookies = new CookieContainer();
        }

        /// <summary>
        /// Dispose the specified disposing.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c> disposing.</param>
        protected virtual void Dispose(bool disposing) {
            if (!this.disposed) {
                if (disposing) {
                    // Dispose managed resources.
                    try {
                        this.storage.Cookies = this.Cookies.GetCookies(this.url);
                    } catch (Exception e) {
                    }
                }

                this.disposed = true;
            }
        }
    }
}