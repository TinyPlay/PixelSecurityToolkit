using System;
using PixelSecurity.Constants;
using PixelSecurity.Modules.SecuredMemory;
using UnityEngine;

namespace PixelSecurity.Core.SecuredTypes
{
    [System.Serializable]
    public struct SecuredShort : IEquatable<SecuredShort>, IFormattable
    {
        private static short _cryptoKey = 214;

        [SerializeField] private short currentCryptoKey;
	    [SerializeField] private short hiddenValue;
	    [SerializeField] private short fakeValue;
	    [SerializeField] private bool inited;

		/// <summary>
		/// Secured Short Constructor
		/// </summary>
		/// <param name="value"></param>
		private SecuredShort(short value)
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
		public static void SetCryptoKey(short newKey)
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
		/// <returns>Encrypted or decrypted <c>short</c> (depending on what <c>short</c> was passed to the function)</returns>
		public static short EncryptDecrypt(short value)
		{
			return EncryptDecrypt(value, 0);
		}

		/// <summary>
		/// Simple symmetric encryption, uses passed crypto key.
		/// </summary>
		/// <returns>Encrypted or decrypted <c>short</c> (depending on what <c>short</c> was passed to the function)</returns>
		public static short EncryptDecrypt(short value, short key)
		{
			if (key == 0)
			{
				return (short)(value ^ _cryptoKey);
			}
			return (short)(value ^ key);
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		public short GetEncrypted()
		{
			ApplyNewCryptoKey();

			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		public void SetEncrypted(short encrypted)
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
		private short InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = _cryptoKey;
				hiddenValue = EncryptDecrypt(0);
				fakeValue = 0;
				inited = true;
			}

			short key = _cryptoKey;

			if (currentCryptoKey != _cryptoKey)
			{
				key = currentCryptoKey;
			}

			short decrypted = EncryptDecrypt(hiddenValue, key);

			if (PixelGuard.Instance.HasModule<SecuredMemory>() && fakeValue != 0 && decrypted != fakeValue)
			{
				PixelGuard.Instance.CreateSecurityWarning(TextCodes.MEMORY_HACKING_DETECTED, PixelGuard.Instance.GetModule<SecuredMemory>());
			}

			return decrypted;
		}
		
		public static implicit operator SecuredShort(short value)
		{
			SecuredShort obscured = new SecuredShort(EncryptDecrypt(value));
			if (PixelGuard.Instance.HasModule<SecuredMemory>())
			{
				obscured.fakeValue = value;
			}
			return obscured;
		}
		public static implicit operator short(SecuredShort value)
		{
			return value.InternalDecrypt();
		}

		/// <summary>
		/// Increment Operator
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static SecuredShort operator ++(SecuredShort input)
		{
			short decrypted = (short)(input.InternalDecrypt() + 1);
			input.hiddenValue = EncryptDecrypt(decrypted);

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
		public static SecuredShort operator --(SecuredShort input)
		{
			short decrypted = (short)(input.InternalDecrypt() - 1);
			input.hiddenValue = EncryptDecrypt(decrypted);

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
			if (!(obj is SecuredShort))
				return false;
			
			SecuredShort ob = (SecuredShort)obj;
			return hiddenValue == ob.hiddenValue;
		}

		/// <summary>
		/// Returns a value indicating whether this instance and a specified SecuredShort object represent the same value.
		/// </summary>
		public bool Equals(SecuredShort obj)
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