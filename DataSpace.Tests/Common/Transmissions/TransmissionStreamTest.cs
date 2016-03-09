//-----------------------------------------------------------------------
// <copyright file="TransmissionStreamTest.cs" company="GRAU DATA AG">
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
namespace Tests.Common.Transmissions {
    using System;
    using System.IO;

    using DataSpace.Common.Streams;
    using DataSpace.Common.Transmissions;

    using NUnit.Framework;

    using Moq;

    [TestFixture, Category("UnitTests"), Category("Transmissions")]
    public class TransmissionStreamTest {
        [Test]
        public void ConstructorFailsIfStreamIsNull() {
            Assert.Throws<ArgumentNullException>(() => {using (new TransmissionStream(null, new Transmission(TransmissionType.DownloadModifiedFile, "path")));});
        }

        [Test]
        public void ConstructorFailsIfTransmissionIsNull() {
            Assert.Throws<ArgumentNullException>(() => {
                using (var stream = new MemoryStream())
                using (new TransmissionStream(stream, null));
            });
        }

        [Test]
        public void ConstructorTakesTransmissionAndStream() {
            var transmission = new Transmission(TransmissionType.DownloadModifiedFile, "path");
            using (var stream = new MemoryStream())
            using (var underTest = new TransmissionStream(stream, transmission)) {
            }
        }

        [Test]
        public void AbortReadIfAbortIsCalled() {
            var transmission = new Transmission(TransmissionType.DownloadModifiedFile, "path");
            using (var outputStream = new MemoryStream())
            using (var stream = new Mock<MemoryStream>() { CallBase = true }.Object)
            using (var underTest = new TransmissionStream(stream, transmission)) {
                transmission.Abort();
                Assert.Throws<AbortedException>(() => underTest.CopyTo(outputStream));
                Mock.Get(stream).Verify(s => s.ReadByte(), Times.Never());
                Mock.Get(stream).Verify(s => s.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
            }
        }

        [Test]
        public void AbortWriteIfAbortIsCalled() {
            var transmission = new Transmission(TransmissionType.DownloadModifiedFile, "path");
            using (var inputStream = new MemoryStream(new byte[1024 * 1024 * 10]))
            using (var stream = new Mock<MemoryStream>() { CallBase = true }.Object)
            using (var underTest = new TransmissionStream(stream, transmission)) {
                transmission.Abort();
                Assert.Throws<AbortedException>(() => inputStream.CopyTo(underTest));
                Mock.Get(stream).Verify(s => s.WriteByte(It.IsAny<byte>()), Times.Never());
                Mock.Get(stream).Verify(s => s.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
            }
        }

        [Test, Timeout(2200)]
        public void BandwidthLimitsArePropagatedAndUsed(
            [Values(true, false)]bool readFromTransmissionStream,
            [Values(true, false)]bool isBandwidthLimitedAfterInit)
        {
            int limit = 1024;
            int contentSize = 2 * limit;

            var transmission = new Transmission(TransmissionType.DownloadModifiedFile, "path");
            if (!isBandwidthLimitedAfterInit) {
                transmission.MaxBandwidth = limit;
            }

            using (var inputOrOutputStream = new MemoryStream(new byte[contentSize]))
            using (var stream = new MemoryStream(new byte[contentSize]))
            using (var underTest = new TransmissionStream(stream, transmission)) {
                transmission.PropertyChanged += (sender, e) => {
                    // limit * 2 is a workaround to handle monitoring of sliding time window
                    Assert.That(transmission.BitsPerSecond / 8, Is.Null.Or.LessThanOrEqualTo(limit * 2));
                };

                if (isBandwidthLimitedAfterInit) {
                    transmission.MaxBandwidth = limit;
                }

                if (readFromTransmissionStream) {
                    underTest.CopyTo(inputOrOutputStream);
                } else {
                    inputOrOutputStream.CopyTo(underTest);
                }
            }
        }
    }
}