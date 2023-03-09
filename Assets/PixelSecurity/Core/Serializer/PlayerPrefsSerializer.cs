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
using System.Text;
using PixelSecurity.Constants;
using PixelSecurity.Core.Encryption;
using UnityEngine;

namespace PixelSecurity.Core.Serializer
{
    /// <summary>
    /// Player Prefs Object Serializer
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public class PlayerPrefsSerializer<TObject> : ISerializer<TObject> where TObject : class
    {
        [System.Serializable]
        public class SerializationOptions : ISerializerOptions
        {
            public string PlayerPrefsKey = "";
            public IDataEncryption Encryptor = null;
        }
        private SerializationOptions _options;
        
        /// <summary>
        /// Serializer Constructor
        /// </summary>
        /// <param name="options"></param>
        public PlayerPrefsSerializer(SerializationOptions options = null)
        {
            // Check Options
            if (options == null)
                _options = new SerializationOptions();
            else
                _options = options;
            
            // Check Base Path
            if (_options != null && string.IsNullOrEmpty(_options.PlayerPrefsKey))
                throw new Exception(TextCodes.PLAYER_PREFS_KEY_REQUIRED);
        }
        
        /// <summary>
        /// Save Object to File
        /// </summary>
        /// <param name="dataToSave"></param>
        public void SaveObject(TObject dataToSave)
        {
            string convertedData = JsonUtility.ToJson(dataToSave);
            if (_options.Encryptor != null)
                convertedData = _options.Encryptor.EncodeString(convertedData);
            PlayerPrefs.SetString(_options.PlayerPrefsKey, convertedData);
        }

        /// <summary>
        /// Load Object from File
        /// </summary>
        /// <param name="inputObject"></param>
        /// <returns></returns>
        public TObject LoadObject(TObject inputObject = null)
        {
            if (!PlayerPrefs.HasKey(_options.PlayerPrefsKey))
                return null;

            string reader = PlayerPrefs.GetString(_options.PlayerPrefsKey);
            if (_options.Encryptor != null)
                reader = _options.Encryptor.DecodeString(reader);
            inputObject = JsonUtility.FromJson<TObject>(reader);
            return inputObject;
        }
    }
}