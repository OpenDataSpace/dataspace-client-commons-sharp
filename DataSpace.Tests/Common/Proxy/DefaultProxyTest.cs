//-----------------------------------------------------------------------
// <copyright file="DefaultProxyTest.cs" company="GRAU DATA AG">
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

namespace Tests.Common.Proxy {
    using System;
    using System.Net;

    using DataSpace.Common.Crypto;
    using DataSpace.Common.Proxy;

    using Moq;

    using NUnit.Framework;

    [TestFixture, Category("UnitTests")]
    public class DefaultProxyTest {
        private ProxySettings settings;
        private readonly string proxyUrl = "http://example.com:8080/";

        [SetUp]
        public void SetUp() {
            DefaultProxy.SetSystemDefaultProxy();
            settings = new ProxySettings();
        }

        [TearDown]
        public void RestoreDefaultSystemProxy() {
            DefaultProxy.SetSystemDefaultProxy();
        }

        [Test]
        public void SetCustomDefaultProxy(
            [Values(true, false)]bool loginRequired)
        {
            settings.Selection = DataSpace.Common.Proxy.Type.CUSTOM;
            settings.Server = new Uri(proxyUrl);
            settings.LoginRequired = loginRequired;
            settings.Username = loginRequired ? "testuser" : null;
            settings.ObfuscatedPassword = loginRequired ? "password".Obfuscate() : null;
            settings.SetAsDefaultProxy();
            Assert.That(WebRequest.DefaultWebProxy, Is.Not.Null);
            Assert.That(WebRequest.DefaultWebProxy.Credentials, loginRequired ? Is.Not.Null : Is.Null);
        }

        [Test]
        public void DisableAnyDefaultProxy() {
            DefaultProxy.SetCustomProxy(to: new WebProxy(proxyUrl));
            settings.Selection = DataSpace.Common.Proxy.Type.NOPROXY;
            settings.SetAsDefaultProxy();
            Assert.That(HttpWebRequest.DefaultWebProxy, Is.Null);
        }

        [Test]
        public void RestoreSystemDefaultAfterCustomProxy() {
            IWebProxy originalProxy = WebRequest.DefaultWebProxy;
            DefaultProxy.SetCustomProxy(to: new WebProxy(proxyUrl));
            Assert.That(WebRequest.DefaultWebProxy, Is.Not.EqualTo(originalProxy));
            DefaultProxy.SetSystemDefaultProxy();
            Assert.That(WebRequest.DefaultWebProxy, Is.EqualTo(originalProxy));
        }

        [Test]
        public void PassingNullAsCustomProxyThrowsExceptionAndDoesNotModifyConfiguredProxy() {
            DefaultProxy.SetCustomProxy(to: new WebProxy(proxyUrl));
            var oldProxy = WebRequest.DefaultWebProxy;
            Assert.Throws<ArgumentNullException>(() => DefaultProxy.SetCustomProxy(null));
            Assert.That(WebRequest.DefaultWebProxy, Is.EqualTo(oldProxy));
        }
    }
}