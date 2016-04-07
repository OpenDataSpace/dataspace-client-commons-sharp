//-----------------------------------------------------------------------
// <copyright file="AccountSettingsFactoryTest.cs" company="GRAU DATA AG">
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
namespace Tests.Common.Settings.Connection {
    using System;
    using System.Configuration;

    using DataSpace.Common.Settings;
    using DataSpace.Common.Settings.Connection;

    using NUnit.Framework;

    [TestFixture]
    public class AccountSettingsFactoryTest {
        [Test]
        public void CreateInstance() {
            IAccountSettingsFactory underTest = new AccountSettingsFactory();
            var config = new ConfigurationLoader(new UserConfigPathBuilder(){FileName = Guid.NewGuid().ToString()}).Configuration;
            var account = underTest.CreateInstance("", config);
            Assert.That(account, Is.Not.Null);
        }
    }
}