//-----------------------------------------------------------------------
// <copyright file="NoNativeKeyStoreAvailable.cs" company="GRAU DATA AG">
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

    public class NoNativeKeyStoreAvailable : NativeKeyStore {
        public NoNativeKeyStoreAvailable(string appName) : base(appName) {
            throw new NotSupportedException("There is no native key store available on this platform");
        }

        public override ICollection<string> Keys {
            get {
                throw new NotSupportedException();
            }
        }

        public override ICollection<string> Values {
            get {
                throw new NotSupportedException();
            }
        }

        public override string this [string key] {
            get {
                throw new NotSupportedException();
            }
            set {
                throw new NotSupportedException();
            }
        }

        public override void Add(string key, string value) {
            throw new NotSupportedException();
        }

        public override bool Contains(string key) {
            throw new NotSupportedException();
        }

        public override bool Remove(string key) {
            throw new NotSupportedException();
        }

        public override IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
            throw new NotSupportedException();
        }
    }
}