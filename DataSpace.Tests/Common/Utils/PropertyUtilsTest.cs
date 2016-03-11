//-----------------------------------------------------------------------
// <copyright file="PropertyUtilsTest.cs" company="GRAU DATA AG">
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

namespace Tests.Common.Utils {
    using System;

    using DataSpace.Common.Utils;

    using NUnit.Framework;

    [TestFixture, Category("UnitTests")]
    public class PropertyUtilsTest {
        [Test]
        public void GetPropertyNameOfAnInstance() {
            var testClass = new TestClass();
            Assert.That(Property.NameOf(() => testClass.TestProperty), Is.EqualTo("TestProperty"));
        }

        [Test]
        public void GetProperyNameOfByPassingAClass() {
            Assert.That(Property.NameOf((TestClass t) => t.TestProperty), Is.EqualTo("TestProperty"));
        }

        private class TestClass {
            public string TestProperty { get; set; }
        }
    }
}