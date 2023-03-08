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
#define DEBUG
#undef DEBUG

#define DEBUG_VERBOSE
#undef DEBUG_VERBOSE

#define DEBUG_PARANOID
#undef DEBUG_PARANOID

using System.IO;
using System.Reflection;
using Debug = UnityEngine.Debug;
using System;
using PixelSecurity.Constants;
using PixelSecurity.Core.SecuredTypes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
    using System.Diagnostics;
#endif

namespace PixelSecurity.Modules.InjectionProtector
{
    /// <summary>
    /// Injection Protector Module
    /// </summary>
    public class InjectionProtector : ISecurityModule
    {
        #if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_IPHONE || UNITY_ANDROID || UNITY_STANDALONE_WIN
        private bool _signaturesAreNotGenuine;
        private AllowedAssembly[] _allowedAssemblies;
        private string[] _hexTable;

        [System.Serializable]
        public class ModuleOptions : IModuleConfig
        {
            public AllowedAssembly[] AllowedAssemblies;
        }
        private ModuleOptions _options;
        public ModuleOptions Options => _options;
        
        /// <summary>
        /// Injection Protector Module Setup
        /// </summary>
        /// <param name="options"></param>
        public InjectionProtector(ModuleOptions options = null)
        {
            if (options == null)
                _options = new ModuleOptions();

            if (_options.AllowedAssemblies != null && _options.AllowedAssemblies.Length > 0)
                _allowedAssemblies = _options.AllowedAssemblies;
            
            #if UNITY_EDITOR
            if (!EditorPrefs.GetBool("InjectionProtector", false))
            {
                Debug.LogWarning("Injection Protector is not enabled in Pixel Security Settings!\nPlease, check readme for details.");
                return;
            }
            
            #if !DEBUG && !DEBUG_VERBOSE && !DEBUG_PARANOID
            if (Application.isEditor)
            {
                Debug.LogWarning("Injection Protector does not work in editor (check readme for details).");
                return;
            }
            #else
			    Debug.LogWarning("Injection Protector works in debug mode. There WILL BE false positives in editor, it's fine!");
            #endif
            #endif
            
            if (_allowedAssemblies == null)
            {
                LoadAndParseAllowedAssemblies();
            }

            if (_signaturesAreNotGenuine)
            {
                OnInjectionDetected();
                return;
            }

            if (!FindInjectionInCurrentAssemblies())
            {
                AppDomain.CurrentDomain.AssemblyLoad += OnNewAssemblyLoaded;
            }
            else
            {
                OnInjectionDetected();
            }
        }

        /// <summary>
        /// On Destrutor
        /// </summary>
        ~InjectionProtector()
        {
            AppDomain.CurrentDomain.AssemblyLoad -= OnNewAssemblyLoaded;
            
        }
        
        /// <summary>
        /// On Injection Detected
        /// </summary>
        private void OnInjectionDetected()
        {
            PixelGuard.Instance.CreateSecurityWarning(TextCodes.INJECTION_HACKING_DETECTED, this);
        }
        
        /// <summary>
        /// On New Assembly Loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnNewAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
        {
            #if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
			    Debug.Log("New assembly loaded: " + args.LoadedAssembly.FullName);
            #endif
            
            if (!AssemblyAllowed(args.LoadedAssembly))
            {
                #if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
				    Debug.Log("Injected Assembly found:\n" + args.LoadedAssembly.FullName);
                #endif
                OnInjectionDetected();
            }
        }

        /// <summary>
        /// Find Injection in Current Assemblies
        /// </summary>
        /// <returns></returns>
        private bool FindInjectionInCurrentAssemblies()
        {
            bool result = false;
            #if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
			Stopwatch stopwatch = Stopwatch.StartNew();
            #endif
            
            Assembly[] assembliesInCurrentDomain = AppDomain.CurrentDomain.GetAssemblies();
            
            if (assembliesInCurrentDomain.Length == 0)
            {
                #if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
				stopwatch.Stop();
				Debug.Log("0 assemblies in current domain! Not genuine behavior.");
				stopwatch.Start();
                #endif
                result = true;
            }else
            {
                foreach (Assembly ass in assembliesInCurrentDomain)
                {
                    #if DEBUG_VERBOSE	
				    stopwatch.Stop();
				    Debug.Log("Currenly loaded assembly:\n" + ass.FullName);
				    stopwatch.Start();
                    #endif
                    
                    if (!AssemblyAllowed(ass))
                    {
                        #if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
						stopwatch.Stop();
						Debug.Log("Injected Assembly found:\n" + ass.FullName + "\n" + GetAssemblyHash(ass));
						stopwatch.Start();
                        #endif
                        
                        result = true;
                        break;
                    }
                }
            }
            
            #if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
			stopwatch.Stop();
			Debug.Log("Loaded assemblies scan duration: " + stopwatch.ElapsedMilliseconds + " ms.");
            #endif
            
            return result;
        }
        
        /// <summary>
        /// Check Allowed Assembly
        /// </summary>
        /// <param name="ass"></param>
        /// <returns></returns>
        private bool AssemblyAllowed(Assembly ass)
        {
            #if !UNITY_WEBPLAYER
            string assemblyName = ass.GetName().Name;
            #else
			string fullname = ass.FullName;
			string assemblyName = fullname.Substring(0, fullname.IndexOf(", ", StringComparison.Ordinal));
            #endif
            
            int hash = GetAssemblyHash(ass);
            
            bool result = false;
            for (int i = 0; i < _allowedAssemblies.Length; i++)
            {
                AllowedAssembly allowedAssembly = _allowedAssemblies[i];

                if (allowedAssembly.Name == assemblyName)
                {
                    if (Array.IndexOf(allowedAssembly.Hashes, hash) != -1)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }
        
        /// <summary>
        /// Load And Parse Allowed Assembly
        /// </summary>
        private void LoadAndParseAllowedAssemblies()
        {
            #if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
			Debug.Log("Starting LoadAndParseAllowedAssemblies()");
			Stopwatch sw = Stopwatch.StartNew();
            #endif
            
            TextAsset assembliesSignatures = (TextAsset)Resources.Load("assmdb", typeof(TextAsset));
            if (assembliesSignatures == null)
            {
                _signaturesAreNotGenuine = true;
                return;
            }
            
            #if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
			sw.Stop();
			Debug.Log("Creating separator array and opening MemoryStream");
			sw.Start();
            #endif
            
            string[] separator = {":"};

            MemoryStream ms = new MemoryStream(assembliesSignatures.bytes);
            BinaryReader br = new BinaryReader(ms);
			
            int count = br.ReadInt32();
            
            #if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
			sw.Stop();
			Debug.Log("Allowed assemblies count from MS: " + count);
			sw.Start();
            #endif
            
            _allowedAssemblies = new AllowedAssembly[count];
            
            for (int i = 0; i < count; i++)
            {
                string line = br.ReadString();
                #if (DEBUG_PARANOID)
				sw.Stop();
				Debug.Log("ine: " + line);
				sw.Start();
                #endif
                line = SecuredString.EncryptDecrypt(line, "TinyPlay");
                #if (DEBUG_PARANOID)
				sw.Stop();
				Debug.Log("Line decrypted : " + line);
				sw.Start();
                #endif
                
                string[] strArr = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                int stringsCount = strArr.Length;
                #if (DEBUG_PARANOID)
				sw.Stop();
				Debug.Log("stringsCount : " + stringsCount);
				sw.Start();
                #endif
                
                if (stringsCount > 1)
                {
                    string assemblyName = strArr[0];

                    int[] hashes = new int[stringsCount - 1];
                    for (int j = 1; j < stringsCount; j++)
                    {
                        hashes[j - 1] = int.Parse(strArr[j]);
                    }

                    _allowedAssemblies[i] = (new AllowedAssembly(assemblyName, hashes));
                }
                else
                {
                    _signaturesAreNotGenuine = true;
                    br.Close();
                    ms.Close();
                    #if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
					sw.Stop();
                    #endif
                    return;
                }
            }
            br.Close();
            ms.Close();
            Resources.UnloadAsset(assembliesSignatures);

            #if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
			sw.Stop();
			Debug.Log("Allowed Assemblies parsing duration: " + sw.ElapsedMilliseconds + " ms.");
            #endif

            _hexTable = new string[256];
            for (int i = 0; i < 256; i++)
            {
                _hexTable[i] = i.ToString("x2");
            }
        }
        
        /// <summary>
        /// Get Assembly Hash
        /// </summary>
        /// <param name="ass"></param>
        /// <returns></returns>
        private int GetAssemblyHash(Assembly ass)
        {
            string hashInfo;
            #if !UNITY_WEBPLAYER
            AssemblyName assName = ass.GetName();
            byte[] bytes = assName.GetPublicKeyToken();
            if (bytes.Length == 8)
            {
                hashInfo = assName.Name + PublicKeyTokenToString(bytes);
            }
            else
            {
                hashInfo = assName.Name;
            }
            #else
			string fullName = ass.FullName;

			string assemblyName = fullName.Substring(0, fullName.IndexOf(", ", StringComparison.Ordinal));
			int tokenIndex = fullName.IndexOf("PublicKeyToken=", StringComparison.Ordinal) + 15;
			string token = fullName.Substring(tokenIndex, fullName.Length - tokenIndex);
			if (token == "null") token = "";
			hashInfo = assemblyName + token;
            #endif
            
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
        
        #if !UNITY_WEBPLAYER
        private string PublicKeyTokenToString(byte[] bytes)
        {
            string result = "";
            for (int i = 0; i < 8; i++)
            {
                result += _hexTable[bytes[i]];
            }

            return result;
        }
        #endif
        #else
        public InjectionProtector(){
            Debug.LogError("Injection Protector is not supported on selected platform!");
        }
        #endif
    }
}