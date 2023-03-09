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
using System.Runtime.Serialization.Formatters.Binary;
using PixelSecurity.Constants;

namespace PixelSecurity.Core.Serializer
{
    /// <summary>
    /// Binary Object Serializer
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public class BinarySerializer<TObject> : ISerializer<TObject> where TObject : class
    {
        [System.Serializable]
        public class SerializationOptions : ISerializerOptions
        {
            public string Path = "";
        }
        private SerializationOptions _options;
        
        /// <summary>
        /// Serializer Constructor
        /// </summary>
        /// <param name="options"></param>
        public BinarySerializer(SerializationOptions options = null)
        {
            // Check Options
            if (options == null)
                _options = new SerializationOptions();
            else
                _options = options;
            
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
            BinaryFormatter converter = new BinaryFormatter();
            FileStream dataStream = new FileStream(_options.Path, FileMode.Create);
            converter.Serialize(dataStream, dataToSave);
            dataStream.Close();
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

            BinaryFormatter converter = new BinaryFormatter();
            FileStream dataStream = new FileStream(_options.Path, FileMode.Open);
            inputObject = converter.Deserialize(dataStream) as TObject;
            dataStream.Close();
            return inputObject;
        }
    }
}