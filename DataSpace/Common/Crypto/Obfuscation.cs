//-----------------------------------------------------------------------
// <copyright file="Obfuscation.cs" company="GRAU DATA AG">
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Obfuscation for sensitive data, making password harvesting a little less straightforward.
    /// Web browsers employ the same technique to store user passwords.
    /// </summary>
    public static class Obfuscation {
        /// <summary>
        /// Obfuscate a string.
        /// </summary>
        /// <param name="value">The string to obfuscate</param>
        /// <returns>The obfuscated string</returns>
        public static string Obfuscate(this string value) {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                return WindowsObfuscate(value);
            } else {
                return UnixObfuscate(value);
            }
        }

        /// <summary>
        /// Deobfuscate a string.
        /// </summary>
        /// <param name="value">The string to deobfuscate</param>
        /// <returns>The clear string</returns>
        public static string Deobfuscate(this string value) {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                return WindowsDeobfuscate(value);
            } else {
                return UnixDeobfuscate(value);
            }
        }

        /// <summary>
        /// Salt for the obfuscation.
        /// </summary>
        /// <returns>A byte array</returns>
        public static byte[] GetObfuscationKey() {
            return System.Text.Encoding.UTF8.GetBytes("Thou art so farth away, I miss you my dear files❥, with CmisSync be forever by my side!");
        }

        /// <summary>
        /// Obfuscate a string on Windows.
        /// We use the recommended API for this: DPAPI (Windows Data Protection API)
        /// http://msdn.microsoft.com/en-us/library/ms995355.aspx
        /// Warning: Even though it uses the Windows user's password, it is not uncrackable.
        /// </summary>
        /// <param name="value">The string to obfuscate</param>
        /// <returns>The obfuscated string</returns>
        private static string WindowsObfuscate(string value) {
            #if __MonoCS__
            // This macro prevents compilation errors on Unix where ProtectedData does not exist.
            return "Should never be reached";
            #else
            byte[] data = System.Text.Encoding.UTF8.GetBytes(value);

            // Encrypt the data using DataProtectionScope.CurrentUser. The result can be decrypted
            //  only by the same current user.
            byte[] crypt = ProtectedData.Protect(data, GetObfuscationKey(), DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(crypt, Base64FormattingOptions.None);
            #endif
        }

        /// <summary>
        /// Deobfuscate a string on Windows.
        /// </summary>
        /// <param name="value">The string to deobfuscate</param>
        /// <returns>The clear string</returns>
        private static string WindowsDeobfuscate(string value) {
            #if __MonoCS__
            // This macro prevents compilation errors on Unix where ProtectedData does not exist.
            throw new ApplicationException("Should never be reached");
            #else
                byte[] data = Convert.FromBase64String(value);

                // Decrypt the data using DataProtectionScope.CurrentUser.
                byte[] uncrypt = ProtectedData.Unprotect(data, GetObfuscationKey(), DataProtectionScope.CurrentUser);
                return System.Text.Encoding.UTF8.GetString(uncrypt);
            #endif
        }

        /// <summary>
        /// Obfuscate a string on Unix.
        /// AES is used.
        /// </summary>
        /// <param name="value">The string to obfuscate</param>
        /// <returns>The obfuscated string</returns>
        private static string UnixObfuscate(string value) {
            #if __MonoCS__
            using (PasswordDeriveBytes pdb = new PasswordDeriveBytes(GetObfuscationKey(), new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }))
            using (AesManaged myAes = new AesManaged()) {
                myAes.Key = pdb.GetBytes(myAes.KeySize / 8);
                myAes.IV = pdb.GetBytes(myAes.BlockSize / 8);
                using (ICryptoTransform encryptor = myAes.CreateEncryptor()) {
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(value);
                    byte[] crypt = encryptor.TransformFinalBlock(data, 0, data.Length);
                    return Convert.ToBase64String(crypt, Base64FormattingOptions.None);
                }
            }
            #else
            throw new ApplicationException("Should never be reached");
            #endif
        }

        /// <summary>
        /// Deobfuscate a string on UNIX.
        /// </summary>
        /// <param name="value">The string to deobfuscate</param>
        /// <returns>The clear string</returns>
        private static string UnixDeobfuscate(string value) {
            #if __MonoCS__
            using (PasswordDeriveBytes pdb = new PasswordDeriveBytes(GetObfuscationKey(), new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }))
            using (AesManaged myAes = new AesManaged()) {
                myAes.Key = pdb.GetBytes(myAes.KeySize / 8);
                myAes.IV = pdb.GetBytes(myAes.BlockSize / 8);
                using (ICryptoTransform decryptor = myAes.CreateDecryptor()) {
                    byte[] data = Convert.FromBase64String(value);
                    byte[] uncrypt = decryptor.TransformFinalBlock(data, 0, data.Length);
                    return System.Text.Encoding.UTF8.GetString(uncrypt);
                }
            }
            #else
            throw new ApplicationException("Should never be reached");
            #endif
        }
    }
}