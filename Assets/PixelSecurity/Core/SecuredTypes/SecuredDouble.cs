using System;
using System.Runtime.InteropServices;
using PixelSecurity.Constants;
using PixelSecurity.Modules.SecuredMemory;
using UnityEngine;

namespace PixelSecurity.Core.SecuredTypes
{
    [System.Serializable]
    public struct SecuredDouble : IEquatable<SecuredDouble>, IFormattable 
    {
        private static long _cryptoKey = 210987L;

        [SerializeField] private long currentCryptoKey;
	    [SerializeField] private byte[] hiddenValue;
	    [SerializeField] private double fakeValue;
	    [SerializeField] private bool inited;

		/// <summary>
		/// Secured Double Constructor
		/// </summary>
		/// <param name="value"></param>
		private SecuredDouble(byte[] value)
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
		/// Use this simple encryption method to encrypt any double value, uses default crypto key.
		/// </summary>
		public static long Encrypt(double value)
		{
			return Encrypt(value, _cryptoKey);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any double value, uses passed crypto key.
		/// </summary>
		public static long Encrypt(double value, long key)
		{
			var u = new DoubleLongBytesUnion();
			u.d = value;
			u.l = u.l ^ key;

			return u.l;
		}

		/// <summary>
		/// Internal Encrypt
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private static byte[] InternalEncrypt(double value)
		{
			return InternalEncrypt(value, 0L);
		}

		/// <summary>
		/// Internal Encrypt using key
		/// </summary>
		/// <param name="value"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		private static byte[] InternalEncrypt(double value, long key)
		{
			long currKey = key;
			if (currKey == 0L)
			{
				currKey = _cryptoKey;
			}

			var u = new DoubleLongBytesUnion();
			u.d = value;
			u.l = u.l ^ currKey;

			return new[] { u.b1, u.b2, u.b3, u.b4, u.b5, u.b6, u.b7, u.b8};
		}

		/// <summary>
		/// Use it to decrypt long you got
		/// from Encrypt(double) back to double, uses default crypto key.
		/// </summary>
		public static double Decrypt(long value)
		{
			return Decrypt(value, _cryptoKey);
		}

		/// <summary>
		/// Use it to decrypt long you got
		/// from Encrypt(double) back to double, uses passed crypto key.
		/// </summary>
		public static double Decrypt(long value, long key)
		{
			var u = new DoubleLongBytesUnion();
			u.l = value ^ key;
			return u.d;
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		public long GetEncrypted()
		{
			ApplyNewCryptoKey();

			var union = new DoubleLongBytesUnion();
			union.b1 = hiddenValue[0];
			union.b2 = hiddenValue[1];
			union.b3 = hiddenValue[2];
			union.b4 = hiddenValue[3];
			union.b5 = hiddenValue[4];
			union.b6 = hiddenValue[5];
			union.b7 = hiddenValue[6];
			union.b8 = hiddenValue[7];

			return union.l;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		public void SetEncrypted(long encrypted)
		{
			inited = true;
			var union = new DoubleLongBytesUnion();
			union.l = encrypted;

			hiddenValue = new[] { union.b1, union.b2, union.b3, union.b4, union.b5, union.b6, union.b7, union.b8 };

			if (PixelGuard.Instance.HasModule<SecuredMemory>())
			{
				fakeValue = InternalDecrypt();
			}
		}

		/// <summary>
		/// Internal Decrypt
		/// </summary>
		/// <returns></returns>
		private double InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = _cryptoKey;
				hiddenValue = InternalEncrypt(0);
				fakeValue = 0;
				inited = true;
			}

			long key = _cryptoKey;

			if (currentCryptoKey != _cryptoKey)
			{
				key = currentCryptoKey;
			}

			var union = new DoubleLongBytesUnion();
			union.b1 = hiddenValue[0];
			union.b2 = hiddenValue[1];
			union.b3 = hiddenValue[2];
			union.b4 = hiddenValue[3];
			union.b5 = hiddenValue[4];
			union.b6 = hiddenValue[5];
			union.b7 = hiddenValue[6];
			union.b8 = hiddenValue[7];

			union.l = union.l ^ key;

			double decrypted = union.d;

			if (PixelGuard.Instance.HasModule<SecuredMemory>() && fakeValue != 0 && Math.Abs(decrypted - fakeValue) > 0.000001d)
			{
				PixelGuard.Instance.CreateSecurityWarning(TextCodes.MEMORY_HACKING_DETECTED, PixelGuard.Instance.GetModule<SecuredMemory>());
			}

			return decrypted;
		}

		/// <summary>
		/// Double Long Bytes Union
		/// </summary>
		[StructLayout(LayoutKind.Explicit)]
		private struct DoubleLongBytesUnion
		{
			[FieldOffset(0)]
			public double d;

			[FieldOffset(0)]
			public long l;

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
		}
		
		public static implicit operator SecuredDouble(double value)
		{
			SecuredDouble obscured = new SecuredDouble(InternalEncrypt(value));
			if (PixelGuard.Instance.HasModule<SecuredMemory>())
			{
				obscured.fakeValue = value;
			}
			return obscured;
		}
		public static implicit operator double(SecuredDouble value)
		{
			return value.InternalDecrypt();
		}

		/// <summary>
		/// Increment Operator
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static SecuredDouble operator ++(SecuredDouble input)
		{
			double decrypted = input.InternalDecrypt() + 1d;
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
		public static SecuredDouble operator --(SecuredDouble input)
		{
			double decrypted = input.InternalDecrypt() - 1d;
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
		/// <returns></returns>
		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent
		/// string representation, using the specified format.
		/// </summary>
		/// <returns></returns>
		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}

		/// <summary>
		/// Converts the numeric value of this instance to its
		/// equivalent string representation using the specified
		/// culture-specific format information.
		/// </summary>
		/// <returns></returns>
		public string ToString(IFormatProvider provider)
		{
			return InternalDecrypt().ToString(provider);
		}

		/// <summary>
		/// Converts the numeric value of this
		/// instance to its equivalent string r
		/// epresentation using the specified format and
		/// culture-specific format information.
		/// </summary>
		/// <returns></returns>
		public string ToString(string format, IFormatProvider provider)
		{
			return InternalDecrypt().ToString(format, provider);
		}
		
		/// <summary>
		/// Check Equals
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (!(obj is SecuredDouble))
				return false;
			SecuredDouble d = (SecuredDouble)obj;
			double dParam = d.InternalDecrypt();
			double dThis = InternalDecrypt();
			
			if (dParam == dThis)
				return true;
			return double.IsNaN(dParam) && double.IsNaN(dThis);
		}

		/// <summary>
		/// Check Equals
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public bool Equals(SecuredDouble obj)
		{
			double dParam = obj.InternalDecrypt();
			double dThis = InternalDecrypt();

			if (dParam == dThis)
				return true;
			return double.IsNaN(dParam) && double.IsNaN(dThis);
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}
    }
}