//-----------------------------------------------------------------------
// <copyright file="NativeKeyStore.cs" company="GRAU DATA AG">
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
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Abstract class which must be implemented by native key stores for specific platforms.
    /// Implementations will be used to store credentials in native password manager/keychains/credential manager.
    ///
    /// An Instance should take the wished application name, which should be used inside in the native store.
    /// Instances with the same <see cref="NativeKeyStore.ApplicationName"/> should target the same collection.
    /// </summary>
    public abstract class NativeKeyStore : IDictionary<string, string> {
        /// <summary>
        /// Gets the name of the application which accesses the native store.
        /// </summary>
        /// <value>The name of the application.</value>
        protected string ApplicationName {
            get; private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSpace.Common.NativeKeyStore.NativeKeyStore"/> class.
        /// </summary>
        /// <param name="applicationName">Application name which will be used to access the native store. Must not be null, empty or whitespaces only.</param>
        protected NativeKeyStore(string applicationName = "DataSpace") {
            if (string.IsNullOrWhiteSpace(applicationName)) {
                if (applicationName == null) {
                    throw new ArgumentNullException("applicationName");
                } else if (applicationName.Equals(string.Empty)) {
                    throw new ArgumentException("Given appName is empty", "applicationName");
                } else {
                    throw new ArgumentException("Given appName contains only whitespaces", "applicationName");
                }
            }

            ApplicationName = applicationName;
        }

        /// <summary>
        /// Gets the keys/user names. Normally it is a combination of username and domain name. E.g. "demo.dataspace.cc\user".
        /// </summary>
        /// <value>The keys/user names.</value>
        public abstract ICollection<string> Keys { get; }

        /// <summary>
        /// Gets the plaintext passwords.
        /// </summary>
        /// <value>The plaintext passwords.</value>
        public abstract ICollection<string> Values { get; }

        /// <summary>
        /// Gets or sets the user name/plaintext password combination.
        /// </summary>
        /// <param name="key">user name.</param>
        public abstract string this [string key] { get; set; }

        /// <summary>
        /// Add the specified user name and plaintext password.
        /// </summary>
        /// <param name="key">User name.</param>
        /// <param name="value">Plaintext password.</param>
        public abstract void Add(string key, string value);

        /// <Docs>The object to locate in the current collection.</Docs>
        /// <para>Determines whether the current collection contains a specific value.</para>
        /// <summary>
        /// Contains the specified key.
        /// </summary>
        /// <param name="key">Key.</param>
        public abstract bool Contains(string key);

        /// <Docs>The item to remove from the current collection.</Docs>
        /// <para>Removes the first occurrence of an item from the current collection.</para>
        /// <summary>
        /// Remove the specified user account.
        /// </summary>
        /// <param name="key">User name of the account which should be removed.</param>
        public abstract bool Remove(string key);

        /// <summary>
        /// Gets the enumerator of all accounts.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public abstract IEnumerator<KeyValuePair<string, string>> GetEnumerator();

        /// <summary>
        /// Gets a value indicating whether this instance is read only. Normally it is false.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        public virtual bool IsReadOnly { get { return false; } }

        /// <summary>
        /// Gets the count of all stored accounts for the <see cref="NativeKeyStore.ApplicationName"/>.
        /// </summary>
        /// <value>The count of all stored accounts.</value>
        public virtual int Count { get { return Keys.Count; } }

        /// <Docs>The item to add to the current collection.</Docs>
        /// <para>Adds an item to the current collection.</para>
        /// <exception cref="System.NotSupportedException">The current collection is read-only.</exception>
        /// <summary>
        /// Add the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        public virtual void Add(KeyValuePair<string, string> item) {
            if (IsReadOnly) {
                throw new NotSupportedException("This store is read only");
            }

            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Removes all accounts for the <see cref="NativeKeyStore.ApplicationName"/>.
        /// </summary>
        public virtual void Clear() {
            foreach (var key in Keys) {
                Remove(key);
            }
        }

        /// <summary>
        /// Tries to get the plaintext password for the given user name.
        /// </summary>
        /// <returns><c>true</c>, if getting plaintext password was successful, <c>false</c> otherwise.</returns>
        /// <param name="key">User name.</param>
        /// <param name="value">Plaintext password.</param>
        public virtual bool TryGetValue(string key, out string value) {
            if (Contains(key)) {
                value = this [key];
                return true;
            } else {
                value = null;
                return false;
            }
        }

        /// <Docs>The object to locate in the current collection.</Docs>
        /// <para>Determines whether the current collection contains a specific value.</para>
        /// <summary>
        /// Contains the specified item.
        /// </summary>
        /// <param name="item">Username and plaintext password combination.</param>
        public virtual bool Contains(KeyValuePair<string, string> item) {
            if (Contains(item.Key)) {
                var entry = this [item.Key];
                return string.Equals(entry, item.Value);
            } else {
                return false;
            }
        }

        /// <Docs>The item to remove from the current collection.</Docs>
        /// <para>Removes the first occurrence of an item from the current collection.</para>
        /// <summary>
        /// Remove the specified item.
        /// </summary>
        /// <param name="item">Username and password combination.</param>
        public virtual bool Remove(KeyValuePair<string, string> item) {
            return Remove(item.Key);
        }

        /// <Docs>The user to locate in the current instance.</Docs>
        /// <para>Determines whether the current instance contains an entry with the specified user.</para>
        /// <summary>
        /// Containses the user.
        /// </summary>
        /// <returns><c>true</c>, if user name was containsed, <c>false</c> otherwise.</returns>
        /// <param name="key">User name.</param>
        public virtual bool ContainsKey(string key) {
            return Contains(key);
        }

        /// <summary>
        /// Copies accounts to the givven array starting at the given arrayIndex.
        /// </summary>
        /// <param name="array">Target array.</param>
        /// <param name="arrayIndex">Array index to start at.</param>
        public virtual void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) {
            foreach (var entry in this) {
                array [arrayIndex] = new KeyValuePair<string, string>(entry.Key, entry.Value);
                arrayIndex++;
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}