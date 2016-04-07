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
    using System.Text;
    using System.Threading.Tasks;

    using DataSpace.Common.Crypto;
    using DataSpace.Common.Settings;
    using DataSpace.Common.Settings.Connection;
    using DataSpace.Common.Settings.Connection.W32;
    using DataSpace.Common.Utils;
    using DataSpace.Tests.Utils;

    using NUnit.Framework;

#if __MonoCS__
        [Ignore("IGNORED ON MONO")]
#endif
    [TestFixture]
    public class W32AccountSettingsTest : WithConfiguredLog4Net {
        private Configuration configuration;

        [SetUp]
        public void SetUp() {
            configuration = new ConfigurationLoader(new UserConfigPathBuilder{ Company = "UnitTest" }).Configuration;
        }

        [Test, NUnit.Framework.Category("Slow")]
        public void PropGet_TriggersLoad() {
            //prep
            IAccountSettings underTest = new AccountSettingsFactory().CreateInstance("DataSpaceAccount", configuration);
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
            // prep
            IAccountSettings AccSet = new ConnectionSettingsFactory().AccountSettings;
            // clear dirty flag
            AccSet.Load();
            AccountSettings AccSetObj = AccSet as AccountSettings;
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 2, 0);
            bool IsTriggered = false;
            AccSet.SettingsLoaded += (sender, arg) => {
                IsTriggered = true;
            };
            // act
            string Url = AccSet.Url;
            System.Threading.Thread.Sleep(1000);
            Url = AccSet.Url;
            // assert
            Assert.AreEqual(false, IsTriggered);
        }

        [Test, NUnit.Framework.Category("Slow")]
        public void PropGet_NoTriggersLoadDisabled() {
            // prep
            IAccountSettings AccSet = new ConnectionSettingsFactory().AccountSettings;
            // clear dirty flag
            AccSet.Load();
            AccountSettings AccSetObj = AccSet as AccountSettings;
            // set 5 sec timer
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 5, 0);
            // set disabled
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 0, 0);
            bool IsTriggered = false;
            AccSet.SettingsLoaded += (sender, arg) => {
                IsTriggered = true;
            };
            // act
            string Url = AccSet.Url;
            System.Threading.Thread.Sleep(5020);
            Url = AccSet.Url;
            // assert
            Assert.AreEqual(false, IsTriggered);
            // house keeping
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 2, 0);
        }

        [Test, NUnit.Framework.Category("Slow")]
        public void PropGet_NoTriggersLoadEditMode() {
            // prep
            IAccountSettings AccSet = new ConnectionSettingsFactory().AccountSettings;
            // clear dirty flag
            AccSet.Load();
            AccountSettings AccSetObj = AccSet as AccountSettings;
            // set 5 sec timer
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 5, 0);
            bool IsTriggered = false;
            AccSet.SettingsLoaded += (sender, arg) => {
                IsTriggered = true;
            };
            // act
            string Url = AccSet.Url;
            // modify -> go in Edit mode
            AccSet.Url = Url + "!";
            System.Threading.Thread.Sleep(5020);
            Url = AccSet.Url;
            // assert
            Assert.AreEqual(false, IsTriggered);
            // house keeping
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 2, 0);
        }
    }
}
