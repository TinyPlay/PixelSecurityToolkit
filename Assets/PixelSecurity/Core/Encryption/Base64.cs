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

namespace PixelSecurity.Core.Encryption
{
    /// <summary>
    /// Base64 Data Encryption
    /// </summary>
    public class Base64 : IDataEncryption
    {
        /// <summary>
        /// Base64 Constructor
        /// </summary>
        public Base64(){}
        
        /// <summary>
        /// Encode String Data
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public string EncodeString(string plane)
        {
            byte[] bytesToEncode = Encoding.UTF8.GetBytes (plane);
            string encodedText = Convert.ToBase64String (bytesToEncode);
            return encodedText;
        }
        
        /// <summary>
        /// Encrypt Data Bytes
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public byte[] EncodeBinary(byte[] src)
        {
            string encodedText = Convert.ToBase64String(src);
            byte[] encodedBytes = Encoding.UTF8.GetBytes (encodedText);
            return encodedBytes;
        }
        
        /// <summary>
        /// Decrypt String
        /// </summary>
        /// <param name="encrypted"></param>
        /// <returns></returns>
        public string DecodeString(string encrypted)
        {
            byte[] decodedBytes = Convert.FromBase64String (encrypted);
            string decodedText = Encoding.UTF8.GetString (decodedBytes);
            return decodedText;
        }
        
        /// <summary>
        /// Decrypt Binary Data
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public byte[] DecodeBinary(byte[] src)
        {
            string encoded = Encoding.UTF8.GetString(src);
            byte[] decodedBytes = Convert.FromBase64String(encoded);
            return decodedBytes;
        }
    }
}