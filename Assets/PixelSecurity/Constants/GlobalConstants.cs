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

using UnityEngine;

namespace PixelSecurity.Constants
{
    /// <summary>
    /// Global Pixel Security Toolkit Constants
    /// </summary>
    public class GlobalConstants
    {
        public const string ConfigResourcePath = "Configs/ToolkitConfig";
        public const string SDKGit = "https://github.com/TinyPlay/PixelSecurityToolkit/";
        public const string SDKDocumentation = "https://github.com/TinyPlay/PixelSecurityToolkit/wiki";
        
        public const string PREFS_INJECTION_GLOBAL = "InjectorDetectionEnabledGlobal";
        public const string PREFS_INJECTION = "InjectorDetectionEnabled";
        public const string REPORT_EMAIL = "hello@flowsourcebox.co";
        
        public const string INJECTION_SERVICE_FOLDER = "InjectionProtectorData";
        public const string INJECTION_DEFAULT_WHITELIST_FILE = "DefaultWhitelist.pixac";
        public const string INJECTION_USER_WHITELIST_FILE = "UserWhitelist.pixac";
        public const string INJECTION_DATA_FILE = "assmdb.pixac";
        public const string INJECTION_DATA_SEPARATOR = ":";
        
        public const string ASSEMBLIES_PATH_RELATIVE = "Library/ScriptAssemblies";
        
        public static readonly string ASSETS_PATH = Application.dataPath;
        public static readonly string RESOURCES_PATH = ASSETS_PATH + "/Resources/";
        public static readonly string ASSEMBLIES_PATH = ASSETS_PATH + "/../" + ASSEMBLIES_PATH_RELATIVE;
        
        public static readonly string INJECTION_DATA_PATH = RESOURCES_PATH + INJECTION_DATA_FILE;
    }
}