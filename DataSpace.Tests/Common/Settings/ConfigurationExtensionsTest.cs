//-----------------------------------------------------------------------
// <copyright file="ConfigurationExtensionsTest.cs" company="GRAU DATA AG">
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
    using System.Configuration;

    using DataSpace.Common.Settings;

    using Moq;

    using NUnit.Framework;

    [TestFixture, Category("UnitTests")]
    public class ConfigurationExtensionsTest : WithGeneratedConfig {
        [Test]
        public void GetSectionInGroup() {
            var groupName = "group";
            var sectionName = "section";
            var fullSectionName = groupName + "/" + sectionName;
            var group = config.GetOrCreateSectionGroup<ConfigurationSectionGroup>(groupName);
            Assert.That(group, Is.Not.Null);
            var section = Mock.Of<ConfigurationSection>();
            group.Sections.Add(sectionName, section);
            Assert.That(config.GetSection(groupName + "/nonExistingSection"), Is.Null);
            Assert.That(config.GetSection("nonExistingSection"), Is.Null);
            Assert.That(config.GetSection(fullSectionName), Is.EqualTo(section));
            Assert.That(config.GetOrCreateSection<ConfigurationSection>(fullSectionName), Is.EqualTo(section));
            Assert.That(config.GetSection(fullSectionName).SectionInformation.Name, Is.EqualTo(sectionName));
            Assert.That(config.GetSection(fullSectionName).SectionInformation.SectionName, Is.EqualTo(fullSectionName));
        }

        [Test]
        public void CreateSectionWithGroupPathCreatesGroup() {
            var groupName = "group";
            var sectionName = "section";
            var section = config.GetOrCreateSection<TestSection>(groupName + "/" + sectionName);
            Assert.That(section, Is.Not.Null);
            Assert.That(config.SectionGroups.Get(groupName), Is.Not.Null);
        }

        private class TestSection : ConfigurationSection {
        }
    }
}