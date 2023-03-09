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

using System.IO;
using System.Linq;
using System.Reflection;
using PixelSecurity.Constants;
using UnityEditor;
using UnityEngine;

namespace PixelSecurity.Editor
{
    /// <summary>
    /// Wizard and Editor Utils
    /// </summary>
    public class WizardUtils
    {
        private static readonly string[] hexTable = Enumerable.Range(0, 256).Select(v => v.ToString("x2")).ToArray();
        
        internal static void CleanInjectionDetectorData()
        {
            if (!File.Exists(GlobalConstants.INJECTION_DATA_PATH))
            {
                return;
            }

            RemoveReadOnlyAttribute(GlobalConstants.INJECTION_DATA_PATH);
            RemoveReadOnlyAttribute(GlobalConstants.INJECTION_DATA_PATH + ".meta");

            FileUtil.DeleteFileOrDirectory(GlobalConstants.INJECTION_DATA_PATH);
            FileUtil.DeleteFileOrDirectory(GlobalConstants.INJECTION_DATA_PATH + ".meta");

            RemoveDirectoryIfEmpty(GlobalConstants.RESOURCES_PATH);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }
        
        internal static string ResolveInjectionDefaultWhitelistPath()
        {
            return ResolveInjectionServiceFolder() + "/" + GlobalConstants.INJECTION_DEFAULT_WHITELIST_FILE;
        }
        
        internal static string ResolveInjectionUserWhitelistPath()
        {
            return ResolveInjectionServiceFolder() + "/" + GlobalConstants.INJECTION_USER_WHITELIST_FILE;
        }
        
        internal static string ResolveInjectionServiceFolder()
        {
            string result = "";
            string[] targetFiles = Directory.GetDirectories(GlobalConstants.ASSETS_PATH, GlobalConstants.INJECTION_SERVICE_FOLDER, SearchOption.AllDirectories);
            if (targetFiles.Length == 0)
            {
                Debug.LogError("Can't find " + GlobalConstants.INJECTION_SERVICE_FOLDER + " folder! Please report to " + GlobalConstants.REPORT_EMAIL);
            }
            else
            {
                result = targetFiles[0];
            }

            return result;
        }
        
        internal static string[] FindLibrariesAt(string dir)
        {
            string[] result = new string[0];

            if (Directory.Exists(dir))
            {
                result = Directory.GetFiles(dir, "*.dll", SearchOption.AllDirectories);
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = result[i].Replace('\\', '/');
                }
            }

            return result;
        }
        
        private static string PublicKeyTokenToString(byte[] bytes)
        {
            string result = "";

            // AssemblyName.GetPublicKeyToken() returns 8 bytes
            for (int i = 0; i < 8; i++)
            {
                result += hexTable[bytes[i]];
            }

            return result;
        }
        
        private static void RemoveDirectoryIfEmpty(string directoryName)
        {
            if (Directory.Exists(directoryName) && IsDirectoryEmpty(directoryName))
            {
                FileUtil.DeleteFileOrDirectory(directoryName);
                if (File.Exists(Path.GetDirectoryName(directoryName) + ".meta"))
                {
                    FileUtil.DeleteFileOrDirectory(Path.GetDirectoryName(directoryName) + ".meta");
                }
            }
        }
        
        private static bool IsDirectoryEmpty(string path)
        {
            string[] dirs = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);
            return dirs.Length == 0 && files.Length == 0;
        }
        
        internal static int GetAssemblyHash(AssemblyName ass)
        {
            string hashInfo = ass.Name;

            byte[] bytes = ass.GetPublicKeyToken();
            if (bytes != null && bytes.Length == 8)
            {
                hashInfo += PublicKeyTokenToString(bytes);
            }

            // Jenkins hash function (http://en.wikipedia.org/wiki/Jenkins_hash_function)
            int result = 0;
            int len = hashInfo.Length;

            for (int i = 0; i < len; ++i)
            {
                result += hashInfo[i];
                result += (result << 10);
                result ^= (result >> 6);
            }
            result += (result << 3);
            result ^= (result >> 11);
            result += (result << 15);

            return result;
        }
        
        internal static void RemoveReadOnlyAttribute(string path)
        {
            if (File.Exists(path))
            {
                FileAttributes attributes = File.GetAttributes(path);
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    attributes = attributes & ~FileAttributes.ReadOnly;
                    File.SetAttributes(path, attributes);
                }
            }
        }
    }
}