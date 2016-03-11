﻿//-----------------------------------------------------------------------
// <copyright file="AbortableStream.cs" company="GRAU DATA AG">
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
    using System.ComponentModel;
    using System.IO;
    using System.Threading;

    using DataSpace.Common.Utils;

    /// <summary>
    /// Abortable stream wraps the given stream and add the possibility to abort the stream read and write by throwing an exception.
    /// </summary>
    public class AbortableStream : NotifyPropertyChangedStream {
        private int aborted = 0;
        private AbortedException exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSpace.Common.Streams.AbortableStream"/> class.
        /// </summary>
        /// <param name="s">Stream which should be abortable.</param>
        public AbortableStream(Stream s) : base(s) {
        }

        /// <summary>
        /// Gets the exception if the stream communication is aborted. Otherwise null.
        /// </summary>
        /// <value>The exception.</value>
        public AbortedException Exception {
            get {
                return this.exception;
            }

            private set {
                if (this.exception != value) {
                    this.exception = value;
                    this.NotifyPropertyChanged(Property.NameOf(() => this.Exception));
                }
            }
        }

        /// <summary>
        /// Read the specified buffer, offset and count.
        /// </summary>
        /// <param name='buffer'>
        /// Buffer to be written into.
        /// </param>
        /// <param name='offset'>
        /// Offset inside the given buffer.
        /// </param>
        /// <param name='count'>
        /// Count of bytes to be read.
        /// </param>
        /// <returns>The count of read bytes.</returns>
        public override int Read(byte[] buffer, int offset, int count) {
            if (this.aborted > 0) {
                this.Exception = new AbortedException();
                throw this.exception;
            }

            return this.Stream.Read(buffer, offset, count);
        }

        /// <summary>
        /// Write the specified buffer, offset and count.
        /// </summary>
        /// <param name='buffer'>
        /// Buffer to be written to stream.
        /// </param>
        /// <param name='offset'>
        /// Offset in the given buffer.
        /// </param>
        /// <param name='count'>
        /// Count of bytes to be written to stream.
        /// </param>
        public override void Write(byte[] buffer, int offset, int count) {
            if (this.aborted > 0) {
                this.Exception = new AbortedException();
                throw this.exception;
            }

            this.Stream.Write(buffer, offset, count);
        }

        /// <summary>
        /// Abort this instance.
        /// </summary>
        public void Abort() {
            Interlocked.Increment(ref this.aborted);
        }
    }
}