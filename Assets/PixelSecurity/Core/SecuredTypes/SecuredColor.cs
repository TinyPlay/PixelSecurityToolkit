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
    public struct SecuredColor
    {
        private static int _cryptoKey = 120222;
        private static readonly Color initialFakeValue = Color.black;

        // Serialized Fields
        [SerializeField] private int currentCryptoKey;
        [SerializeField] private RawEncryptedColor hiddenValue;
        [SerializeField] private Color fakeValue;
        [SerializeField] private bool inited;
        
        /// <summary>
        /// Secured Color Constructor
        /// </summary>
        /// <param name="encrypted"></param>
        private SecuredColor(RawEncryptedColor encrypted)
        {
            currentCryptoKey = _cryptoKey;
            hiddenValue = encrypted;
            fakeValue = initialFakeValue;
            inited = true;
        }
        
        /// <summary>
        /// Color Red
        /// </summary>
        public float r
        {
            get
            {
	            SecuredMemory detector =
		            (SecuredMemory) PixelGuard.Instance.GetModule<SecuredMemory>();
	            float decrypted = InternalDecryptField(hiddenValue.r);
                if (detector!=null && !fakeValue.Equals(initialFakeValue) && Math.Abs(decrypted - fakeValue.r) > detector.Options.ColorEpsilon)
                {
	                PixelGuard.Instance.CreateSecurityWarning(TextCodes.MEMORY_HACKING_DETECTED, PixelGuard.Instance.GetModule<SecuredMemory>());
                }
                return decrypted;
            }

            set
            {
	            hiddenValue.r = InternalEncryptField(value);
                if (PixelGuard.Instance.HasModule<SecuredMemory>())
                {
                    fakeValue.r = value;
                }
            }
        }
        
        /// <summary>
        /// Color Green
        /// </summary>
        public float g
        {
            get
            {
	            SecuredMemory detector =
		            (SecuredMemory) PixelGuard.Instance.GetModule<SecuredMemory>();
	            float decrypted = InternalDecryptField(hiddenValue.g);
                if (detector!=null && !fakeValue.Equals(initialFakeValue) && Math.Abs(decrypted - fakeValue.g) > detector.Options.ColorEpsilon)
                {
	                PixelGuard.Instance.CreateSecurityWarning(TextCodes.MEMORY_HACKING_DETECTED, PixelGuard.Instance.GetModule<SecuredMemory>());
                }
                return decrypted;
            }

            set
            {
	            hiddenValue.g = InternalEncryptField(value);
                if (PixelGuard.Instance.HasModule<SecuredMemory>())
                {
                    fakeValue.g = value;
                }
            }
        }
        
        /// <summary>
        /// Color Blue
        /// </summary>
        public float b
        {
            get
            {
	            SecuredMemory detector =
		            (SecuredMemory) PixelGuard.Instance.GetModule<SecuredMemory>();
	            float decrypted = InternalDecryptField(hiddenValue.b);
                if (detector!=null && !fakeValue.Equals(initialFakeValue) && Math.Abs(decrypted - fakeValue.b) > detector.Options.ColorEpsilon)
                {
	                PixelGuard.Instance.CreateSecurityWarning(TextCodes.MEMORY_HACKING_DETECTED, PixelGuard.Instance.GetModule<SecuredMemory>());
                }
                return decrypted;
            }

            set
            {
	            hiddenValue.b = InternalEncryptField(value);
                if (PixelGuard.Instance.HasModule<SecuredMemory>())
                {
                    fakeValue.b = value;
                }
            }
        }
        
        /// <summary>
        /// Color Alpha
        /// </summary>
        public float a
        {
            get
            {
	            SecuredMemory detector =
		            (SecuredMemory) PixelGuard.Instance.GetModule<SecuredMemory>();
                float decrypted = InternalDecryptField(hiddenValue.a);
                if (detector!=null && !fakeValue.Equals(initialFakeValue) && Math.Abs(decrypted - fakeValue.a) > detector.Options.ColorEpsilon)
                {
	                PixelGuard.Instance.CreateSecurityWarning(TextCodes.MEMORY_HACKING_DETECTED, PixelGuard.Instance.GetModule<SecuredMemory>());
                }
                return decrypted;
            }

            set
            {
	            hiddenValue.a = InternalEncryptField(value);
                if (PixelGuard.Instance.HasModule<SecuredMemory>())
                {
                    fakeValue.a = value;
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
                        return r;
                    case 1:
                        return g;
                    case 2:
                        return b;
                    case 3:
                        return a;
                    default:
                        throw new IndexOutOfRangeException("Invalid SecuredColor index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        r = value;
                        break;
                    case 1:
                        g = value;
                        break;
                    case 2:
                        b = value;
                        break;
                    case 3:
                        a = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid SecuredColor index!");
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
		public static RawEncryptedColor Encrypt(Color value)
		{
			return Encrypt(value, 0);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any Vector3 value, uses passed crypto key.
		/// </summary>
		public static RawEncryptedColor Encrypt(Color value, int key)
		{
			if (key == 0)
			{
				key = _cryptoKey;
			}

			RawEncryptedColor result;
			result.r = SecuredFloat.Encrypt(value.r, key);
			result.g = SecuredFloat.Encrypt(value.g, key);
			result.b = SecuredFloat.Encrypt(value.b, key);
			result.a = SecuredFloat.Encrypt(value.a, key);

			return result;
		}

		/// <summary>
		/// Use it to decrypt RawEncryptedVector3 you got from Encrypt(), uses default crypto key.
		/// </summary>
		public static Color Decrypt(RawEncryptedColor value)
		{
			return Decrypt(value, 0);
		}

		/// <summary>
		/// Use it to decrypt RawEncryptedVector3 you got from Encrypt(), uses passed crypto key.
		/// </summary>
		public static Color Decrypt(RawEncryptedColor value, int key)
		{
			if (key == 0)
			{
				key = _cryptoKey;
			}

			Color result;
			result.r = SecuredFloat.Decrypt(value.r, key);
			result.g = SecuredFloat.Decrypt(value.g, key);
			result.b = SecuredFloat.Decrypt(value.b, key);
			result.a = SecuredFloat.Decrypt(value.a, key);

			return result;
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		public RawEncryptedColor GetEncrypted()
		{
			ApplyNewCryptoKey();
			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		public void SetEncrypted(RawEncryptedColor encrypted)
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
		private Color InternalDecrypt()
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

			Color value;

			value.r = SecuredFloat.Decrypt(hiddenValue.r, key);
			value.g = SecuredFloat.Decrypt(hiddenValue.g, key);
			value.b = SecuredFloat.Decrypt(hiddenValue.b, key);
			value.a = SecuredFloat.Decrypt(hiddenValue.a, key);

			SecuredMemory detector =
				(SecuredMemory) PixelGuard.Instance.GetModule<SecuredMemory>();
			if (PixelGuard.Instance.HasModule<SecuredMemory>() && !fakeValue.Equals(Color.black) && !CompareVectorsWithTolerance(value, fakeValue))
			{
				PixelGuard.Instance.CreateSecurityWarning(TextCodes.MEMORY_HACKING_DETECTED, PixelGuard.Instance.GetModule<SecuredMemory>());
			}

			return value;
		}

		/// <summary>
		/// Compare Vectors with Tolerance
		/// </summary>
		/// <param name="color1"></param>
		/// <param name="color2"></param>
		/// <returns></returns>
		private bool CompareVectorsWithTolerance(Color color1, Color color2)
		{
			SecuredMemory detector =
				(SecuredMemory) PixelGuard.Instance.GetModule<SecuredMemory>();
			float epsilon = detector.Options.ColorEpsilon;
			return Math.Abs(color1.r - color2.r) < epsilon &&
				   Math.Abs(color1.g - color2.g) < epsilon &&
				   Math.Abs(color1.b - color2.b) < epsilon &&
				   Math.Abs(color1.a - color2.a) < epsilon;
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
		public static implicit operator SecuredColor(Color value)
		{
			SecuredColor obscured = new SecuredColor(Encrypt(value, _cryptoKey));
			if (PixelGuard.Instance.HasModule<SecuredMemory>())
			{
				obscured.fakeValue = value;
			}
			return obscured;
		}
		public static implicit operator Color(SecuredColor value)
		{
			return value.InternalDecrypt();
		}
		public static SecuredColor operator +(SecuredColor a, SecuredColor b)
		{
			return a.InternalDecrypt() + b.InternalDecrypt();
		}
		public static SecuredColor operator +(Color a, SecuredColor b)
		{
			return a + b.InternalDecrypt();
		}
		public static SecuredColor operator +(SecuredColor a, Color b)
		{
			return a.InternalDecrypt() + b;
		}
		public static SecuredColor operator -(SecuredColor a, SecuredColor b)
		{
			return a.InternalDecrypt() - b.InternalDecrypt();
		}
		public static SecuredColor operator -(Color a, SecuredColor b)
		{
			return a - b.InternalDecrypt();
		}
		public static SecuredColor operator -(SecuredColor a, Color b)
		{
			return a.InternalDecrypt() - b;
		}
		public static SecuredColor operator *(SecuredColor a, float d)
		{
			return a.InternalDecrypt() * d;
		}
		public static SecuredColor operator *(float d, SecuredColor a)
		{
			return d * a.InternalDecrypt();
		}
		public static SecuredColor operator /(SecuredColor a, float d)
		{
			return a.InternalDecrypt() / d;
		}

		public static bool operator ==(SecuredColor lhs, SecuredColor rhs)
		{
			return lhs.InternalDecrypt() == rhs.InternalDecrypt();
		}
		public static bool operator ==(Color lhs, SecuredColor rhs)
		{
			return lhs == rhs.InternalDecrypt();
		}
		public static bool operator ==(SecuredColor lhs, Color rhs)
		{
			return lhs.InternalDecrypt() == rhs;
		}

		public static bool operator !=(SecuredColor lhs, SecuredColor rhs)
		{
			return lhs.InternalDecrypt() != rhs.InternalDecrypt();
		}
		public static bool operator !=(Color lhs, SecuredColor rhs)
		{
			return lhs != rhs.InternalDecrypt();
		}
		public static bool operator !=(SecuredColor lhs, Color rhs)
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
		/// Returns a nicely formatted string for this color.
		/// </summary>
		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		/// <summary>
		/// Returns a nicely formatted string for this color.
		/// </summary>
		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}

		/// <summary>
		/// Used to store encrypted Color.
		/// </summary>
		[Serializable]
		public struct RawEncryptedColor
		{
			/// <summary>
			/// Encrypted value
			/// </summary>
			public int r;

			/// <summary>
			/// Encrypted value
			/// </summary>
			public int g;

			/// <summary>
			/// Encrypted value
			/// </summary>
			public int b;

			/// <summary>
			/// Encrypted Value
			/// </summary>
			public int a;
		}
    }
}