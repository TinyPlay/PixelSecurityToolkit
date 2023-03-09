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
namespace PixelSecurity.Constants
{
    /// <summary>
    /// Security Toolkit Text Codes
    /// </summary>
    public static class TextCodes
    {
        public const string ENCRYPTION_HASH_DECODING_ERROR = "Decrypting hashes is not possible.";
        public const string UNSUPPORTED_FOR_NON_STRING =
            "This encryption method is support only String encryption/decryption";

        public const string EMPTY_PATH_SERIALIZER = "Serializer was an empty path in the options. Please, setup path to serialize / deserialize objects.";
        public const string PLAYER_PREFS_KEY_REQUIRED =
            "For serialization/deserialization object to player prefs you need to specify PlayerPrefs key name in the serializator object.";

        public const string CHEATING_DETECTED = "Cheating Detected";
        public const string MEMORY_HACKING_DETECTED = "One of secured types was been changed in the game memory.";
        public const string INJECTION_HACKING_DETECTED = "Application Assembly Injection detected in the game assemblies.";
        public const string WALLHACK_DETECTED = "Wallhack Cheat Detected in the game lifecycle.";
        public const string TELEPORT_DETECTED = "Teleportation Detected for GameObject {0}. Available Distance per second {1}";
        public const string SPEEDHACK_DETECTED = "Speedhack Detected in the game timing system";
        public const string TIMECHANGE_DETECTED = "It looks like you changed the time on the device. Please change the date and time settings.";
        
        public const string ACCEPT = "Accept";
        public const string READ_MORE = "Read More";
    }
}