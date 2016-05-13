//-----------------------------------------------------------------------
// <copyright file="DataSpaceAccountCollectionTest.cs" company="GRAU DATA AG">
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

namespace Tests.Common.Settings.Accounts {
    using System.Linq;
    using System.Security;

    using DataSpace.Common.Crypto;
    using DataSpace.Common.Settings;

    using NUnit.Framework;

    [TestFixture, NUnit.Framework.Category("UnitTests")]
    public class DataSpaceAccountCollectionTest : WithGeneratedConfig {
        [Test]
        public void SaveConfigAndLoadConfig() {
            var username = "user";
            var password = "password";
            var url = "https://example.org/";
            var underTest = config.GetDataSpaceAccounts();
            Assert.That(underTest, Is.Empty);
            var account = config.AddDataSpaceAccount(url, username, new SecureString().Init(password));
            Assert.That(account.Url, Is.EqualTo(url));
            Assert.That(config.GetDataSpaceAccounts().Count, Is.EqualTo(1));
            Assert.That(config.GetDataSpaceAccounts().First().Value, Is.EqualTo(account));
            account.Delete();
        }
    }
}