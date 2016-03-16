
namespace DataSpace.Common.Transmissions {
    using System;

    /// <summary>
    /// File transmission types.
    /// </summary>
    public enum TransmissionType {
        /// <summary>
        /// A new file is uploaded
        /// </summary>
        UploadNewFile,

        /// <summary>
        /// A locally modified file is uploaded
        /// </summary>
        UploadModifiedFile,

        /// <summary>
        /// A new remote file is downloaded
        /// </summary>
        DownloadNewFile,

        /// <summary>
        /// A remotely modified file is downloaded
        /// </summary>
        DownloadModifiedFile
    }
}