//-----------------------------------------------------------------------
// <copyright file="UserConfigPathBuilderTest.cs" company="GRAU DATA AG">
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
namespace Tests.Common.Settings {
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;

    using DataSpace.Common.Settings;
    using DataSpace.Tests.Utils;

    using NUnit.Framework;

    [TestFixture]
    public class UserConfigPathBuilderTest : WithConfiguredLog4Net {
        [Test]
        public void CreatePathWithDifferentTypesCreatesDifferentPaths() {
            var paths = new HashSet<string>();
            var underTest = new UserConfigPathBuilder();
            underTest.SettingsType = ConfigurationUserLevel.None;
            paths.Add(underTest.CreatePath());
            underTest.SettingsType = ConfigurationUserLevel.PerUserRoaming;
            paths.Add(underTest.CreatePath());
            underTest.SettingsType = ConfigurationUserLevel.PerUserRoamingAndLocal;
            paths.Add(underTest.CreatePath());
            Assert.That(paths.Count, Is.EqualTo(3));
        }

        [Test]
        public void CreatedPathContainsCompanyAndProductAndFileName(
            [Values(ConfigurationUserLevel.None, ConfigurationUserLevel.PerUserRoaming, ConfigurationUserLevel.PerUserRoamingAndLocal)]ConfigurationUserLevel type,
            [Values("DataSpace", "Generic Product")]string product,
            [Values("GrauData", "Generic Company")]string company)
        {
            var fileName = Guid.NewGuid().ToString();
            IUserConfigPathBuilder underTest = new UserConfigPathBuilder() { SettingsType = type, Company = company, Product = product, FileName = fileName };
            var path = underTest.CreatePath();
            Assert.That(path, Contains.Substring(company));
            Assert.That(path, Contains.Substring(product));
            Assert.That(path, Contains.Substring(fileName + ".config"));
            Assert.That(path, Is.EqualTo(Path.GetFullPath(path)));
        }
    }
}