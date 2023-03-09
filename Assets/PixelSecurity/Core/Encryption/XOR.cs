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
using System.Text;
using PixelSecurity.Constants;

namespace PixelSecurity.Core.Encryption
{
    /// <summary>
    /// XOR Encryption Class
    /// </summary>
    public class XOR  : IDataEncryption
    {
        [System.Serializable]
        public class EncryptionOptions : IEncryptionOptions
        {
            public string Password = "123456";
        }
        private static EncryptionOptions _encryptor = new EncryptionOptions();

        /// <summary>
        /// XOR Encryption Provider
        /// </summary>
        /// <param name="options"></param>
        public XOR(EncryptionOptions options)
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
            return EncryptOrDecrypt(plane, _encryptor.Password);
        }
        
        /// <summary>
        /// Encrypt Data Bytes
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public byte[] EncodeBinary(byte[] src)
        {
            throw new Exception(TextCodes.UNSUPPORTED_FOR_NON_STRING);
        }
        
        /// <summary>
        /// Decrypt String
        /// </summary>
        /// <param name="encrypted"></param>
        /// <returns></returns>
        public string DecodeString(string encrypted)
        {
            return EncryptOrDecrypt(encrypted, _encryptor.Password);
        }
        
        /// <summary>
        /// Decrypt Binary Data
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public byte[] DecodeBinary(byte[] src)
        {
            throw new Exception(TextCodes.UNSUPPORTED_FOR_NON_STRING);
        }
        
        /// <summary>
        /// Encrypt / Decrypt XOR
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private string EncryptOrDecrypt(string text, string key)
        {
            var result = new StringBuilder();
            for (int c = 0; c < text.Length; c++)
            {
                char character = text[c];
                uint charCode = (uint)character;
                int keyPosition = c % key.Length;
                char keyChar = key[keyPosition];
                uint keyCode = (uint)keyChar;
                uint combinedCode = charCode ^ keyCode;
                char combinedChar = (char)combinedCode;
                result.Append(combinedChar);
            }
            return result.ToString();
        }
    }
}