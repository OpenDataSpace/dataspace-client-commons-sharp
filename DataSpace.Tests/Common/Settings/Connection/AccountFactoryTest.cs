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
using DataSpace.Common.Crypto;


﻿
namespace Tests.Common.Settings.Connection {
    using System;
    using System.Configuration;
    using System.Security;

    using DataSpace.Common.Settings;
    using DataSpace.Common.Settings.Connection;

    using NUnit.Framework;

    [TestFixture, Category("UnitTests")]
    public class AccountFactoryTest {
        private readonly string url = "https://example.com/";
        private readonly string username = "user";
        private readonly string password = "password";

        [Test]
        public void CreateInstance() {
            IAccountFactory underTest = new AccountFactory();
            var account = underTest.CreateInstance(url, username, new SecureString().Init(password));
            Assert.That(account, Is.Not.Null);
            Assert.That(account.Url, Is.EqualTo(url));
            Assert.That(account.UserName, Is.EqualTo(username));
            Assert.That(account.Password.ConvertToUnsecureString(), Is.EqualTo(password));
        }
    }
}