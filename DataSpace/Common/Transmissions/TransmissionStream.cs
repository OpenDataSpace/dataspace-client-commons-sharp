//-----------------------------------------------------------------------
// <copyright file="TransmissionStream.cs" company="GRAU DATA AG">
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

namespace DataSpace.Common.Transmissions {
    using System;
    using System.ComponentModel;
    using System.IO;

    using DataSpace.Common.Streams;
    using DataSpace.Common.Utils;

    /// <summary>
    /// Transmission stream should only be used by unit tests and indirectly by calling Transmission.CreateStream.
    /// </summary>
    public class TransmissionStream : Stream {
        private BandwidthNotifyingStream bandwidthNotify;
        private PausableStream pause;
        private AbortableStream abort;
        private ProgressStream progress;
        private BandwidthLimitedStream bandwidthLimit;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="CmisSync.Lib.Streams.TransmissionStream"/> class.
        /// </summary>
        /// <param name="wrappedStream">Wrapped stream.</param>
        /// <param name="transmission">Transmission object to be notified about changes and listened to events as well.</param>
        public TransmissionStream(Stream wrappedStream, Transmission transmission) {
            if (transmission == null) {
                throw new ArgumentNullException("transmission");
            }

            this.abort = new AbortableStream(wrappedStream);
            this.pause = new PausableStream(this.abort);
            this.bandwidthLimit = new BandwidthLimitedStream(this.pause);
            this.bandwidthNotify = new BandwidthNotifyingStream(this.bandwidthLimit);
            this.progress = new ProgressStream(this.bandwidthNotify);
            this.abort.PropertyChanged += (object sender, PropertyChangedEventArgs e) => {
                var a = sender as AbortableStream;
                if (e.PropertyName == Property.NameOf(() => a.Exception)) {
                    transmission.Status = TransmissionStatus.Aborted;
                    transmission.FailedException = a.Exception;
                }
            };
            this.bandwidthNotify.PropertyChanged += (object sender, PropertyChangedEventArgs e) => {
                var s = sender as BandwidthNotifyingStream;
                if (e.PropertyName == Property.NameOf(() => s.BitsPerSecond)) {
                    transmission.BitsPerSecond = s.BitsPerSecond;
                }
            };
            this.progress.PropertyChanged += (object sender, PropertyChangedEventArgs e) => {
                var p = sender as ProgressStream;
                if (e.PropertyName == Property.NameOf(() => p.Position)) {
                    transmission.Position = p.Position;
                } else if (e.PropertyName == Property.NameOf(() => p.Length)) {
                    transmission.Length = p.Length;
                }
            };
            transmission.PropertyChanged += (object sender, PropertyChangedEventArgs e) => {
                var t = sender as Transmission;
                if (e.PropertyName == Property.NameOf(() => t.Status)) {
                    if (t.Status == TransmissionStatus.Aborting) {
                        this.abort.Abort();
                        this.pause.Resume();
                    } else if (t.Status == TransmissionStatus.Paused) {
                        this.pause.Pause();
                    } else if (t.Status == TransmissionStatus.Transmitting) {
                        this.pause.Resume();
                    }
                } else if (e.PropertyName == Property.NameOf(() => t.MaxBandwidth)) {
                    if (t.MaxBandwidth > 0) {
                        this.bandwidthLimit.ReadLimit = t.MaxBandwidth;
                        this.bandwidthLimit.WriteLimit = t.MaxBandwidth;
                    } else {
                        this.bandwidthLimit.DisableLimits();
                    }
                }
            };
            if (transmission.Status == TransmissionStatus.Aborting || transmission.Status == TransmissionStatus.Aborted) {
                this.abort.Abort();
            }

            if (transmission.MaxBandwidth > 0) {
                this.bandwidthLimit.ReadLimit = transmission.MaxBandwidth;
                this.bandwidthLimit.WriteLimit = transmission.MaxBandwidth;
            }
        }

        #region boilerplate
        /// <summary>
        /// Gets a value indicating whether the wrapped instance can read.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can read; otherwise, <c>false</c>.
        /// </value>
        public override bool CanRead {
            get {
                return progress.CanRead;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the wrapped instance can seek.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can seek; otherwise, <c>false</c>.
        /// </value>
        public override bool CanSeek {
            get {
                return this.progress.CanSeek;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the wrapped instance can write.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can write; otherwise, <c>false</c>.
        /// </value>
        public override bool CanWrite {
            get {
                return this.progress.CanWrite;
            }
        }

        /// <summary>
        /// Gets the length of the the wrapped instance.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override long Length {
            get {
                return this.progress.Length;
            }
        }

        /// <summary>
        /// Gets or sets the position of the wrapped instance.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public override long Position {
            get {
                return this.progress.Position;
            }

            set {
                if (this.disposed) {
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                this.progress.Position = value;
            }
        }

        /// <summary>
        /// Flush the wrapped instance.
        /// </summary>
        public override void Flush() {
            if (this.disposed) {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.progress.Flush();
        }

        /// <summary>
        /// Begins the read on the wrapped instance.
        /// </summary>
        /// <returns>
        /// The result of the the wrapped instance call.
        /// </returns>
        /// <param name='buffer'>
        /// Buffer passed to the wrapped instance.
        /// </param>
        /// <param name='offset'>
        /// Offset passed to the wrapped instance.
        /// </param>
        /// <param name='count'>
        /// Count passed to the wrapped instance.
        /// </param>
        /// <param name='callback'>
        /// Callback passed to the wrapped instance.
        /// </param>
        /// <param name='state'>
        /// State passed to the wrapped instance.
        /// </param>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
            if (this.disposed) {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            return this.progress.BeginRead(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// Seeks on the wrapped instance with the specified offset and origin.
        /// </summary>
        /// <param name='offset'>
        /// Offset passed to the wrapped instance.
        /// </param>
        /// <param name='origin'>
        /// Origin passed to the wrapped instance.
        /// </param>
        /// <returns>
        /// The result of the call passed to the wrapped instance.
        /// </returns>
        public override long Seek(long offset, SeekOrigin origin) {
            if (this.disposed) {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            return this.progress.Seek(offset, origin);
        }

        /// <summary>
        /// Read the specified buffer, offset and count.
        /// </summary>
        /// <param name='buffer'>
        /// Buffer passed to the wrapped instance.
        /// </param>
        /// <param name='offset'>
        /// Offset passed to the wrapped instance.
        /// </param>
        /// <param name='count'>
        /// Count passed to the wrapped instance.
        /// </param>
        /// <returns>
        /// The result of the call passed to the wrapped instance.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count) {
            if (this.disposed) {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            return this.progress.Read(buffer, offset, count);
        }

        /// <summary>
        /// Sets the length of the wrapped instance.
        /// </summary>
        /// <param name='value'>
        /// Value passed to the wrapped instance.
        /// </param>
        public override void SetLength(long value) {
            if (this.disposed) {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.progress.SetLength(value);
        }

        /// <summary>
        /// Write the specified buffer, offset and count.
        /// </summary>
        /// <param name='buffer'>
        /// Buffer passed to the wrapped instance.
        /// </param>
        /// <param name='offset'>
        /// Offset passed to the wrapped instance.
        /// </param>
        /// <param name='count'>
        /// Count passed to the wrapped instance.
        /// </param>
        public override void Write(byte[] buffer, int offset, int count) {
            if (this.disposed) {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            this.progress.Write(buffer, offset, count);
        }
        #endregion

        /// <summary>
        /// Dispose the transmission stream by disposing all internal streams.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c> all stream will be disposed.</param>
        protected override void Dispose(bool disposing) {
            if (!this.disposed) {
                if (disposing) {
                    if (this.pause != null) {
                        this.pause.Resume();
                        this.pause.Dispose();
                    }

                    if (this.progress != null) {
                        this.progress.Dispose();
                    }

                    if (this.bandwidthNotify != null) {
                        this.bandwidthNotify.Dispose();
                    }

                    if (this.abort != null) {
                        this.abort.Dispose();
                    }
                }

                this.disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}