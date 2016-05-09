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
﻿
namespace Tests.Common.Settings.Connection {
    using System;
    using System.Configuration;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.IO;

    using DataSpace.Common.Settings;
    using DataSpace.Common.Settings.Connection;
    using DataSpace.Common.Utils;

    using Moq;

    using NUnit.Framework;

    [TestFixture, NUnit.Framework.Category("UnitTests")]
    public class DataSpaceAccountCollectionTest : WithGeneratedConfig {
        [Test]
        public void NotificationOnElementChange() {
            int isCollectionTriggered = 0;
            int isElementTriggered = 0;
            string changedProperty = "bla";
            var underTest = new DataSpaceAccountCollection(config);
            var mockedEntry = new Mock<IAccountSettings>();
            underTest.Add(mockedEntry.Object);
            underTest.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => {
                Assert.That(sender, Is.EqualTo(underTest));
                Assert.That(e.Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
                isCollectionTriggered++;
            };

            mockedEntry.Object.PropertyChanged += (object sender, PropertyChangedEventArgs e) => {
                Assert.That(sender, Is.EqualTo(mockedEntry.Object));
                isElementTriggered++;
            };

            mockedEntry.Raise(m => m.PropertyChanged += null, new PropertyChangedEventArgs(changedProperty));
            Assert.That(isElementTriggered, Is.EqualTo(1));
            Assert.That(isCollectionTriggered, Is.EqualTo(0));
            Assert.That(underTest.Count, Is.EqualTo(1));
            underTest.Add(Mock.Of<IAccountSettings>());
            Assert.That(isCollectionTriggered, Is.EqualTo(1));
            Assert.That(underTest.Count, Is.EqualTo(2));
        }
    }
}