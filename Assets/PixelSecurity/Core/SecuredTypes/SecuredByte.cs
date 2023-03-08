using System;
using PixelSecurity.Constants;
using PixelSecurity.Modules.SecuredMemory;
using UnityEngine;

namespace PixelSecurity.Core.SecuredTypes
{
    [Serializable]
    public struct SecuredByte : IEquatable<SecuredByte>, IFormattable
    {
        private static byte _cryptoKey = 244;

        [SerializeField] private byte currentCryptoKey;
	    [SerializeField] private byte hiddenValue;
	    [SerializeField] private byte fakeValue;
	    [SerializeField] private bool inited;
        
        /// <summary>
        /// Secured Byte Constructor
        /// </summary>
        /// <param name="value"></param>
        private SecuredByte(byte value)
        {
            currentCryptoKey = _cryptoKey;
            hiddenValue = value;
            fakeValue = 0;
            inited = true;
        }
        
        /// <summary>
		/// Set New Crypto Key
		/// </summary>
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
				hiddenValue = EncryptDecrypt(InternalDecrypt(), _cryptoKey);
				currentCryptoKey = _cryptoKey;
			}
		}

		/// <summary>
		/// Simple symmetric encryption, uses default crypto key.
		/// </summary>
		public static byte EncryptDecrypt(byte value)
		{
			return EncryptDecrypt(value, 0);
		}

		/// <summary>
		/// Simple symmetric encryption, uses passed crypto key.
		/// </summary>
		public static byte EncryptDecrypt(byte value, byte key)
		{
			if (key == 0)
			{
				return (byte)(value ^ _cryptoKey);
			}
			return (byte)(value ^ key);
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		public byte GetEncrypted()
		{
			ApplyNewCryptoKey();
			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		public void SetEncrypted(byte encrypted)
		{
			inited = true;
			hiddenValue = encrypted;
			if (PixelGuard.Instance.HasModule<SecuredMemory>())
			{
				fakeValue = InternalDecrypt();
			}
		}

		/// <summary>
		/// Internal Decrypt
		/// </summary>
		/// <returns></returns>
		private byte InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = _cryptoKey;
				hiddenValue = EncryptDecrypt(0);
				fakeValue = 0;
				inited = true;
			}

			byte key = _cryptoKey;

			if (currentCryptoKey != _cryptoKey)
			{
				key = currentCryptoKey;
			}

			byte decrypted = EncryptDecrypt(hiddenValue, key);

			if (PixelGuard.Instance.HasModule<SecuredMemory>() && fakeValue != 0 && decrypted != fakeValue)
			{
				PixelGuard.Instance.CreateSecurityWarning(TextCodes.MEMORY_HACKING_DETECTED, PixelGuard.Instance.GetModule<SecuredMemory>());
			}

			return decrypted;
		}

		public static implicit operator SecuredByte(byte value)
		{
			SecuredByte obscured = new SecuredByte(EncryptDecrypt(value));
			if (PixelGuard.Instance.HasModule<SecuredMemory>())
			{
				obscured.fakeValue = value;
			}
			return obscured;
		}

		public static implicit operator byte(SecuredByte value)
		{
			return value.InternalDecrypt();
		}

		/// <summary>
		/// Increment Operator
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static SecuredByte operator ++(SecuredByte input)
		{
			byte decrypted = (byte)(input.InternalDecrypt() + 1);
			input.hiddenValue = EncryptDecrypt(decrypted, input.currentCryptoKey);

			if (PixelGuard.Instance.HasModule<SecuredMemory>())
			{
				input.fakeValue = decrypted;
			}
			return input;
		}

		/// <summary>
		/// Decrement Operator
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static SecuredByte operator --(SecuredByte input)
		{
			byte decrypted = (byte)(input.InternalDecrypt() - 1);
			input.hiddenValue = EncryptDecrypt(decrypted, input.currentCryptoKey);

			if (PixelGuard.Instance.HasModule<SecuredMemory>())
			{
				input.fakeValue = decrypted;
			}
			return input;
		}

		/// <summary>
		/// Returns a value indicating whether this instance is equal to a specified object.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (!(obj is SecuredByte))
				return false;

			SecuredByte ob = (SecuredByte)obj;
			return hiddenValue == ob.hiddenValue;
		}

		/// <summary>
		/// Returns a value indicating whether this instance and a specified SecuredByte object represent the same value.
		/// </summary>
		public bool Equals(SecuredByte obj)
		{
			return hiddenValue == obj.hiddenValue;
		}

		/// <summary>
		/// Converts the numeric value of this instance to
		/// its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		/// <summary>
		/// Converts the numeric value of this instance to its
		/// equivalent string representation using the specified format.
		/// </summary>
		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		/// <summary>
		/// Converts the numeric value of this instance to its
		/// equivalent string representation using the specified
		/// culture-specific format information.
		/// </summary>
		public string ToString(IFormatProvider provider)
		{
			return InternalDecrypt().ToString(provider);
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent
		/// string representation using the specified
		/// format and culture-specific format information.
		/// </summary>
		public string ToString(string format, IFormatProvider provider)
		{
			return InternalDecrypt().ToString(format, provider);
		}
    }
}