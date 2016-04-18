//-----------------------------------------------------------------------
// <copyright file="KeyChainTests.cs" company="GRAU DATA AG">
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
namespace Tests.Common.NativeKeyStore.MacOS {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DataSpace.Common.NativeKeyStore.MacOS;

    using MonoMac.Security;
    using MonoMac.AppKit;

    using NUnit.Framework;

    [TestFixture]
    public class KeyChainTests {
        private string user;
        private string password;
        private Keychain underTest;
        private string appName;

        [TestFixtureSetUp]
        public void ClassSetUp() {
            try {
                NSApplication.Init();
            } catch(InvalidOperationException) {
            }
        }

        [SetUp]
        public void SetUpNewKeyChainAccess() {
            user = "user" + Guid.NewGuid().ToString();
            password = "pw" + Guid.NewGuid().ToString();
            appName = "DataSpaceTest " + Guid.NewGuid().ToString();
            underTest =  new Keychain(appName);
        }

        [TearDown]
        public void Cleanup() {
            underTest.Clear();
        }

        [Test]
        public void Constructor() {
            Assert.That(underTest.Keys, Is.Empty);
            Assert.That(underTest.Values, Is.Empty);
            Assert.That(underTest.Count, Is.EqualTo(0));
            Assert.That(underTest.IsReadOnly, Is.False);
            Assert.That(underTest.GetEnumerator(), Is.Not.Null);
        }

        [Test]
        public void AddAndRemoveEntry() {
            underTest.Add(new KeyValuePair<string, string>(user, password));
            Assert.That(underTest[user], Is.EqualTo(password));
            Assert.That(underTest.Count, Is.EqualTo(1));
            Assert.That(underTest.Keys, Contains.Item(user));
            Assert.That(underTest.Values, Contains.Item(password));
            foreach (var entry in underTest) {
                Assert.That(entry.Key, Is.EqualTo(user));
                Assert.That(entry.Value, Is.EqualTo(password));
            }

            underTest.Remove(user);
            Assert.That(underTest.Count, Is.EqualTo(0));
            Assert.That(underTest, Is.Empty);
            Assert.That(underTest.Keys, Is.Empty);
            Assert.That(underTest.Values, Is.Empty);
        }

        [Test]
        public void UpdateEntry() {
            underTest.Add(new KeyValuePair<string, string>(user, password));
            var otherPassword = "other password";
            underTest [user] = otherPassword;
            Assert.That(underTest.Count, Is.EqualTo(1));
            Assert.That(underTest [user], Is.EqualTo(otherPassword));
        }

        [Test]
        public void RequestingNonExistingUser() {
            var otherUser = "other user";
            Assert.That(underTest.Contains(otherUser), Is.False);
            Assert.That(underTest.ContainsKey(otherUser), Is.False);
            Assert.That(underTest [otherUser], Is.Null);
            Assert.That(underTest.Remove(otherUser), Is.False);
        }

        [Test]
        public void KeyChainsAreCompletelyDisjunct() {
            var otherKeyChain = new Keychain(appName.Substring(0, appName.Length - 2));
            underTest [user] = password;
            Assert.That(otherKeyChain, Is.Empty);
        }

        [Test]
        public void CreateQuery() {
            new SecRecord(SecKind.GenericPassword) {};
        }
    }
}