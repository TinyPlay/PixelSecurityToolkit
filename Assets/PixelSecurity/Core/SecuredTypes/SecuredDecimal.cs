using System;
using System.Runtime.InteropServices;
using PixelSecurity.Constants;
using PixelSecurity.Modules.SecuredMemory;
using UnityEngine;

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
namespace PixelSecurity.Core.SecuredTypes
{
    [System.Serializable]
    public struct SecuredDecimal : IEquatable<SecuredDecimal>, IFormattable
    {
        private static long _cryptoKey = 209208L;

        [SerializeField] private long currentCryptoKey;
	    [SerializeField] private byte[] hiddenValue;
        [SerializeField] private decimal fakeValue;
        [SerializeField] private bool inited;
        
        /// <summary>
        /// Secured Decimal Constructor
        /// </summary>
        /// <param name="value"></param>
        private SecuredDecimal(byte[] value)
        {
            currentCryptoKey = _cryptoKey;
            hiddenValue = value;
            fakeValue = 0m;
            inited = true;
        }
        
        /// <summary>
		/// Allows to change default crypto key of this type instances. All new instances will use specified key.<br/>
		/// All current instances will use previous key unless you call ApplyNewCryptoKey() on them explicitly.
		/// </summary>
		public static void SetCryptoKey(long newKey)
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
				hiddenValue = InternalEncrypt(InternalDecrypt(), _cryptoKey);
				currentCryptoKey = _cryptoKey;
			}
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any decimal value, uses default crypto key.
		/// </summary>
		public static decimal Encrypt(decimal value)
		{
			return Encrypt(value, _cryptoKey);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any decimal value, uses passed crypto key.
		/// </summary>
		public static decimal Encrypt(decimal value, long key)
		{
			var u = new DecimalLongBytesUnion();
			u.d = value;
			u.l1 = u.l1 ^ key;
			u.l2 = u.l2 ^ key;

			return u.d;
		}

		/// <summary>
		/// Internal Encrypt
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private static byte[] InternalEncrypt(decimal value)
		{
			return InternalEncrypt(value, 0L);
		}

		/// <summary>
		/// Internal Encrypt by key
		/// </summary>
		/// <param name="value"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		private static byte[] InternalEncrypt(decimal value, long key)
		{
			long currKey = key;
			if (currKey == 0L)
			{
				currKey = _cryptoKey;
			}

			DecimalLongBytesUnion union = new DecimalLongBytesUnion();
			union.d = value;
			union.l1 = union.l1 ^ currKey;
			union.l2 = union.l2 ^ currKey;

			return new[]
			{
				union.b1, union.b2, union.b3, union.b4, union.b5, union.b6, union.b7, union.b8,
				union.b9, union.b10, union.b11, union.b12, union.b13, union.b14, union.b15, union.b16
			};
		}

		/// <summary>
		/// Use it to decrypt long you got from Encrypt(decimal) back to decimal, uses default crypto key.
		/// </summary>
		public static decimal Decrypt(decimal value)
		{
			return Decrypt(value, _cryptoKey);
		}

		/// <summary>
		/// Use it to decrypt long you got from Encrypt(decimal) back to decimal, uses passed crypto key.
		/// </summary>
		public static decimal Decrypt(decimal value, long key)
		{
			DecimalLongBytesUnion u = new DecimalLongBytesUnion();
			u.d = value;
			u.l1 = u.l1 ^ key;
			u.l2 = u.l2 ^ key;
			return u.d;
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		public decimal GetEncrypted()
		{
			ApplyNewCryptoKey();

			DecimalLongBytesUnion union = new DecimalLongBytesUnion();
			union.b1 = hiddenValue[0];
			union.b2 = hiddenValue[1];
			union.b3 = hiddenValue[2];
			union.b4 = hiddenValue[3];
			union.b5 = hiddenValue[4];
			union.b6 = hiddenValue[5];
			union.b7 = hiddenValue[6];
			union.b8 = hiddenValue[7];
			union.b9 = hiddenValue[8];
			union.b10 = hiddenValue[9];
			union.b11 = hiddenValue[10];
			union.b12 = hiddenValue[11];
			union.b13 = hiddenValue[12];
			union.b14 = hiddenValue[13];
			union.b15 = hiddenValue[14];
			union.b16 = hiddenValue[15];

			return union.d;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		public void SetEncrypted(decimal encrypted)
		{
			inited = true;
			DecimalLongBytesUnion union = new DecimalLongBytesUnion();
			union.d = encrypted;

			hiddenValue = new[]
			{
				union.b1, union.b2, union.b3, union.b4, union.b5, union.b6, union.b7, union.b8,
				union.b9, union.b10, union.b11, union.b12, union.b13, union.b14, union.b15, union.b16
			};

			if (PixelGuard.Instance.HasModule<SecuredMemory>())
			{
				fakeValue = InternalDecrypt();
			}
		}

		/// <summary>
		/// Internal Decrypt
		/// </summary>
		/// <returns></returns>
		private decimal InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = _cryptoKey;
				hiddenValue = InternalEncrypt(0m);
				fakeValue = 0m;
				inited = true;
			}

			long key = _cryptoKey;

			if (currentCryptoKey != _cryptoKey)
			{
				key = currentCryptoKey;
			}

			DecimalLongBytesUnion union = new DecimalLongBytesUnion();
			union.b1 = hiddenValue[0];
			union.b2 = hiddenValue[1];
			union.b3 = hiddenValue[2];
			union.b4 = hiddenValue[3];
			union.b5 = hiddenValue[4];
			union.b6 = hiddenValue[5];
			union.b7 = hiddenValue[6];
			union.b8 = hiddenValue[7];
			union.b9 = hiddenValue[8];
			union.b10 = hiddenValue[9];
			union.b11 = hiddenValue[10];
			union.b12 = hiddenValue[11];
			union.b13 = hiddenValue[12];
			union.b14 = hiddenValue[13];
			union.b15 = hiddenValue[14];
			union.b16 = hiddenValue[15];

			union.l1 = union.l1 ^ key;
			union.l2 = union.l2 ^ key;

			decimal decrypted = union.d;

			if (PixelGuard.Instance.HasModule<SecuredMemory>() && fakeValue != 0 && decrypted != fakeValue)
			{
				PixelGuard.Instance.CreateSecurityWarning(TextCodes.MEMORY_HACKING_DETECTED, PixelGuard.Instance.GetModule<SecuredMemory>());
			}

			return decrypted;
		}

		/// <summary>
		/// Long-Bytes Union Decimal
		/// </summary>
		[StructLayout(LayoutKind.Explicit)]
		private struct DecimalLongBytesUnion
		{
			[FieldOffset(0)]
			public decimal d;

			[FieldOffset(0)]
			public long l1;

			[FieldOffset(8)]
			public long l2;

			[FieldOffset(0)]
			public byte b1;

			[FieldOffset(1)]
			public byte b2;

			[FieldOffset(2)]
			public byte b3;

			[FieldOffset(3)]
			public byte b4;

			[FieldOffset(4)]
			public byte b5;

			[FieldOffset(5)]
			public byte b6;

			[FieldOffset(6)]
			public byte b7;

			[FieldOffset(7)]
			public byte b8;

			[FieldOffset(8)]
			public byte b9;

			[FieldOffset(9)]
			public byte b10;

			[FieldOffset(10)]
			public byte b11;

			[FieldOffset(11)]
			public byte b12;

			[FieldOffset(12)]
			public byte b13;

			[FieldOffset(13)]
			public byte b14;

			[FieldOffset(14)]
			public byte b15;

			[FieldOffset(15)]
			public byte b16;
		}
		
		public static implicit operator SecuredDecimal(decimal value)
		{
			SecuredDecimal obscured = new SecuredDecimal(InternalEncrypt(value));
			if (PixelGuard.Instance.HasModule<SecuredMemory>())
			{
				obscured.fakeValue = value;
			}
			return obscured;
		}
		public static implicit operator decimal(SecuredDecimal value)
		{
			return value.InternalDecrypt();
		}

		/// <summary>
		/// Increment Operator
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static SecuredDecimal operator ++(SecuredDecimal input)
		{
			decimal decrypted = input.InternalDecrypt() + 1m;
			input.hiddenValue = InternalEncrypt(decrypted, input.currentCryptoKey);

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
		public static SecuredDecimal operator --(SecuredDecimal input)
		{
			decimal decrypted = input.InternalDecrypt() - 1m;
			input.hiddenValue = InternalEncrypt(decrypted, input.currentCryptoKey);

			if (PixelGuard.Instance.HasModule<SecuredMemory>())
			{
				input.fakeValue = decrypted;
			}
			return input;
		}

		/// <summary>
		/// Converts the numeric value of this instance to its
		/// equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		/// <summary>
		/// Converts the numeric value of this instance to its
		/// equivalent string representation, using the specified format.
		/// </summary>
		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent
		/// string representation using the specified culture-specific format information.
		/// </summary>
		public string ToString(IFormatProvider provider)
		{
			return InternalDecrypt().ToString(provider);
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent
		/// string representation using the specified format and
		/// culture-specific format information.
		/// </summary>
		public string ToString(string format, IFormatProvider provider)
		{
			return InternalDecrypt().ToString(format, provider);
		}

		/// <summary>
		/// Returns a value indicating whether this instance is equal to a specified object.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (!(obj is SecuredDecimal))
				return false;
			SecuredDecimal d = (SecuredDecimal)obj;
			return d.InternalDecrypt().Equals(InternalDecrypt());
		}

		/// <summary>
		/// Returns a value indicating whether this instance
		/// and a specified <see cref="T:System.Decimal"/> object represent the same value.
		/// </summary>
		public bool Equals(SecuredDecimal obj)
		{
			return obj.InternalDecrypt().Equals(InternalDecrypt());
		}

		/// <summary>
		/// Get Hash Code
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}
    }
}