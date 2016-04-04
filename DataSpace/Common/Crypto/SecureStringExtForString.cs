//-----------------------------------------------------------------------
// <copyright file="SecureStringExtForString.cs" company="GRAU DATA AG">
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

namespace DataSpace.Common.Crypto {
    ﻿using System;
    using System.Runtime.InteropServices;
    using System.Security;

    /// <summary>
    /// SecureString Extensions for usage with standart string
    /// </summary>
    public static class SecureStringExtForString {
        /// <summary>
        /// converts SecureString in normal String
        /// </summary>
        /// <param name="securePassword">Secure Password object</param>
        /// <returns>Password as normal String</returns>
        public static string ConvertToUnsecureString(this SecureString securePassword) {
            if (securePassword == null) {
                throw new ArgumentNullException("securePassword");
            }

            IntPtr unmanagedString = IntPtr.Zero;
            try {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            } finally {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
        /// <summary>
        /// initialize SecureString with normal string
        /// </summary>
        /// <param name="securePassword">Secure Password object</param>
        /// <param name="unsecurePassword">Password as normal String</param>
        /// <returns>initialized Secure Password</returns>
        public static SecureString Init(this SecureString securePassword, string unsecurePassword) {
            if (securePassword == null) {
                throw new ArgumentNullException("securePassword");
            }

            // Change null to empty Pwd
            unsecurePassword = unsecurePassword ?? string.Empty;

            securePassword.Clear();
            foreach (char Letter in unsecurePassword) {
                securePassword.AppendChar(Letter);
            }

            return securePassword;
        }
    }
}