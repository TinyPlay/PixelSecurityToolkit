using System;
using PixelSecurity.Constants;
using PixelSecurity.Modules.SecuredMemory;
using UnityEngine;

namespace PixelSecurity.Core.SecuredTypes
{
    [System.Serializable]
    public struct SecuredVector3
    {
        private static int _cryptoKey = 120207;
		private static readonly Vector3 initialFakeValue = Vector3.zero;

		// Serialized Fields
		[SerializeField] private int currentCryptoKey;
		[SerializeField] private RawEncryptedVector3 hiddenValue;
		[SerializeField] private Vector3 fakeValue;
		[SerializeField] private bool inited;

		/// <summary>
		/// Secured Vector3 Constructor
		/// </summary>
		/// <param name="encrypted"></param>
		private SecuredVector3(RawEncryptedVector3 encrypted)
		{
			currentCryptoKey = _cryptoKey;
			hiddenValue = encrypted;
			fakeValue = initialFakeValue;
			inited = true;
		}

		/// <summary>
		/// Vector3.x
		/// </summary>
		public float x
		{
			get
			{
				SecuredMemory detector =
					(SecuredMemory) PixelGuard.Instance.GetModule<SecuredMemory>();
				float decrypted = InternalDecryptField(hiddenValue.x);
				if (detector!=null && !fakeValue.Equals(initialFakeValue) && Math.Abs(decrypted - fakeValue.x) > detector.Options.Vector3Epsilon)
				{
					PixelGuard.Instance.CreateSecurityWarning(TextCodes.MEMORY_HACKING_DETECTED, PixelGuard.Instance.GetModule<SecuredMemory>());
				}
				return decrypted;
			}

			set
			{
				SecuredMemory detector =
					(SecuredMemory) PixelGuard.Instance.GetModule<SecuredMemory>();
				hiddenValue.x = InternalEncryptField(value);
				if (detector!=null)
				{
					fakeValue.x = value;
				}
			}
		}

		/// <summary>
		/// Vector3.y
		/// </summary>
		public float y
		{
			get
			{
				SecuredMemory detector =
					(SecuredMemory) PixelGuard.Instance.GetModule<SecuredMemory>();
				float decrypted = InternalDecryptField(hiddenValue.y);
				if (detector!=null && !fakeValue.Equals(initialFakeValue) && Math.Abs(decrypted - fakeValue.y) > detector.Options.Vector3Epsilon)
				{
					PixelGuard.Instance.CreateSecurityWarning(TextCodes.MEMORY_HACKING_DETECTED, PixelGuard.Instance.GetModule<SecuredMemory>());
				}
				return decrypted;
			}

			set
			{
				SecuredMemory detector =
					(SecuredMemory) PixelGuard.Instance.GetModule<SecuredMemory>();
				hiddenValue.y = InternalEncryptField(value);
				if (detector!=null)
				{
					fakeValue.y = value;
				}
			}
		}

		/// <summary>
		/// Vector3.z
		/// </summary>
		public float z
		{
			get
			{
				SecuredMemory detector =
					(SecuredMemory) PixelGuard.Instance.GetModule<SecuredMemory>();
				float decrypted = InternalDecryptField(hiddenValue.z);
				if (detector!=null && !fakeValue.Equals(initialFakeValue) && Math.Abs(decrypted - fakeValue.z) > detector.Options.Vector3Epsilon)
				{
					PixelGuard.Instance.CreateSecurityWarning(TextCodes.MEMORY_HACKING_DETECTED, PixelGuard.Instance.GetModule<SecuredMemory>());
				}
				return decrypted;
			}

			set
			{
				SecuredMemory detector =
					(SecuredMemory) PixelGuard.Instance.GetModule<SecuredMemory>();
				hiddenValue.z = InternalEncryptField(value);
				if (detector!=null)
				{
					fakeValue.z = value;
				}
			}
		}

		/// <summary>
		/// This
		/// </summary>
		/// <param name="index"></param>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public float this[int index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return x;
					case 1:
						return y;
					case 2:
						return z;
					default:
						throw new IndexOutOfRangeException("Invalid SecuredVector3 index!");
				}
			}
			set
			{
				switch (index)
				{
					case 0:
						x = value;
						break;
					case 1:
						y = value;
						break;
					case 2:
						z = value;
						break;
					default:
						throw new IndexOutOfRangeException("Invalid SecuredVector3 index!");
				}
			}
		}

		/// <summary>
		/// Allows to change default crypto key of this type instances. All new instances will use specified key.<br/>
		/// All current instances will use previous key unless you call ApplyNewCryptoKey() on them explicitly.
		/// </summary>
		public static void SetCryptoKey(int newKey)
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
		/// Use this simple encryption method to encrypt any Vector3 value, uses default crypto key.
		/// </summary>
		public static RawEncryptedVector3 Encrypt(Vector3 value)
		{
			return Encrypt(value, 0);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any Vector3 value, uses passed crypto key.
		/// </summary>
		public static RawEncryptedVector3 Encrypt(Vector3 value, int key)
		{
			if (key == 0)
			{
				key = _cryptoKey;
			}

			RawEncryptedVector3 result;
			result.x = SecuredFloat.Encrypt(value.x, key);
			result.y = SecuredFloat.Encrypt(value.y, key);
			result.z = SecuredFloat.Encrypt(value.z, key);

			return result;
		}

		/// <summary>
		/// Use it to decrypt RawEncryptedVector3 you got from Encrypt(), uses default crypto key.
		/// </summary>
		public static Vector3 Decrypt(RawEncryptedVector3 value)
		{
			return Decrypt(value, 0);
		}

		/// <summary>
		/// Use it to decrypt RawEncryptedVector3 you got from Encrypt(), uses passed crypto key.
		/// </summary>
		public static Vector3 Decrypt(RawEncryptedVector3 value, int key)
		{
			if (key == 0)
			{
				key = _cryptoKey;
			}

			Vector3 result;
			result.x = SecuredFloat.Decrypt(value.x, key);
			result.y = SecuredFloat.Decrypt(value.y, key);
			result.z = SecuredFloat.Decrypt(value.z, key);

			return result;
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		public RawEncryptedVector3 GetEncrypted()
		{
			ApplyNewCryptoKey();
			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		public void SetEncrypted(RawEncryptedVector3 encrypted)
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
		private Vector3 InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = _cryptoKey;
				hiddenValue = Encrypt(initialFakeValue, _cryptoKey);
				fakeValue = initialFakeValue;
				inited = true;
			}

			int key = _cryptoKey;

			if (currentCryptoKey != _cryptoKey)
			{
				key = currentCryptoKey;
			}

			Vector3 value;

			value.x = SecuredFloat.Decrypt(hiddenValue.x, key);
			value.y = SecuredFloat.Decrypt(hiddenValue.y, key);
			value.z = SecuredFloat.Decrypt(hiddenValue.z, key);
			
			if (PixelGuard.Instance.HasModule<SecuredMemory>() && !fakeValue.Equals(Vector3.zero) && !CompareVectorsWithTolerance(value, fakeValue))
			{
				PixelGuard.Instance.CreateSecurityWarning(TextCodes.MEMORY_HACKING_DETECTED, PixelGuard.Instance.GetModule<SecuredMemory>());
			}

			return value;
		}

		/// <summary>
		/// Compare Vectors with Tolerance
		/// </summary>
		/// <param name="vector1"></param>
		/// <param name="vector2"></param>
		/// <returns></returns>
		private bool CompareVectorsWithTolerance(Vector3 vector1, Vector3 vector2)
		{
			SecuredMemory detector =
				(SecuredMemory) PixelGuard.Instance.GetModule<SecuredMemory>();
			float epsilon = detector.Options.Vector3Epsilon;
			return Math.Abs(vector1.x - vector2.x) < epsilon &&
				   Math.Abs(vector1.y - vector2.y) < epsilon &&
				   Math.Abs(vector1.z - vector2.z) < epsilon;
		}

		/// <summary>
		/// Internal Decrypt Field
		/// </summary>
		/// <param name="encrypted"></param>
		/// <returns></returns>
		private float InternalDecryptField(int encrypted)
		{
			int key = _cryptoKey;

			if (currentCryptoKey != _cryptoKey)
			{
				key = currentCryptoKey;
			}

			float result = SecuredFloat.Decrypt(encrypted, key);
			return result;
		}
		
		/// <summary>
		/// Internal Encrypt Field
		/// </summary>
		/// <param name="encrypted"></param>
		/// <returns></returns>
		private int InternalEncryptField(float encrypted)
		{
			int result = SecuredFloat.Encrypt(encrypted, _cryptoKey);
			return result;
		}

		#region Operators
		public static implicit operator SecuredVector3(Vector3 value)
		{
			SecuredVector3 obscured = new SecuredVector3(Encrypt(value, _cryptoKey));
			if (PixelGuard.Instance.HasModule<SecuredMemory>())
			{
				obscured.fakeValue = value;
			}
			return obscured;
		}
		public static implicit operator Vector3(SecuredVector3 value)
		{
			return value.InternalDecrypt();
		}
		public static SecuredVector3 operator +(SecuredVector3 a, SecuredVector3 b)
		{
			return a.InternalDecrypt() + b.InternalDecrypt();
		}
		public static SecuredVector3 operator +(Vector3 a, SecuredVector3 b)
		{
			return a + b.InternalDecrypt();
		}
		public static SecuredVector3 operator +(SecuredVector3 a, Vector3 b)
		{
			return a.InternalDecrypt() + b;
		}
		public static SecuredVector3 operator -(SecuredVector3 a, SecuredVector3 b)
		{
			return a.InternalDecrypt() - b.InternalDecrypt();
		}
		public static SecuredVector3 operator -(Vector3 a, SecuredVector3 b)
		{
			return a - b.InternalDecrypt();
		}
		public static SecuredVector3 operator -(SecuredVector3 a, Vector3 b)
		{
			return a.InternalDecrypt() - b;
		}
		public static SecuredVector3 operator -(SecuredVector3 a)
		{
			return -a.InternalDecrypt();
		}
		public static SecuredVector3 operator *(SecuredVector3 a, float d)
		{
			return a.InternalDecrypt() * d;
		}
		public static SecuredVector3 operator *(float d, SecuredVector3 a)
		{
			return d * a.InternalDecrypt();
		}
		public static SecuredVector3 operator /(SecuredVector3 a, float d)
		{
			return a.InternalDecrypt() / d;
		}

		public static bool operator ==(SecuredVector3 lhs, SecuredVector3 rhs)
		{
			return lhs.InternalDecrypt() == rhs.InternalDecrypt();
		}
		public static bool operator ==(Vector3 lhs, SecuredVector3 rhs)
		{
			return lhs == rhs.InternalDecrypt();
		}
		public static bool operator ==(SecuredVector3 lhs, Vector3 rhs)
		{
			return lhs.InternalDecrypt() == rhs;
		}

		public static bool operator !=(SecuredVector3 lhs, SecuredVector3 rhs)
		{
			return lhs.InternalDecrypt() != rhs.InternalDecrypt();
		}
		public static bool operator !=(Vector3 lhs, SecuredVector3 rhs)
		{
			return lhs != rhs.InternalDecrypt();
		}
		public static bool operator !=(SecuredVector3 lhs, Vector3 rhs)
		{
			return lhs.InternalDecrypt() != rhs;
		}
		#endregion
		
		/// <summary>
		/// Check Equals
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public override bool Equals(object other)
		{
			return InternalDecrypt().Equals(other);
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		/// <summary>
		/// Returns a nicely formatted string for this vector.
		/// </summary>
		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		/// <summary>
		/// Returns a nicely formatted string for this vector.
		/// </summary>
		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}

		/// <summary>
		/// Used to store encrypted Vector3.
		/// </summary>
		[Serializable]
		public struct RawEncryptedVector3
		{
			/// <summary>
			/// Encrypted value
			/// </summary>
			public int x;

			/// <summary>
			/// Encrypted value
			/// </summary>
			public int y;

			/// <summary>
			/// Encrypted value
			/// </summary>
			public int z;
		}
    }
}