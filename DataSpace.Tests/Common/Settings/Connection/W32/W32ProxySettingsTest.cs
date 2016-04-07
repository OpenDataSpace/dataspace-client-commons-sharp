//-----------------------------------------------------------------------
// <copyright file="W32ProxySettingsTest.cs" company="GRAU DATA AG">
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
#if ! __MonoCS__
namespace Tests.Common.Settings.Connection.W32 {
    using System;

    using DataSpace.Common.Settings.Connection;
    using DataSpace.Common.Settings.Connection.W32;

    using NUnit.Framework;

    [TestFixture]
    public class W32ProxySettingsTest {
        [Test, NUnit.Framework.Category("Slow")]
        public void PropGet_TriggersLoad() {
            //prep
            IProxySettings underTest = new ConnectionSettingsFactory().ProxySettings;
            // clear dirty flag
            underTest.Load();
            ProxySettings ProxSetObj = underTest as ProxySettings;
            ProxSetObj.PropsRefreshSpan = new TimeSpan(0, 0, 5);
            bool IsTriggered = false;
            underTest.SettingsLoaded += (sender, arg) => {
                IsTriggered = true;
            };
            // act
            string Url = underTest.Url;
            System.Threading.Thread.Sleep(6020);
            Url = underTest.Url;
            // assert
            Assert.That(IsTriggered, Is.True);
            // house keeping
            ProxSetObj.PropsRefreshSpan = new TimeSpan(0, 2, 0);
        }

        [Test, NUnit.Framework.Category("Slow")]
        public void PropGet_NoTriggersLoadInRefreshSpan() {
            // prep
            IProxySettings ProxSet = new ConnectionSettingsFactory().ProxySettings;
            // clear dirty flag
            ProxSet.Load();
            ProxySettings ProxSetObj = ProxSet as ProxySettings;
            ProxSetObj.PropsRefreshSpan = new TimeSpan(0, 2, 0);
            bool IsTriggered = false;
            ProxSet.SettingsLoaded += (sender, arg) => {
                IsTriggered = true;
            };
            // act
            string Url = ProxSet.Url;
            System.Threading.Thread.Sleep(500);
            Url = ProxSet.Url;
            // assert
            Assert.That(IsTriggered, Is.False);
        }

        [Test, NUnit.Framework.Category("Slow")]
        public void PropGet_NoTriggersLoadDisabled() {
            // prep
            IProxySettings ProxSet = new ConnectionSettingsFactory().ProxySettings;
            // clear dirty flag
            ProxSet.Load();
            ProxySettings ProxSetObj = ProxSet as ProxySettings;
            // set 5 sec timer
            ProxSetObj.PropsRefreshSpan = new TimeSpan(0, 0, 5);
            // set disabled
            ProxSetObj.PropsRefreshSpan = new TimeSpan(0, 0, 0);
            bool IsTriggered = false;
            ProxSet.SettingsLoaded += (sender, arg) => {
                IsTriggered = true;
            };
            // act
            string Url = ProxSet.Url;
            System.Threading.Thread.Sleep(5020);
            Url = ProxSet.Url;
            // assert
            Assert.That(IsTriggered, Is.False);
            // house keeping
            ProxSetObj.PropsRefreshSpan = new TimeSpan(0, 2, 0);
        }

        [Test, NUnit.Framework.Category("Slow")]
        public void PropGet_NoTriggersLoadEditMode() {
            // prep
            IProxySettings ProxSet = new ConnectionSettingsFactory().ProxySettings;
            // clear dirty flag
            ProxSet.Load();
            ProxySettings ProxSetObj = ProxSet as ProxySettings;
            // set 5 sec timer
            ProxSetObj.PropsRefreshSpan = new TimeSpan(0, 0, 5);
            bool IsTriggered = false;
            ProxSet.SettingsLoaded += (sender, arg) => {
                IsTriggered = true;
            };
            // act
            string Url = ProxSet.Url;
            // modify -> go in Edit mode
            ProxSet.Url = Url + "!";
            System.Threading.Thread.Sleep(5020);
            Url = ProxSet.Url;
            // assert
            Assert.That(IsTriggered, Is.False);
            // house keeping
            ProxSetObj.PropsRefreshSpan = new TimeSpan(0, 2, 0);
        }
    }
}
#endif