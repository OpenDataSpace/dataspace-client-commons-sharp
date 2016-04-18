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
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using DataSpace.Common.Crypto;
    using DataSpace.Common.Settings;
    using DataSpace.Common.Settings.Connection;
    using DataSpace.Common.Utils;
    using DataSpace.Tests.Utils;

    using NUnit.Framework;

    [TestFixture, NUnit.Framework.Category("UnitTests")]
    public class AccountSettingsTest : WithGeneratedConfig {
        private string _Url = "test.url.com";
        private string _UserName = "TestName";
        private string _Password = "TestPassword";

        [Test]
        public void Constructor() {
            IAccountSettings underTest = new AccountSettingsFactory().CreateInstance("DataSpaceAccount", config);
            underTest.Load();
            // Assert
            Assert.That(underTest.IsDirty, Is.False);
            Assert.That(underTest.Url, Is.EqualTo(string.Empty));
            Assert.That(underTest.UserName, Is.EqualTo(string.Empty));
            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void PropertyGetSet() {
            IAccountSettings underTest = new AccountSettingsFactory().CreateInstance("DataSpaceAccount", config);
            underTest.Load();
            // act
            underTest.Url = _Url;
            underTest.UserName = _UserName;
            underTest.Password = new System.Security.SecureString().Init(_Password);
            // assert
            Assert.That(underTest.Url, Is.EqualTo(_Url));
            Assert.That(underTest.UserName, Is.EqualTo(_UserName));
            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.EqualTo(_Password));
            Assert.That(underTest.IsDirty, Is.True);
        }

        [Test]
        public void WriteAndRead() {
            IAccountSettings underTest = new AccountSettingsFactory().CreateInstance("DataSpaceAccount", config);
            underTest.Load();
            underTest.Url = _Url;
            underTest.UserName = _UserName;
            underTest.Password = new System.Security.SecureString().Init(_Password);
            underTest.Save();

            underTest = new AccountSettingsFactory().CreateInstance("DataSpaceAccount", config);
            underTest.Load();
            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.EqualTo(_Password));
            Assert.That(underTest.Url, Is.EqualTo(_Url));
            Assert.That(underTest.UserName, Is.EqualTo(_UserName));
        }

        [Test]
        public void CreateNew() {
            IAccountSettings underTest = new AccountSettingsFactory().CreateInstance("DataSpaceAccount", config);
            underTest.Load();
            underTest.Url = _Url;
            underTest.UserName = _UserName;
            underTest.Password = new System.Security.SecureString().Init(_Password);
            underTest.Save();
        }

        [Test]
        public void Check_OnPropertyChanged_Success() {
            IAccountSettings underTest = new AccountSettingsFactory().CreateInstance("DataSpaceAccount", config);
            underTest.Load();
            List<string> ReceivedEvents = new List<string>();

            underTest.PropertyChanged += delegate (object sender, PropertyChangedEventArgs args) {
                ReceivedEvents.Add(args.PropertyName);
            };
            // Act
            underTest.Url = _Url;
            underTest.UserName = _UserName;
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
            IAccountSettings underTest = new AccountSettingsFactory().CreateInstance("DataSpaceAccount", config);

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
            IAccountSettings underTest = new AccountSettingsFactory().CreateInstance("DataSpaceAccount", config);
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
