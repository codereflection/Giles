using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Giles.Core.Encryption
{
    /// <summary>
    /// Stolen from http://www.codeplex.com/CodePlexClient/SourceControl/changeset/view/18337#59622
    /// and changed a little to match our naming conventions
    /// </summary>
    public static class EncryptionHelper
    {
        static readonly byte[] entropy = { 4, 17, 253, 94, 16 };

        public static string DecryptString(string stringToDecrypt)
        {
            byte[] result = ProtectedData.Unprotect(Convert.FromBase64String(stringToDecrypt), entropy, DataProtectionScope.CurrentUser);
            return Encoding.Unicode.GetString(result);
        }

        public static string EncryptString(string stringToEncrypt)
        {
            byte[] result = ProtectedData.Protect(Encoding.Unicode.GetBytes(stringToEncrypt), entropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(result);
        }

        public static byte[] GetHash(Stream stream)
        {
            return new MD5CryptoServiceProvider().ComputeHash(stream);
        }
    }
}