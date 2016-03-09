﻿//-----------------------------------------------------------------------
// <copyright file="AbortableStreamTest.cs" company="GRAU DATA AG">
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

namespace Tests.Common.Streams {
    using System;
    using System.IO;

    using DataSpace.Common.Streams;
    using DataSpace.Common.Utils;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class AbortableStreamTest {
        [Test, Category("Fast"), Category("Streams")]
        public void ReadingStreamWithoutAbortionWorks() {
            var length = 1024 * 1024;
            var content = new byte[length];
            using (var inputStream = new MemoryStream(content))
            using (var outputStream = new MemoryStream())
            using (var underTest = new AbortableStream(inputStream)) {
                underTest.CopyTo(outputStream);
                Assert.That(outputStream.Length, Is.EqualTo(length));
            }
        }

        [Test, Category("Fast"), Category("Streams")]
        public void WritingStreamWithoutAbortionWorks() {
            var length = 1024 * 1024;
            var content = new byte[length];
            using (var inputStream = new MemoryStream(content))
            using (var outputStream = new MemoryStream())
            using (var underTest = new AbortableStream(outputStream)) {
                inputStream.CopyTo(underTest);
                Assert.That(outputStream.Length, Is.EqualTo(length));
            }
        }

        [Test, Category("Fast"), Category("Streams")]
        public void AbortReadIfAbortIsCalled() {
            byte[] content = new byte[1024];
            using (var stream = new Mock<MemoryStream>(content) { CallBase = true }.Object)
            using (var underTest = new AbortableStream(stream)) {
                underTest.Abort();
                Assert.Throws<AbortedException>(() => underTest.ReadByte());
                Mock.Get(stream).Verify(s => s.ReadByte(), Times.Never());
                Mock.Get(stream).Verify(s => s.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
            }
        }

        [Test, Category("Fast"), Category("Streams")]
        public void AbortWriteIfAbortIsCalled() {
            using (var inputStream = new MemoryStream(new byte[1024 * 1024 * 10]))
            using (var stream = new Mock<MemoryStream>() { CallBase = true }.Object)
            using (var underTest = new AbortableStream(stream)) {
                underTest.Abort();
                Assert.Throws<AbortedException>(() => inputStream.CopyTo(underTest));
                Mock.Get(stream).Verify(s => s.WriteByte(It.IsAny<byte>()), Times.Never());
                Mock.Get(stream).Verify(s => s.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
            }
        }

        [Test, Category("Fast"), Category("Streams")]
        public void NotificationSendOutOnAbortion() {
            bool notified = false;
            byte[] content = new byte[1024];
            using (var stream = new Mock<MemoryStream>(content) { CallBase = true }.Object)
            using (var underTest = new AbortableStream(stream)) {
                underTest.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => {
                    Assert.That(e.PropertyName, Is.EqualTo(Property.NameOf((AbortableStream s) => s.Exception)));
                    Assert.That(underTest.Exception, Is.Not.Null);
                    notified = true;
                };
                underTest.Abort();
                Assert.Throws<AbortedException>(() => underTest.ReadByte());
            }

            Assert.That(notified, Is.True);
        }
    }
}