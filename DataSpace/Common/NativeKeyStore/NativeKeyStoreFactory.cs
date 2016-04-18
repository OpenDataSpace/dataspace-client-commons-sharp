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
    using System.Reflection;

    public class NativeKeyStoreFactory<T> : INativeKeyStoreFactory where T : NativeKeyStore {
        public NativeKeyStore CreateInstance(params object[] args) {
            try {
                return Activator.CreateInstance(typeof(T), args) as NativeKeyStore;
            } catch (TargetInvocationException ex) {
                throw new NotSupportedException(ex.Message, ex);
            }
        }
    }
}