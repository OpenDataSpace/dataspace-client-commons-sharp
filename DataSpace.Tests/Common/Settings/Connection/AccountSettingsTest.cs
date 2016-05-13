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
using System.Linq;

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
        private IAccount underTest;

        [SetUp]
        public void CreateAccountInstance() {
            underTest = config.AddDataSpaceAccount(_Url, _UserName, new SecureString().Init(_Password));
            config.Save();
        }

        [TearDown]
        public void RemoveAccountInstance() {
            if (underTest != null) {
                underTest.Delete();
            }
        }


        [Test]
        public void Constructor() {
            // Assert
            Assert.That(underTest.Url, Is.EqualTo(_Url));
            Assert.That(underTest.UserName, Is.EqualTo(_UserName));
            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.EqualTo(_Password));
        }

        [Test]
        public void PropertyGetSet() {
            string anotherPassword = Guid.NewGuid().ToString();
            // act
            underTest.Password = new System.Security.SecureString().Init(anotherPassword);
            // assert
            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.EqualTo(anotherPassword));
        }

        [Test]
        public void WriteAndRead() {
            underTest = config.GetDataSpaceAccounts().First().Value;

            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.EqualTo(_Password));
            Assert.That(underTest.Url, Is.EqualTo(_Url));
            Assert.That(underTest.UserName, Is.EqualTo(_UserName));
        }

        [Test]
        public void Check_OnPropertyChanged_Success() {
            List<string> ReceivedEvents = new List<string>();

            underTest.PropertyChanged += delegate (object sender, PropertyChangedEventArgs args) {
                ReceivedEvents.Add(args.PropertyName);
            };
            // Act
            underTest.Password = new System.Security.SecureString().Init(_Password + " changed");

            // Assert
            Assert.That(underTest.Url, Is.EqualTo(_Url));
            Assert.That(underTest.UserName, Is.EqualTo(_UserName));
            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.EqualTo(_Password + " changed"));

            Assert.That(ReceivedEvents.Count, Is.EqualTo(1));
            Assert.That(ReceivedEvents[0], Is.EqualTo(Property.NameOf((IAccount a) => a.Password)));
        }

        [Test]
        public void DeleteTriggersEvent() {
            // Delete Event Handler
            bool IsTriggered = false;
            underTest.SettingsDeleted += (sender, arg) => {
                IsTriggered = true;
            };
            // Save 
            underTest.Delete();
            Assert.That(IsTriggered, Is.True);
        }
    }
}