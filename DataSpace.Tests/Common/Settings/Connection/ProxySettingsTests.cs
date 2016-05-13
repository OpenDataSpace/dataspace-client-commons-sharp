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
    using System.Security;

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
            IProxySettings underTest = config.GetProxySettings();
            // check default values
            Assert.That(underTest.NeedLogin, Is.False);
            Assert.That(underTest.ProxyType, Is.EqualTo(ProxyType.Default));
            Assert.That(underTest.Url, Is.Empty);
            Assert.That(underTest.UserName, Is.Empty);
            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.Empty);
        }

        [Test]
        public void PropertyGetSet() {
            IProxySettings underTest = config.GetProxySettings();
            // act
            underTest.Url = url;
            underTest.UserName = username;
            underTest.Password = new SecureString().Init(password);
            underTest.NeedLogin = true;
            underTest.ProxyType = ProxyType.Custom;
            // assert
            Assert.That(underTest.Url, Is.EqualTo(url));
            Assert.That(underTest.UserName, Is.EqualTo(username));
            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.EqualTo(password));
            Assert.That(underTest.NeedLogin, Is.True);
            Assert.That(underTest.ProxyType, Is.EqualTo(ProxyType.Custom));
        }

        [Test]
        public void CreateNew() {
            IProxySettings underTest = config.GetProxySettings();
            underTest.ProxyType = ProxyType.Custom;
            underTest.NeedLogin = true;
            underTest.Url = url;
            underTest.UserName = username;
            underTest.Password = new System.Security.SecureString().Init(password);
        }

        [Test]
        public void DeleteTest() {
            IProxySettings underTest = config.GetProxySettings();
            underTest.UserName = Guid.NewGuid().ToString();
            underTest.Url = Guid.NewGuid().ToString();
            underTest.ProxyType = ProxyType.Custom;
            underTest.NeedLogin = true;
            underTest.Password = new SecureString().Init(Guid.NewGuid().ToString());
            underTest.Delete();

            Assert.That(underTest.NeedLogin, Is.False);
            Assert.That(underTest.ProxyType, Is.EqualTo(ProxyType.Default));
            Assert.That(underTest.Url, Is.Empty);
            Assert.That(underTest.UserName, Is.Empty);
            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.Empty);
        }

        [Test]
        public void DeletedTriggersEvent() {
            IProxySettings underTest = config.GetProxySettings();
            // Save Event Handler
            bool IsTriggered = false;
            underTest.SettingsDeleted += (sender, arg) => {
                IsTriggered = true;
            };
            // Save
            underTest.Delete();

            Assert.That(IsTriggered, Is.True);
        }

        [Test]
        public void OnPropertyChanged_Success() {
            IProxySettings underTest = config.GetProxySettings();

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
            Assert.That(underTest.ProxyType, Is.EqualTo(ProxyType.Custom));
            Assert.That(underTest.NeedLogin, Is.True);
            Assert.That(underTest.Url, Is.EqualTo(url));
            Assert.That(underTest.UserName, Is.EqualTo(username));
            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.EqualTo(password));

            Assert.That(ReceivedEvents.Count, Is.EqualTo(5));
            Assert.That(ReceivedEvents[0], Is.EqualTo(Property.NameOf((IProxySettings a) => a.ProxyType)));
            Assert.That(ReceivedEvents[1], Is.EqualTo(Property.NameOf((IProxySettings a) => a.NeedLogin)));
            Assert.That(ReceivedEvents[2], Is.EqualTo(Property.NameOf((IProxySettings a) => a.Url)));
            Assert.That(ReceivedEvents[3], Is.EqualTo(Property.NameOf((IProxySettings a) => a.UserName)));
            Assert.That(ReceivedEvents[4], Is.EqualTo(Property.NameOf((IProxySettings a) => a.Password)));
        }
    }
}