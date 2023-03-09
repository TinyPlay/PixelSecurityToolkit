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
using PixelSecurity.Constants;
using PixelSecurity.Modules.SecuredMemory;
using UnityEngine;

namespace PixelSecurity.Core.SecuredTypes
{
    /// <summary>
    /// Memory-Secured Boolean Type
    /// </summary>
    [System.Serializable]
    public struct SecuredBool : IEquatable<SecuredBool>
    {
        // Crypto Key
        private static byte _cryptoKey = 215;
        
        // Serialized Fields
        [SerializeField] private byte currentCryptoKey;
        [SerializeField] private int hiddenValue;
        [SerializeField] private bool fakeValue;
        [SerializeField] private bool fakeValueChanged;
        [SerializeField] private bool inited;
        
        /// <summary>
        /// Setup Secured Boolean
        /// </summary>
        /// <param name="value"></param>
        private SecuredBool(int value)
        {
            currentCryptoKey = _cryptoKey;
            hiddenValue = value;
            fakeValue = false;
            fakeValueChanged = false;
            inited = true;
        }
        
        /// <summary>
        /// Set Crypto Key
        /// </summary>
        /// <param name="cryptoKey"></param>
        public static void SetCryptoKey(byte cryptoKey)
        {
            _cryptoKey = cryptoKey;
        }
        
        /// <summary>
        /// Apply New Crypto Key
        /// </summary>
        public void ApplyNewCryptoKey()
        {
            if (currentCryptoKey != _cryptoKey)
            {
                hiddenValue = Encrypt(InternalDecrypt(), _cryptoKey);
                currentCryptoKey = _cryptoKey;
            }
        }
        
        /// <summary>
        /// Use it to encrypt any bool Value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Encrypt(bool value)
        {
            return Encrypt(value, 0);
        }
        
        /// <summary>
        /// Encrypt Bool by Key
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int Encrypt(bool value, byte key)
        {
            if (key == 0)
            {
                key = _cryptoKey;
            }

            int encryptedValue = value ? 213 : 181;

            encryptedValue ^= key;

            return encryptedValue;
        }

        /// <summary>
        /// Decrypt int into bool
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Decrypt(int value)
        {
            return Decrypt(value, 0);
        }
        
        /// <summary>
        /// Decrypt int into bool using key
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Decrypt(int value, byte key)
        {
            if (key == 0)
            {
                key = _cryptoKey;
            }

            value ^= key;

            return value != 181;
        }

        /// <summary>
        /// Get Encrypted Data
        /// </summary>
        /// <returns></returns>
        public int GetEncrypted()
        {
            ApplyNewCryptoKey();
            return hiddenValue;
        }

        /// <summary>
        /// Set Encrypted Data
        /// </summary>
        /// <param name="encrypted"></param>
        public void SetEncrypted(int encrypted)
        {
            inited = true;
            hiddenValue = encrypted;
            if (PixelGuard.Instance.HasModule<SecuredMemory>())
            {
                fakeValue = InternalDecrypt();
                fakeValueChanged = true;
            }
        }
        
        /// <summary>
        /// Internal Decrypt Method
        /// </summary>
        /// <returns></returns>
        private bool InternalDecrypt()
        {
            if (!inited)
            {
                currentCryptoKey = _cryptoKey;
                hiddenValue = Encrypt(false);
                fakeValue = false;
                fakeValueChanged = true;
                inited = true;
            }

            byte key = _cryptoKey;

            if (currentCryptoKey != _cryptoKey)
            {
                key = currentCryptoKey;
            }

            int value = hiddenValue;
            value ^= key;

            bool decrypted = value != 181;

            if (PixelGuard.Instance.HasModule<SecuredMemory>() && fakeValueChanged && decrypted != fakeValue)
            {
                PixelGuard.Instance.CreateSecurityWarning(TextCodes.MEMORY_HACKING_DETECTED, PixelGuard.Instance.GetModule<SecuredMemory>());
            }

            return decrypted;
        }
        
        public static implicit operator SecuredBool(bool value)
        {
            SecuredBool obscured = new SecuredBool(Encrypt(value));

            if (PixelGuard.Instance.HasModule<SecuredMemory>())
            {
                obscured.fakeValue = value;
                obscured.fakeValueChanged = true;
            }

            return obscured;
        }
        public static implicit operator bool(SecuredBool value)
        {
            return value.InternalDecrypt();
        }
        
        /// <summary>
        /// Equaks Method for any object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is SecuredBool))
                return false;

            SecuredBool oi = (SecuredBool)obj;
            return (hiddenValue == oi.hiddenValue);
        }
        
        /// <summary>
        /// Equals Method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(SecuredBool obj)
        {
            return hiddenValue == obj.hiddenValue;
        }

        /// <summary>
        /// Get Hash Code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return InternalDecrypt().GetHashCode();
        }
        
        /// <summary>
        /// Convert to String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return InternalDecrypt().ToString();
        }
    }
}