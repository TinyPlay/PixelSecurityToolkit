using System;
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
    public struct SecuredQuaternion
    {
        private static int _cryptoKey = 120205;
        private static readonly Quaternion initialFakeValue = Quaternion.identity;

        // Serialized Params
        [SerializeField] private int currentCryptoKey;
        [SerializeField] private RawEncryptedQuaternion hiddenValue;
        [SerializeField] private Quaternion fakeValue;
        [SerializeField] private bool inited;

        /// <summary>
        /// Secured Quaternion Constructor
        /// </summary>
        /// <param name="value"></param>
		private SecuredQuaternion(RawEncryptedQuaternion value)
		{
			currentCryptoKey = _cryptoKey;
			hiddenValue = value;
			fakeValue = initialFakeValue;
			inited = true;
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
		/// Use this simple encryption method to encrypt any Quaternion value, uses default crypto key.
		/// </summary>
		public static RawEncryptedQuaternion Encrypt(Quaternion value)
		{
			return Encrypt(value, 0);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any Quaternion value, uses passed crypto key.
		/// </summary>
		public static RawEncryptedQuaternion Encrypt(Quaternion value, int key)
		{
			if (key == 0)
			{
				key = _cryptoKey;
			}

			RawEncryptedQuaternion result;
			result.x = SecuredFloat.Encrypt(value.x, key);
			result.y = SecuredFloat.Encrypt(value.y, key);
			result.z = SecuredFloat.Encrypt(value.z, key);
			result.w = SecuredFloat.Encrypt(value.w, key);

			return result;
		}

		/// <summary>
		/// Use it to decrypt RawEncryptedQuaternion you got from Encrypt(), uses default crypto key.
		/// </summary>
		public static Quaternion Decrypt(RawEncryptedQuaternion value)
		{
			return Decrypt(value, 0);
		}

		/// <summary>
		/// Use it to decrypt RawEncryptedQuaternion you got from Encrypt(), uses passed crypto key.
		/// </summary>
		public static Quaternion Decrypt(RawEncryptedQuaternion value, int key)
		{
			if (key == 0)
			{
				key = _cryptoKey;
			}

			Quaternion result;
			result.x = SecuredFloat.Decrypt(value.x, key);
			result.y = SecuredFloat.Decrypt(value.y, key);
			result.z = SecuredFloat.Decrypt(value.z, key);
			result.w = SecuredFloat.Decrypt(value.w, key);

			return result;
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		public RawEncryptedQuaternion GetEncrypted()
		{
			ApplyNewCryptoKey();
			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		public void SetEncrypted(RawEncryptedQuaternion encrypted)
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
		private Quaternion InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = _cryptoKey;
				hiddenValue = Encrypt(initialFakeValue);
				fakeValue = initialFakeValue;
				inited = true;
			}

			int key = _cryptoKey;

			if (currentCryptoKey != _cryptoKey)
			{
				key = currentCryptoKey;
			}

			Quaternion value;

			value.x = SecuredFloat.Decrypt(hiddenValue.x, key);
			value.y = SecuredFloat.Decrypt(hiddenValue.y, key);
			value.z = SecuredFloat.Decrypt(hiddenValue.z, key);
			value.w = SecuredFloat.Decrypt(hiddenValue.w, key);

			if (PixelGuard.Instance.HasModule<SecuredMemory>() && !fakeValue.Equals(initialFakeValue) && !CompareQuaternionsWithTolerance(value, fakeValue))
			{
				PixelGuard.Instance.CreateSecurityWarning(TextCodes.MEMORY_HACKING_DETECTED, PixelGuard.Instance.GetModule<SecuredMemory>());
			}

			return value;
		}

		/// <summary>
		/// Compare Quaternion with Tolerance
		/// </summary>
		/// <param name="q1"></param>
		/// <param name="q2"></param>
		/// <returns></returns>
		private bool CompareQuaternionsWithTolerance(Quaternion q1, Quaternion q2)
		{
			SecuredMemory detector = (SecuredMemory)PixelGuard.Instance.GetModule<SecuredMemory>();
			float epsilon = detector.Options.QuaternionEpsilon;
			return Math.Abs(q1.x - q2.x) < epsilon &&
				   Math.Abs(q1.y - q2.y) < epsilon &&
				   Math.Abs(q1.z - q2.z) < epsilon &&
				   Math.Abs(q1.w - q2.w) < epsilon;
		}
		
		public static implicit operator SecuredQuaternion(Quaternion value)
		{
			SecuredQuaternion obscured = new SecuredQuaternion(Encrypt(value));
			if (PixelGuard.Instance.HasModule<SecuredMemory>())
			{
				obscured.fakeValue = value;
			}
			return obscured;
		}
		public static implicit operator Quaternion(SecuredQuaternion value)
		{
			return value.InternalDecrypt();
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		/// <summary>
		/// Returns a nicely formatted string of the Quaternion.
		/// </summary>
		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		/// <summary>
		/// Returns a nicely formatted string of the Quaternion.
		/// </summary>
		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}

		/// <summary>
		/// Used to store encrypted Quaternion.
		/// </summary>
		[Serializable]
		public struct RawEncryptedQuaternion
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

			/// <summary>
			/// Encrypted value
			/// </summary>
			public int w;
		}
    }
}