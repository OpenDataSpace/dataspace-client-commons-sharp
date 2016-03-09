//-----------------------------------------------------------------------
// <copyright file="PausableStream.cs" company="GRAU DATA AG">
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

namespace DataSpace.Common.Streams {
    using System;
    using System.IO;
    using System.Threading;

    /// <summary>
    /// Pausable stream takes the given stream and causes the reading or writing thread to pause if pause is called until resume is called.
    /// </summary>
    public class PausableStream : StreamWrapper {
        private ManualResetEventSlim waitHandle = new ManualResetEventSlim(true);
        private CancellationTokenSource cancelTaskSource;
        private CancellationToken cancelToken;
        /// <summary>
        /// Initializes a new instance of the <see cref="CmisSync.Lib.Streams.PausableStream"/> class.
        /// </summary>
        /// <param name="s">Stream which should be wrapped and extended by the possibility to be paused on read or write by another thread.</param>
        public PausableStream(Stream s) : base(s) {
            this.cancelTaskSource = new CancellationTokenSource();
            this.cancelToken = this.cancelTaskSource.Token;
        }

        /// <summary>
        /// Write the specified buffer, offset and count.
        /// </summary>
        /// <param name='buffer'>
        /// Buffer.
        /// </param>
        /// <param name='offset'>
        /// Offset.
        /// </param>
        /// <param name='count'>
        /// Count.
        /// </param>
        public override void Write(byte[] buffer, int offset, int count) {
            // for it may be chained before CryptoStream, we should write the content for CryptoStream has calculated the hash of the content
            this.Stream.Write(buffer, offset, count);

            // Pause here
            this.waitHandle.Wait(this.cancelToken);
        }

        /// <summary>
        /// Read the specified buffer, offset and count.
        /// </summary>
        /// <param name='buffer'>
        /// Buffer.
        /// </param>
        /// <param name='offset'>
        /// Offset.
        /// </param>
        /// <param name='count'>
        /// Count.
        /// </param>
        public override int Read(byte[] buffer, int offset, int count) {
            // Pause here
            this.waitHandle.Wait(this.cancelToken);

            return this.Stream.Read(buffer, offset, count);
        }

        /// <summary>
        /// Pause this stream until resume is called.
        /// </summary>
        public void Pause() {
            this.waitHandle.Reset();
        }

        /// <summary>
        /// Resume this stream.
        /// </summary>
        public void Resume() {
            this.waitHandle.Set();
        }
    }
}