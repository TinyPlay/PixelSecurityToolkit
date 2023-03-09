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
using System.Xml.Serialization;
using PixelSecurity.Constants;
using PixelSecurity.Core.Encryption;

namespace PixelSecurity.Core.Serializer
{
    /// <summary>
    /// XML Object Serializer
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public class XMLSerializer<TObject> : ISerializer<TObject> where TObject : class
    {
        [System.Serializable]
        public class SerializationOptions : ISerializerOptions
        {
            public string Path = "";
            public Encoding Encoding = Encoding.UTF8;
        }
        private SerializationOptions _options;
        
        /// <summary>
        /// Serializer Constructor
        /// </summary>
        /// <param name="options"></param>
        public XMLSerializer(SerializationOptions options = null)
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
            XmlSerializer serializer = new XmlSerializer(typeof(TObject));
            using (TextWriter writer = new StreamWriter(_options.Path, false, _options.Encoding))
            {
                serializer.Serialize(writer, dataToSave);
            }
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
            
            XmlSerializer deserializer = new XmlSerializer(typeof(TObject));
            using (TextReader reader = new StreamReader(_options.Path, _options.Encoding))
            {
                inputObject = (TObject) deserializer.Deserialize(reader);
            }
            
            return inputObject;
        }
    }
}