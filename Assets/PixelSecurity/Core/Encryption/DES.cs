/*
 * Pixel Security Toolkit
 * This is the free and open-source security
 * library with different modules to secure your
 * application.
 *
 * @developer       TinyPlay Games
 * @author          Ilya Rastorguev
 * @version         1.0.0
 * @build           1004
 * @url             https://github.com/TinyPlay/PixelSecurityToolkit/
 * @support         hello@flowsourcebox.com
 */

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PixelSecurity.Core.Encryption
{
    /// <summary>
    /// DES Encryption Class
    /// </summary>
    public class DES  : IDataEncryption
    {
        [System.Serializable]
        public class EncryptionOptions : IEncryptionOptions
        {
            public string Password = "ABCDEFGH";
        }
        private static EncryptionOptions _encryptor = new EncryptionOptions();

        /// <summary>
        /// DES Encryption Provider
        /// </summary>
        /// <param name="options"></param>
        public DES(EncryptionOptions options)
        {
            _encryptor = options;
        }
        
        /// <summary>
        /// Encode String Data
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public string EncodeString(string plane)
        {
            byte[] txtByteData = Encoding.UTF8.GetBytes(plane);
            byte[] encoded = EncodeBinary(txtByteData);
            return Convert.ToBase64String(encoded);
        }
        
        /// <summary>
        /// Encrypt Data Bytes
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public byte[] EncodeBinary(byte[] src)
        {
            byte[] keyByteData = Encoding.UTF8.GetBytes(_encryptor.Password);
            DESCryptoServiceProvider DEScryptoProvider = new DESCryptoServiceProvider();
            ICryptoTransform trnsfrm = DEScryptoProvider.CreateEncryptor(keyByteData, keyByteData);
            CryptoStreamMode mode = CryptoStreamMode.Write;
 
            //Set up Stream & Write Encript data
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, trnsfrm, mode);
            cStream.Write(src, 0, src.Length);
            cStream.FlushFinalBlock();
 
            //Read Ecncrypted Data From Memory Stream
            byte[] result = new byte[mStream.Length];
            mStream.Position = 0;
            mStream.Read(result, 0, result.Length);

            return result;
        }
        
        /// <summary>
        /// Decrypt String
        /// </summary>
        /// <param name="encrypted"></param>
        /// <returns></returns>
        public string DecodeString(string encrypted)
        {
            byte[] txtByteData = Convert.FromBase64String(encrypted);
            byte[] decoded = DecodeBinary(txtByteData);
            return Encoding.UTF8.GetString(decoded);
        }
        
        /// <summary>
        /// Decrypt Binary Data
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public byte[] DecodeBinary(byte[] src)
        {
            byte[] keyByteData = Encoding.UTF8.GetBytes(_encryptor.Password);
 
            DESCryptoServiceProvider DEScryptoProvider = new DESCryptoServiceProvider();
            ICryptoTransform trnsfrm = DEScryptoProvider.CreateDecryptor(keyByteData, keyByteData);
            CryptoStreamMode mode = CryptoStreamMode.Write;
 
            //Set up Stream & Write Encript data
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, trnsfrm, mode);
            cStream.Write(src, 0, src.Length);
            cStream.FlushFinalBlock();
 
            //Read Ecncrypted Data From Memory Stream
            byte[] result = new byte[mStream.Length];
            mStream.Position = 0;
            mStream.Read(result, 0, result.Length);
 
            return result;
        }
    }
}