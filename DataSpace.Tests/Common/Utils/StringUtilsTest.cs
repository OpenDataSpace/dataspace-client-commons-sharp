//-----------------------------------------------------------------------
// <copyright file="StringUtilsTest.cs" company="GRAU DATA AG">
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
namespace Tests.Common.Utils {
    using System;

    using DataSpace.Common.Utils;

    using NUnit.Framework;

    [TestFixture, Category("UnitTests")]
    public class StringUtilsTest {
        [Test, Sequential]
        public void NumberAsFormattedBandwidth(
            [Values(1, 2, 1000, 1100, 1499, 1500, 1000 * 1000, 1000 * 1000 * 1000, 1000 * 1000 * 1000 + 100 * 1000 * 1000)]long bitsPerSecond,
            [Values("1 Bit/s", "2 Bit/s", "1 KBit/s", "1.1 KBit/s", "1.5 KBit/s", "1.5 KBit/s", "1 MBit/s", "1 GBit/s", "1.1 GBit/s")]string expected)
        {
            double bitsPerSecondDouble = bitsPerSecond;
            Assert.That(bitsPerSecond.AsFormattedBandwidth(), Is.EqualTo(bitsPerSecondDouble.AsFormattedBandwidth()));
            Assert.That(bitsPerSecond.AsFormattedBandwidth().Replace(',', '.'), Contains.Substring(expected));
        }

        [Test, Sequential]
        public void NumberAsFormattedPercent(
            [Values(5.03d, 5.06d, 0.1d)]double input,
            [Values("5.0 %", "5.0 %", "0.1 %")]string expected)
        {
            Assert.That(input.AsFormattedPercent().Replace(',', '.'), Is.EqualTo(expected));
        }
    }
}