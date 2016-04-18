//-----------------------------------------------------------------------
// <copyright file="WindowsPasswordStoreTest.cs" company="GRAU DATA AG">
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
namespace Tests.Common.NativeKeyStore.W32 {
    using System;

    using DataSpace.Common.NativeKeyStore;
    using DataSpace.Common.NativeKeyStore.W32;

    using DataSpace.Tests.Utils;

    using NUnit.Framework;

    [TestFixture, Category("UnitTests")]
    public class WindowsPasswordStoreTest {
        [TestFixtureSetUp]
        public void LimitToWindows() {
            this.EnsureThisRunsOn(PlatformID.Win32NT, PlatformID.Win32S, PlatformID.Win32Windows, PlatformID.WinCE);
        }

        [Test]
        public void CreateInstanceViaFactory() {
            var underTest = new NativeKeyStoreFactory<WindowsKeyStore>().CreateInstance();
            Assert.That(underTest, Is.Not.Null);
        }
    }
}