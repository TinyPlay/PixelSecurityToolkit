using System;
using PixelSecurity.Constants;
using PixelSecurity.Modules.SecuredMemory;
using UnityEngine;

namespace PixelSecurity.Core.SecuredTypes
{
    [System.Serializable]
    public struct SecuredSByte : IEquatable<SecuredSByte>, IFormattable
    {
        private static sbyte _cryptoKey = 112;

        [SerializeField] private sbyte currentCryptoKey;
        [SerializeField] private sbyte hiddenValue;
        [SerializeField] private sbyte fakeValue;
        [SerializeField] private bool inited;

		/// <summary>
		/// Secured SByte Constructor
		/// </summary>
		/// <param name="value"></param>
		private SecuredSByte(sbyte value)
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
		public static void SetCryptoKey(sbyte newKey)
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
				hiddenValue = EncryptDecrypt(InternalDecrypt(), _cryptoKey);
				currentCryptoKey = _cryptoKey;
			}
		}

		/// <summary>
		/// Simple symmetric encryption, uses default crypto key.
		/// </summary>
		/// <returns>Encrypted or decrypted <c>sbyte</c> (depending on what <c>sbyte</c> was passed to the function)</returns>
		public static sbyte EncryptDecrypt(sbyte value)
		{
			return EncryptDecrypt(value, 0);
		}

		/// <summary>
		/// Simple symmetric encryption, uses passed crypto key.
		/// </summary>
		/// <returns>Encrypted or decrypted <c>sbyte</c> (depending on what <c>sbyte</c> was passed to the function)</returns>
		public static sbyte EncryptDecrypt(sbyte value, sbyte key)
		{
			if (key == 0)
			{
				return (sbyte)(value ^ _cryptoKey);
			}
			return (sbyte)(value ^ key);
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		public sbyte GetEncrypted()
		{
			ApplyNewCryptoKey();

			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		public void SetEncrypted(sbyte encrypted)
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
		private sbyte InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = _cryptoKey;
				hiddenValue = EncryptDecrypt(0);
				fakeValue = 0;
				inited = true;
			}

			sbyte key = _cryptoKey;

			if (currentCryptoKey != _cryptoKey)
			{
				key = currentCryptoKey;
			}

			sbyte decrypted = EncryptDecrypt(hiddenValue, key);

			if (PixelGuard.Instance.HasModule<SecuredMemory>() && fakeValue != 0 && decrypted != fakeValue)
			{
				PixelGuard.Instance.CreateSecurityWarning(TextCodes.MEMORY_HACKING_DETECTED, PixelGuard.Instance.GetModule<SecuredMemory>());
			}

			return decrypted;
		}
		
		public static implicit operator SecuredSByte(sbyte value)
		{
			SecuredSByte obscured = new SecuredSByte(EncryptDecrypt(value));
			if (PixelGuard.Instance.HasModule<SecuredMemory>())
			{
				obscured.fakeValue = value;
			}
			return obscured;
		}
		public static implicit operator sbyte(SecuredSByte value)
		{
			return value.InternalDecrypt();
		}

		/// <summary>
		/// Increment Operator
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static SecuredSByte operator ++(SecuredSByte input)
		{
			sbyte decrypted = (sbyte)(input.InternalDecrypt() + 1);
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
		public static SecuredSByte operator --(SecuredSByte input)
		{
			sbyte decrypted = (sbyte)(input.InternalDecrypt() - 1);
			input.hiddenValue = EncryptDecrypt(decrypted, input.currentCryptoKey);

			if (PixelGuard.Instance.HasModule<SecuredMemory>())
			{
				input.fakeValue = decrypted;
			}
			return input;
		}
		
		/// <summary>
		/// Check Equals
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (!(obj is SecuredSByte))
				return false;

			SecuredSByte ob = (SecuredSByte)obj;
			return hiddenValue == ob.hiddenValue;
		}

		/// <summary>
		/// Check Equals
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public bool Equals(SecuredSByte obj)
		{
			return hiddenValue == obj.hiddenValue;
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