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

namespace Tests.Common.Settings.Connection {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Security;

    using DataSpace.Common.Crypto;
    using DataSpace.Common.Settings.Connection;
    using DataSpace.Common.Settings;
    using DataSpace.Common.Utils;

    using NUnit.Framework;

    [TestFixture, NUnit.Framework.Category("UnitTests")]
    public class AccountSettingsTest : WithGeneratedConfig {
        private string _Url = "test.url.com";
        private string _UserName = "TestName";
        private string _Password = "TestPassword";
        private IAccountSettings underTest;
        private string accountName;

        [SetUp]
        public void CreateAccountInstance() {
            accountName = "DataSpaceAccount" + Guid.NewGuid().ToString();
            underTest = new AccountSettingsFactory().CreateInstance(config, accountName, _Url, _UserName, new SecureString().Init(_Password));
        }

        [TearDown]
        public void RemoveAccountInstance() {
            if (underTest != null) {
                underTest.Delete();
            }
        }


        [Test]
        public void Constructor() {
            underTest.Load();
            // Assert
            Assert.That(underTest.IsDirty, Is.False);
            Assert.That(underTest.Url, Is.EqualTo(string.Empty));
            Assert.That(underTest.UserName, Is.EqualTo(string.Empty));
            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void PropertyGetSet() {
            underTest.Load();
            // act
            underTest.Password = new System.Security.SecureString().Init(_Password);
            // assert
            Assert.That(underTest.Url, Is.EqualTo(_Url));
            Assert.That(underTest.UserName, Is.EqualTo(_UserName));
            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.EqualTo(_Password));
            Assert.That(underTest.IsDirty, Is.True);
        }

        [Test]
        public void WriteAndRead() {
            underTest.Load();
            underTest.Password = new System.Security.SecureString().Init(_Password);
            underTest.Save();

            underTest = new AccountSettingsFactory().LoadInstance(config, config.GetSection(accountName) as AbstractAccountSettingsSection);
            underTest.Load();
            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.EqualTo(_Password));
            Assert.That(underTest.Url, Is.EqualTo(_Url));
            Assert.That(underTest.UserName, Is.EqualTo(_UserName));
        }

        [Test]
        public void CreateNew() {
            underTest.Load();
            underTest.Password = new System.Security.SecureString().Init(_Password);
            underTest.Save();
        }

        [Test]
        public void Check_OnPropertyChanged_Success() {
            underTest.Load();
            List<string> ReceivedEvents = new List<string>();

            underTest.PropertyChanged += delegate (object sender, PropertyChangedEventArgs args) {
                ReceivedEvents.Add(args.PropertyName);
            };
            // Act
            underTest.Password = new System.Security.SecureString().Init(_Password);

            // Assert
            Assert.That(underTest.IsDirty, Is.True);
            Assert.That(underTest.Url, Is.EqualTo(_Url));
            Assert.That(underTest.UserName, Is.EqualTo(_UserName));
            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.EqualTo(_Password));

            Assert.That(ReceivedEvents.Count, Is.EqualTo(4));
            Assert.That(ReceivedEvents[0], Is.EqualTo(Property.NameOf((IAccountSettings a) => a.Url)));
            Assert.That(ReceivedEvents[1], Is.EqualTo(Property.NameOf((IAccountSettings a) => a.IsDirty)));
            Assert.That(ReceivedEvents[2], Is.EqualTo(Property.NameOf((IAccountSettings a) => a.UserName)));
            Assert.That(ReceivedEvents[3], Is.EqualTo(Property.NameOf((IAccountSettings a) => a.Password)));
        }

        [Test]
        public void Load_TriggersEvent() {
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
            // Save Event Handler
            bool IsTriggered = false;
            underTest.SettingsSaved += (sender, arg) => {
                IsTriggered = true;
            };
            // Save 
            underTest.Save();

            Assert.That(IsTriggered, Is.True);
        }
    }
}
