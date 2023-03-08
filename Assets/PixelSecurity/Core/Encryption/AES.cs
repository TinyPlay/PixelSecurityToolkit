using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

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
namespace PixelSecurity.Core.Encryption
{
    /// <summary>
    /// AES Encryption Class
    /// </summary>
    public class AES : IDataEncryption
    {
        [System.Serializable]
        public class EncryptionOptions : IEncryptionOptions
        {
            public int BufferKeySize = 32;
            public int BlockSize = 256;
            public int KeySize = 256;

            public string Password = "AESPassword";
        }
        private static EncryptionOptions _encryptor = new EncryptionOptions();

        /// <summary>
        /// AES Encryption Provider
        /// </summary>
        /// <param name="options"></param>
        public AES(EncryptionOptions options)
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
            RijndaelManaged rij = SetupRijndaelManaged;

            // A pseudorandom number is newly generated based on the inputted password
            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(_encryptor.Password, _encryptor.BufferKeySize);
            // The missing parts are specified in advance to fill in 0 length
            byte[] salt = new byte[_encryptor.BufferKeySize];
            // Rfc2898DeriveBytes gets an internally generated salt
            salt = deriveBytes.Salt;
            // The 32-byte data extracted from the generated pseudorandom number is used as a password
            byte[] bufferKey = deriveBytes.GetBytes(_encryptor.BufferKeySize);

            rij.Key = bufferKey;
            rij.GenerateIV();

            using (ICryptoTransform encrypt = rij.CreateEncryptor(rij.Key, rij.IV))
            {
                byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);
                // first 32 bytes of salt and second 32 bytes of IV for the first 64 bytes
                List<byte> compile = new List<byte>(salt);
                compile.AddRange(rij.IV);
                compile.AddRange(dest);
                return compile.ToArray();
            }
        }
        
        /// <summary>
        /// Decrypt String
        /// </summary>
        /// <param name="encrypted"></param>
        /// <returns></returns>
        public string DecodeString(string encrypted)
        {
            byte[] decripted = DecodeBinary(Convert.FromBase64String(encrypted));
            return Encoding.UTF8.GetString(decripted);
        }
        
        /// <summary>
        /// Decrypt Binary Data
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public byte[] DecodeBinary(byte[] src)
        {
            RijndaelManaged rij = SetupRijndaelManaged;

            List<byte> compile = new List<byte>(src);

            // First 32 bytes are salt.
            List<byte> salt = compile.GetRange(0, _encryptor.BufferKeySize);
            // Second 32 bytes are IV.
            List<byte> iv = compile.GetRange(_encryptor.BufferKeySize, _encryptor.BufferKeySize);
            rij.IV = iv.ToArray();

            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(_encryptor.Password, salt.ToArray());
            byte[] bufferKey = deriveBytes.GetBytes(_encryptor.BufferKeySize);    // Convert 32 bytes of salt to password
            rij.Key = bufferKey;

            byte[] plain = compile.GetRange(_encryptor.BufferKeySize * 2, compile.Count - (_encryptor.BufferKeySize * 2)).ToArray();

            using (ICryptoTransform decrypt = rij.CreateDecryptor(rij.Key, rij.IV))
            {
                byte[] dest = decrypt.TransformFinalBlock(plain, 0, plain.Length);
                return dest;
            }
        }
        
        /// <summary>
        /// Setup Manager
        /// </summary>
        private RijndaelManaged SetupRijndaelManaged
        {
            get
            {
                RijndaelManaged rij = new RijndaelManaged();
                rij.BlockSize = _encryptor.BlockSize;
                rij.KeySize = _encryptor.KeySize;
                rij.Mode = CipherMode.CBC;
                rij.Padding = PaddingMode.PKCS7;
                return rij;
            }
        }
    }
}