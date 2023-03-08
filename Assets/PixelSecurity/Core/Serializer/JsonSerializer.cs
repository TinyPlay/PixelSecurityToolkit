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
using System.IO;
using System.Text;
using PixelSecurity.Constants;
using PixelSecurity.Core.Encryption;
using UnityEngine;

namespace PixelSecurity.Core.Serializer
{
    /// <summary>
    /// JSON Object Serializer
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public class JsonSerializer<TObject> : ISerializer<TObject> where TObject : class
    {
        [System.Serializable]
        public class SerializationOptions : ISerializerOptions
        {
            public string Path = "";
            public Encoding Encoding = Encoding.UTF8;
            public IDataEncryption Encryptor = null;
        }
        private SerializationOptions _options;
        
        /// <summary>
        /// Serializer Constructor
        /// </summary>
        /// <param name="options"></param>
        public JsonSerializer(SerializationOptions options = null)
        {
            // Check Options
            if (options == null)
                _options = new SerializationOptions();
            
            // Check Base Path
            if (_options != null && string.IsNullOrEmpty(_options.Path))
                throw new Exception(TextCodes.EMPTY_PATH_SERIALIZER);
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
            File.WriteAllText(_options.Path, convertedData, _options.Encoding);
        }

        /// <summary>
        /// Load Object from File
        /// </summary>
        /// <param name="inputObject"></param>
        /// <returns></returns>
        public TObject LoadObject(TObject inputObject = null)
        {
            if (!File.Exists(_options.Path))
                return null;

            string reader = File.ReadAllText(_options.Path, _options.Encoding);
            if (_options.Encryptor != null)
                reader = _options.Encryptor.DecodeString(reader);
            inputObject = JsonUtility.FromJson<TObject>(reader);
            return inputObject;
        }
    }
}