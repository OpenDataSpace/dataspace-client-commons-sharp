//-----------------------------------------------------------------------
// <copyright file="ProxySettingsTests.cs" company="GRAU DATA AG">
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

namespace Tests.Common.Settings.Connection {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using DataSpace.Common.Crypto;
    using DataSpace.Common.Settings.Connection;
    using DataSpace.Common.Settings.Connection.W32;
    using DataSpace.Common.Utils;
    using DataSpace.Tests.Utils;

    ﻿using NUnit.Framework;

    [TestFixture]
    public class ProxySettingsTest : WithConfiguredLog4Net {
        private string _Url = "test.url.com";
        private string _UserName = "TestName";
        private string _Password = "TestPassword";

        [Test]
        public void Constructor() {
            IProxySettings ProxSet = new ProxySettings();
            // check default values
            Assert.That(ProxSet.IsDirty, Is.False);
            Assert.That(ProxSet.NeedLogin, Is.False);
            Assert.That(ProxSet.ProxyType, Is.EqualTo(ProxyType.None));
            Assert.That(ProxSet.Url, Is.Empty);
            Assert.That(ProxSet.UserName, Is.Empty);
            Assert.That(ProxSet.Password.ConvertToUnsecureString(), Is.Empty);
        }

        [Test]
        public void PropertyGetSet() {
            IProxySettings ProxSet = new ProxySettings();
            // act
            ProxSet.Url = _Url;
            ProxSet.UserName = _UserName;
            ProxSet.Password = new System.Security.SecureString().Init(_Password);
            ProxSet.NeedLogin = true;
            ProxSet.ProxyType = ProxyType.Custom;
            // assert
            Assert.That(ProxSet.Url, Is.EqualTo(_Url));
            Assert.That(ProxSet.UserName, Is.EqualTo(_UserName));
            Assert.That(ProxSet.Password.ConvertToUnsecureString(), Is.EqualTo(_Password));
            Assert.That(ProxSet.NeedLogin, Is.True);
            Assert.That(ProxSet.ProxyType, Is.EqualTo(ProxyType.Custom));
            Assert.That(ProxSet.IsDirty, Is.True);
        }

        [Test]
        public void CreateNew() {
            IProxySettings ProxSet = new ConnectionSettingsFactory().ProxySettings;
            ProxSet.ProxyType = ProxyType.Custom;
            ProxSet.NeedLogin = true;
            ProxSet.Url = _Url;
            ProxSet.UserName = _UserName;
            ProxSet.Password = new System.Security.SecureString().Init(_Password);
            ProxSet.Save();
        }

        [Test]
        public void DeleteTest() {
            IProxySettings ProxSet = new ConnectionSettingsFactory().ProxySettings;

            ProxSet.Delete();

            Assert.That(ProxSet.IsDirty, Is.False);
            Assert.AreEqual(false, ProxSet.NeedLogin);
            Assert.AreEqual(ProxyType.None, ProxSet.ProxyType);
            Assert.AreEqual(string.Empty, ProxSet.Url);
            Assert.AreEqual(string.Empty, ProxSet.UserName);
            Assert.AreEqual(string.Empty, ProxSet.Password.ConvertToUnsecureString());
        }

        [Test]
        public void Load_TriggersEvent() {
            string tstr = AppDomain.CurrentDomain.BaseDirectory;
            IProxySettings ProxSet = new ConnectionSettingsFactory().ProxySettings;
            // Load Event Handler
            bool IsTriggered = false;
            ProxSet.SettingsLoaded += (sender, arg) => {
                IsTriggered = true;
            };

            // Load 
            ProxSet.Load();

            Assert.That(IsTriggered, Is.True);
        }

        [Test]
        public void Save_TriggersEvent() {
            IProxySettings ProxSet = new ConnectionSettingsFactory().ProxySettings;
            // Save Event Handler
            bool IsTriggered = false;
            ProxSet.SettingsSaved += (sender, arg) => {
                IsTriggered = true;
            };
            // Save 
            ProxSet.Save();

            Assert.That(IsTriggered, Is.True);
        }

        [Test]
        public void OnPropertyChanged_Success() {
            IProxySettings ProxSet = new ProxySettings();

            List<string> ReceivedEvents = new List<string>();

            ProxSet.PropertyChanged += delegate (object sender, PropertyChangedEventArgs args) {
                ReceivedEvents.Add(args.PropertyName);
            };
            // Act
            ProxSet.ProxyType = ProxyType.Custom;
            ProxSet.NeedLogin = true;
            ProxSet.Url = _Url;
            ProxSet.UserName = _UserName;
            ProxSet.Password = new System.Security.SecureString().Init(_Password);

            // Assert
            Assert.That(ProxSet.IsDirty, Is.True);
            Assert.That(ProxSet.ProxyType, Is.EqualTo(ProxyType.Custom));
            Assert.That(ProxSet.NeedLogin, Is.True);
            Assert.That(ProxSet.Url, Is.EqualTo(_Url));
            Assert.That(ProxSet.UserName, Is.EqualTo(_UserName));
            Assert.That(ProxSet.Password.ConvertToUnsecureString(), Is.EqualTo(_Password));

            Assert.That(ReceivedEvents.Count, Is.EqualTo(6));
            Assert.AreEqual(Property.NameOf((IProxySettings a) => a.ProxyType), ReceivedEvents[0]);
            Assert.AreEqual(Property.NameOf((IProxySettings a) => a.IsDirty), ReceivedEvents[1]);
            Assert.AreEqual(Property.NameOf((IProxySettings a) => a.NeedLogin), ReceivedEvents[2]);
            Assert.AreEqual(Property.NameOf((IProxySettings a) => a.Url), ReceivedEvents[3]);
            Assert.AreEqual(Property.NameOf((IProxySettings a) => a.UserName), ReceivedEvents[4]);
            Assert.AreEqual(Property.NameOf((IProxySettings a) => a.Password), ReceivedEvents[5]);
        }

        [Test, NUnit.Framework.Category("Slow")]
        public void PropGet_TriggersLoad() {
            //prep
            IProxySettings ProxSet = new ConnectionSettingsFactory().ProxySettings;
            // clear dirty flag
            ProxSet.Load();
            ProxySettings ProxSetObj = ProxSet as ProxySettings;
            ProxSetObj.PropsRefreshSpan = new TimeSpan(0, 0, 5);
            bool IsTriggered = false;
            ProxSet.SettingsLoaded += (sender, arg) => {
                IsTriggered = true;
            };
            // act
            string Url = ProxSet.Url;
            System.Threading.Thread.Sleep(6020);
            Url = ProxSet.Url;
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