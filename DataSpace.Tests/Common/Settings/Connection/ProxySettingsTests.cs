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
    using System.Configuration;
    using System.ComponentModel;
    using System.IO;

    using DataSpace.Common.Crypto;
    using DataSpace.Common.Settings;
    using DataSpace.Common.Settings.Connection;
    using DataSpace.Common.Utils;
    using DataSpace.Tests.Utils;

    ﻿using NUnit.Framework;

    [TestFixture, NUnit.Framework.Category("UnitTests")]
    public class ProxySettingsTest : WithGeneratedConfig {
        private string url = "test.url.com";
        private string username = "TestName";
        private string password = "TestPassword";

        [Test]
        public void Constructor() {
            IProxySettings underTest = new ProxySettingsFactory().GetInstance(config);
            // check default values
            Assert.That(underTest.IsDirty, Is.False);
            Assert.That(underTest.NeedLogin, Is.False);
            Assert.That(underTest.ProxyType, Is.EqualTo(ProxyType.Default));
            Assert.That(underTest.Url, Is.Empty);
            Assert.That(underTest.UserName, Is.Empty);
            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.Empty);
        }

        [Test]
        public void PropertyGetSet() {
            IProxySettings underTest = new ProxySettingsFactory().GetInstance(config);
            // act
            underTest.Url = url;
            underTest.UserName = username;
            underTest.Password = new System.Security.SecureString().Init(password);
            underTest.NeedLogin = true;
            underTest.ProxyType = ProxyType.Custom;
            // assert
            Assert.That(underTest.Url, Is.EqualTo(url));
            Assert.That(underTest.UserName, Is.EqualTo(username));
            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.EqualTo(password));
            Assert.That(underTest.NeedLogin, Is.True);
            Assert.That(underTest.ProxyType, Is.EqualTo(ProxyType.Custom));
            Assert.That(underTest.IsDirty, Is.True);
        }

        [Test]
        public void CreateNew() {
            IProxySettings underTest = new ProxySettingsFactory().GetInstance(config);
            underTest.ProxyType = ProxyType.Custom;
            underTest.NeedLogin = true;
            underTest.Url = url;
            underTest.UserName = username;
            underTest.Password = new System.Security.SecureString().Init(password);
            underTest.Save();
        }

        [Test]
        public void DeleteTest() {
            IProxySettings underTest = new ProxySettingsFactory().GetInstance(config);

            underTest.Delete();

            Assert.That(underTest.IsDirty, Is.False);
            Assert.That(underTest.NeedLogin, Is.False);
            Assert.That(underTest.ProxyType, Is.EqualTo(ProxyType.Default));
            Assert.That(underTest.Url, Is.Empty);
            Assert.That(underTest.UserName, Is.Empty);
            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.Empty);
        }

        [Test]
        public void Load_TriggersEvent() {
            IProxySettings underTest = new ProxySettingsFactory().GetInstance(config);
            // Load Event Handler
            bool IsTriggered = false;
            underTest.SettingsLoaded += (sender, arg) => {
                IsTriggered = true;
            };

            // Load
            underTest.Load();

            Assert.That(IsTriggered, Is.True);
        }

        [Test]
        public void Save_TriggersEvent() {
            IProxySettings underTest = new ProxySettingsFactory().GetInstance(config);
            // Save Event Handler
            bool IsTriggered = false;
            underTest.SettingsSaved += (sender, arg) => {
                IsTriggered = true;
            };
            // Save
            underTest.Save();

            Assert.That(IsTriggered, Is.True);
        }

        [Test]
        public void OnPropertyChanged_Success() {
            IProxySettings underTest = new ProxySettingsFactory().GetInstance(config);

            List<string> ReceivedEvents = new List<string>();

            underTest.PropertyChanged += delegate (object sender, PropertyChangedEventArgs args) {
                ReceivedEvents.Add(args.PropertyName);
            };
            // Act
            underTest.ProxyType = ProxyType.Custom;
            underTest.NeedLogin = true;
            underTest.Url = url;
            underTest.UserName = username;
            underTest.Password = new System.Security.SecureString().Init(password);

            // Assert
            Assert.That(underTest.IsDirty, Is.True);
            Assert.That(underTest.ProxyType, Is.EqualTo(ProxyType.Custom));
            Assert.That(underTest.NeedLogin, Is.True);
            Assert.That(underTest.Url, Is.EqualTo(url));
            Assert.That(underTest.UserName, Is.EqualTo(username));
            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.EqualTo(password));

            Assert.That(ReceivedEvents.Count, Is.EqualTo(6));
            Assert.AreEqual(Property.NameOf((IProxySettings a) => a.ProxyType), ReceivedEvents[0]);
            Assert.AreEqual(Property.NameOf((IProxySettings a) => a.IsDirty), ReceivedEvents[1]);
            Assert.AreEqual(Property.NameOf((IProxySettings a) => a.NeedLogin), ReceivedEvents[2]);
            Assert.AreEqual(Property.NameOf((IProxySettings a) => a.Url), ReceivedEvents[3]);
            Assert.AreEqual(Property.NameOf((IProxySettings a) => a.UserName), ReceivedEvents[4]);
            Assert.AreEqual(Property.NameOf((IProxySettings a) => a.Password), ReceivedEvents[5]);
        }
    }
}