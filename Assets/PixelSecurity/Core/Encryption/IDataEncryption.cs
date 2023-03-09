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
namespace PixelSecurity.Core.Encryption
{
    /// <summary>
    /// Base Data Encryption Interface
    /// </summary>
    public interface IDataEncryption
    {
        string DecodeString(string encodedString);
        byte[] DecodeBinary(byte[] encodedData);
        string EncodeString(string decodedString);
        byte[] EncodeBinary(byte[] decodedData);
    }
}