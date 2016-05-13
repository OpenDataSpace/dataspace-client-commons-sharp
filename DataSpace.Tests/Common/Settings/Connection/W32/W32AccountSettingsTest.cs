//-----------------------------------------------------------------------
// <copyright file="AccountSettingsTest.cs" company="GRAU DATA AG">
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

namespace Tests.Common.Settings.Connection.W32 {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using DataSpace.Common.Crypto;
    using DataSpace.Common.Settings;
    using DataSpace.Common.Settings.Connection;
    using DataSpace.Common.Settings.Connection.W32;
    using DataSpace.Common.Utils;
    using DataSpace.Tests.Utils;

    using NUnit.Framework;

    [TestFixture]
    public class W32AccountSettingsTest : WithConfiguredLog4Net {
        private Configuration configuration;
        private IAccountSettings underTest;

        [TestFixtureSetUp]
        public void LimitToWindows() {
            this.EnsureThisRunsOn(PlatformID.Win32NT, PlatformID.Win32S, PlatformID.Win32Windows, PlatformID.WinCE);
        }

        [SetUp]
        public void SetUp() {
            configuration = new ConfigurationLoader(new UserConfigPathBuilder{ Company = "UnitTest" }).Configuration;
            underTest = new AccountSettingsFactory().CreateInstance(configuration, "DataSpaceAccount" + Guid.NewGuid().ToString(), "https://example.com/", "username", new SecureString().Init("pw"));
        }

        [Test, NUnit.Framework.Category("Slow")]
        public void PropGet_TriggersLoad() {
            // clear dirty flag
            underTest.Load();
            AccountSettings AccSetObj = underTest as AccountSettings;
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 0, 5);
            bool IsTriggered = false;
            underTest.SettingsLoaded += (sender, arg) => {
                IsTriggered = true;
            };
            // act
            string Url = underTest.Url;
            System.Threading.Thread.Sleep(5020);
            Url = underTest.Url;
            // assert
            Assert.AreEqual(true, IsTriggered);
            // house keeping
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 2, 0);
        }

        [Test, NUnit.Framework.Category("Slow")]
        public void PropGet_NoTriggersLoadInRefreshSpan() {
            // clear dirty flag
            underTest.Load();
            AccountSettings AccSetObj = underTest as AccountSettings;
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 2, 0);
            bool IsTriggered = false;
            underTest.SettingsLoaded += (sender, arg) => {
                IsTriggered = true;
            };
            // act
            string Url = underTest.Url;
            System.Threading.Thread.Sleep(1000);
            Url = underTest.Url;
            // assert
            Assert.AreEqual(false, IsTriggered);
        }

        [Test, NUnit.Framework.Category("Slow")]
        public void PropGet_NoTriggersLoadDisabled() {
            // clear dirty flag
            underTest.Load();
            AccountSettings AccSetObj = underTest as AccountSettings;
            // set 5 sec timer
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 5, 0);
            // set disabled
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 0, 0);
            bool IsTriggered = false;
            underTest.SettingsLoaded += (sender, arg) => {
                IsTriggered = true;
            };
            // act
            string Url = underTest.Url;
            System.Threading.Thread.Sleep(5020);
            Url = underTest.Url;
            // assert
            Assert.AreEqual(false, IsTriggered);
            // house keeping
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 2, 0);
        }

        [Test, NUnit.Framework.Category("Slow")]
        public void PropGet_NoTriggersLoadEditMode() {
            // clear dirty flag
            underTest.Load();
            AccountSettings AccSetObj = underTest as AccountSettings;
            // set 5 sec timer
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 5, 0);
            bool isTriggered = false;
            underTest.SettingsLoaded += (sender, arg) => {
                isTriggered = true;
            };
            // act
            string Password = underTest.Password.ConvertToUnsecureString();
            // modify -> go in Edit mode
            underTest.Password =  new SecureString().Init(Password + "!");
            Thread.Sleep(5020);
            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.EquivalentTo(Password));
            // assert
            Assert.That(isTriggered, Is.False);
            // house keeping
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 2, 0);
        }
    }
}