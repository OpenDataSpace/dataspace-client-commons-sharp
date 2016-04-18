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

    public abstract class NativeKeyStore : IDictionary<string, string> {
        protected string ApplicationName {
            get; private set;
        }

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

        public abstract ICollection<string> Keys { get; }

        public abstract ICollection<string> Values { get; }

        public abstract string this [string key] { get; set; }

        public abstract void Add(string key, string value);

        public abstract bool Contains(string key);

        public abstract bool Remove(string key);

        public abstract IEnumerator<KeyValuePair<string, string>> GetEnumerator();

        public virtual bool IsReadOnly { get { return false; } }

        public virtual int Count { get { return Keys.Count; } }

        public virtual void Clear() {
            foreach (var key in Keys) {
                Remove(key);
            }
        }

        public virtual bool TryGetValue(string key, out string value) {
            if (Contains(key)) {
                value = this [key];
                return true;
            } else {
                value = null;
                return false;
            }
        }

        public virtual bool Contains(KeyValuePair<string, string> item) {
            if (Contains(item.Key)) {
                var entry = this [item.Key];
                return string.Equals(entry, item.Value);
            } else {
                return false;
            }
        }

        public virtual bool Remove(KeyValuePair<string, string> item) {
            return Remove(item.Key);
        }

        public virtual bool ContainsKey(string key) {
            return Contains(key);
        }

        public virtual void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) {
            foreach (var entry in this) {
                array [arrayIndex] = new KeyValuePair<string, string>(entry.Key, entry.Value);
                arrayIndex++;
            }
        }

        public virtual System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}