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
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PixelSecurity.Core.Encryption
{
    /// <summary>
    /// RSA Encryption Provider
    /// </summary>
    public class RSA : IDataEncryption
    {
        [System.Serializable]
        public class EncryptionOptions : IEncryptionOptions
        {
            public string PublicKey = "RSAPublicKey";
            public string PrivateKey = "RSAPrivateKey";
        }

        private static EncryptionOptions _encryptor = new EncryptionOptions();

        /// <summary>
        /// RSA Encryption Provider
        /// </summary>
        /// <param name="options"></param>
        public RSA(EncryptionOptions options)
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
            byte[] encrypted = EncodeBinary(Encoding.UTF8.GetBytes(plane));
            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Encrypt Data Bytes
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public byte[] EncodeBinary(byte[] src)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(_encryptor.PublicKey);
                byte[] encrypted = rsa.Encrypt(src, false);
                return encrypted;
            }
        }

        /// <summary>
        /// Decrypt String
        /// </summary>
        /// <param name="encrypted"></param>
        /// <returns></returns>
        public string DecodeString(string encrypted)
        {
            byte[] decrypted = DecodeBinary(Convert.FromBase64String(encrypted));
            return Encoding.UTF8.GetString(decrypted);
        }

        /// <summary>
        /// Decrypt Binary Data
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public byte[] DecodeBinary(byte[] src)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(_encryptor.PrivateKey);
                byte[] decrypted = rsa.Decrypt(src, false);
                return decrypted;
            }
        }

        /// <summary>
        /// Generate Key Pair
        /// </summary>
        /// <param name="keySize"></param>
        /// <returns></returns>
        public static KeyValuePair<string, string> GenrateKeyPair(int keySize)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize);
            string publicKey = rsa.ToXmlString(false);
            string privateKey = rsa.ToXmlString(true);
            return new KeyValuePair<string, string>(publicKey, privateKey);
        }
    }
}