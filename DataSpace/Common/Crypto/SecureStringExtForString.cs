using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace DataSpace.Common.Crypto
{
    /// <summary>
    /// SecureString Extensions for usage with standart string
    /// </summary>
    public static class SecureStringExtForString
    {
        /// <summary>
        /// converts SecureString in normal String
        /// </summary>
        /// <param name="securePassword">Secure Password object</param>
        /// <returns>Password as normal String</returns>
        public static string ConvertToUnsecureString(this SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException("securePassword");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
        /// <summary>
        /// initialize SecureString with normal string
        /// </summary>
        /// <param name="securePassword">Secure Password object</param>
        /// <param name="unsecurePassword">Password as normal String</param>
        /// <returns>initialized Secure Password</returns>
        public static SecureString Init(this SecureString securePassword, string unsecurePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException("securePassword");
            // Change null to empty Pwd
            unsecurePassword = unsecurePassword ?? string.Empty;

            securePassword.Clear();
            foreach (char Letter in unsecurePassword)
            {
                securePassword.AppendChar(Letter);
            }
            return securePassword;
        }
    }
}
