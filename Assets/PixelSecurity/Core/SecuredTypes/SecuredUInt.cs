using System;
using PixelSecurity.Constants;
using PixelSecurity.Modules.SecuredMemory;
using UnityEngine;

namespace PixelSecurity.Core.SecuredTypes
{
    [System.Serializable]
    public struct SecuredUInt : IEquatable<SecuredUInt>, IFormattable
    {
        private static uint _cryptoKey = 240513;

	    [SerializeField] private uint currentCryptoKey;
	    [SerializeField] private uint hiddenValue;
	    [SerializeField] private uint fakeValue;
	    [SerializeField] private bool inited;

		/// <summary>
		/// Secured UInt Constructor
		/// </summary>
		/// <param name="value"></param>
		private SecuredUInt(uint value)
		{
			currentCryptoKey = _cryptoKey;
			hiddenValue = value;
			fakeValue = 0;
			inited = true;
		}

		/// <summary>
		/// Allows to change default crypto key of this type instances. All new instances will use specified key.<br/>
		/// All current instances will use previous key unless you call ApplyNewCryptoKey() on them explicitly.
		/// </summary>
		public static void SetCryptoKey(uint newKey)
		{
			_cryptoKey = newKey;
		}

		/// <summary>
		/// Use it after SetNewCryptoKey() to re-encrypt current instance using new crypto key.
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
		/// Simple symmetric encryption, uses default crypto key.
		/// </summary>
		/// <returns>Encrypted <c>uint</c>.</returns>
		public static uint Encrypt(uint value)
		{
			return Encrypt(value, 0);
		}

		/// <summary>
		/// Simple symmetric encryption, uses default crypto key.
		/// </summary>
		/// <returns>Decrypted <c>uint</c>.</returns>
		public static uint Decrypt(uint value)
		{
			return Decrypt(value, 0);
		}

		/// <summary>
		/// Simple symmetric encryption, uses passed crypto key.
		/// </summary>
		/// <returns>Encrypted <c>uint</c>.</returns>
		public static uint Encrypt(uint value, uint key)
		{
			if (key == 0)
			{
				return value ^ _cryptoKey;
			}
			return value ^ key;
		}

		/// <summary>
		/// Simple symmetric encryption, uses passed crypto key.
		/// </summary>
		/// <returns>Decrypted <c>uint</c>.</returns>
		public static uint Decrypt(uint value, uint key)
		{
			if (key == 0)
			{
				return value ^ _cryptoKey;
			}
			return value ^ key;
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		public uint GetEncrypted()
		{
			ApplyNewCryptoKey();

			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		public void SetEncrypted(uint encrypted)
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
		private uint InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = _cryptoKey;
				hiddenValue = Encrypt(0);
				fakeValue = 0;
				inited = true;
			}

			uint key = _cryptoKey;

			if (currentCryptoKey != _cryptoKey)
			{
				key = currentCryptoKey;
			}

			uint decrypted = Decrypt(hiddenValue, key);

			if (PixelGuard.Instance.HasModule<SecuredMemory>() && fakeValue != 0 && decrypted != fakeValue)
			{
				PixelGuard.Instance.CreateSecurityWarning(TextCodes.MEMORY_HACKING_DETECTED, PixelGuard.Instance.GetModule<SecuredMemory>());
			}

			return decrypted;
		}
		
		public static implicit operator SecuredUInt(uint value)
		{
			SecuredUInt obscured = new SecuredUInt(Encrypt(value));
			if (PixelGuard.Instance.HasModule<SecuredMemory>())
			{
				obscured.fakeValue = value;
			}
			return obscured;
		}
		public static implicit operator uint(SecuredUInt value)
		{
			return value.InternalDecrypt();
		}

		/// <summary>
		/// Increment Operator
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static SecuredUInt operator ++(SecuredUInt input)
		{
			uint decrypted = input.InternalDecrypt() + 1;
			input.hiddenValue = Encrypt(decrypted, input.currentCryptoKey);

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
		public static SecuredUInt operator --(SecuredUInt input)
		{
			uint decrypted = input.InternalDecrypt() - 1;
			input.hiddenValue = Encrypt(decrypted, input.currentCryptoKey);

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
			if (!(obj is SecuredUInt))
				return false;
			
			SecuredUInt oi = (SecuredUInt)obj;
			return ((int)hiddenValue == (int)oi.hiddenValue);
		}

		/// <summary>
		/// Returns a value indicating whether this instance is equal to a specified SecuredUInt.
		/// </summary>
		public bool Equals(SecuredUInt obj)
		{
			return (int)hiddenValue == (int)obj.hiddenValue;
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation using the specified format.
		/// </summary>
		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation using the specified culture-specific format information.
		/// </summary>
		public string ToString(IFormatProvider provider)
		{
			return InternalDecrypt().ToString(provider);
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation using the specified format and culture-specific format information.
		/// </summary>
		public string ToString(string format, IFormatProvider provider)
		{
			return InternalDecrypt().ToString(format, provider);
		}
    }
}