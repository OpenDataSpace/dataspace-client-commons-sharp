//-----------------------------------------------------------------------
// <copyright file="HttpProxyUtilsTest.cs" company="GRAU DATA AG">
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
    public class HttpProxyUtilsTest {
        private ProxySettings settings;

        [SetUp]
        public void SetUp() {
            this.settings = new ProxySettings();
        }

        [Test]
        public void SetDefaultProxyToHTTPWithoutCredentials() {
            this.settings.Selection = DataSpace.Common.Proxy.Type.CUSTOM;
            this.settings.LoginRequired = false;
            this.settings.Server = new Uri("http://example.com:8080/");
            DefaultProxy.SetDefaultProxy(this.settings);
            Assert.That(WebRequest.DefaultWebProxy, Is.Not.Null);
            Assert.That(WebRequest.DefaultWebProxy, Is.Not.EqualTo(WebRequest.GetSystemWebProxy()));
            Assert.That(WebRequest.DefaultWebProxy.Credentials, Is.Null);
        }

        [Test]
        public void SetDefaultProxyToHTTPWithCredentials() {
            this.settings.Selection = DataSpace.Common.Proxy.Type.CUSTOM;
            this.settings.Server = new Uri("http://example.com:8080/");
            this.settings.LoginRequired = true;
            this.settings.Username = "testuser";
            this.settings.ObfuscatedPassword = "password".Obfuscate();
            DefaultProxy.SetDefaultProxy(this.settings);
            Assert.That(WebRequest.DefaultWebProxy, Is.Not.Null);
            Assert.That(WebRequest.DefaultWebProxy.Credentials, Is.Not.Null);
        }

        [Test]
        public void SetDefaultProxyToBeIgnored() {
            this.settings.Selection = DataSpace.Common.Proxy.Type.CUSTOM;
            this.settings.LoginRequired = false;
            this.settings.Server = new Uri("http://example.com:8080/");
            DefaultProxy.SetDefaultProxy(this.settings);
            this.settings.Selection = DataSpace.Common.Proxy.Type.NOPROXY;
            DefaultProxy.SetDefaultProxy(this.settings);
            Assert.That(HttpWebRequest.DefaultWebProxy, Is.Null);
        }
    }
}