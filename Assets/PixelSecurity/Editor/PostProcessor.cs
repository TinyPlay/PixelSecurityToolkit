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

#define DEBUG_PARANIOD
#undef DEBUG_PARANIOD

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PixelSecurity.Constants;
using PixelSecurity.Core.SecuredTypes;
using PixelSecurity.Editor.Common;
using UnityEditor;
using UnityEditor.Callbacks;
using Debug = UnityEngine.Debug;
#if (DEBUG || DEBUG_VERBOSE || DEBUG_PARANIOD)
using System.Diagnostics;
#endif

namespace PixelSecurity.Editor
{
    public class PostProcessor
    {
        private static readonly List<AllowedAssembly> allowedAssemblies = new List<AllowedAssembly>();
        private static readonly List<string> allLibraries = new List<string>();
        
		#if (DEBUG || DEBUG_VERBOSE || DEBUG_PARANIOD)
		[UnityEditor.MenuItem("Pixel Security/Force Collect Data for Injection Detector", 2)]
		private static void CallInjectionScan()
		{
			InjectionAssembliesScan(true); 
		}
		#endif
	    
	    private static void OnPostprocessAllAssets(String[] mportedAssets, String[] deletedAssets, String[] movedAssets, String[] movedFromAssetPaths)
	    {
		    if (!EditorPrefs.GetBool(GlobalConstants.PREFS_INJECTION_GLOBAL)) return;
		    if (!InjectionDetectorTargetCompatibleCheck()) return;

		    if (deletedAssets.Length > 0)
		    {
			    foreach (string deletedAsset in deletedAssets)
			    {
				    if (deletedAsset.IndexOf(GlobalConstants.INJECTION_DATA_FILE) > -1 && !EditorApplication.isCompiling)
				    {
						#if (DEBUG || DEBUG_VERBOSE || DEBUG_PARANIOD)
						Debug.LogWarning("Looks like Injection Protector data file was accidentally removed! Re-creating...\nIf you wish to remove " + GlobalConstants.INJECTION_DATA_FILE + " file, just disable Injection Detecotr in the Anti-Cheat Options window.");
						#endif
					    InjectionAssembliesScan();
				    }
			    }
		    }
	    }
	    
	    // called by Unity
	    [DidReloadScripts]
	    private static void ScriptsWereReloaded()
	    {
		    EditorUserBuildSettings.activeBuildTargetChanged += OnBuildTargetChanged;
		    if (EditorPrefs.GetBool(GlobalConstants.PREFS_INJECTION_GLOBAL))
		    {
			    InjectionAssembliesScan();
		    }
	    }
	    
	    private static void OnBuildTargetChanged()
	    {
		    InjectionDetectorTargetCompatibleCheck();
	    }

	    internal static void InjectionAssembliesScan()
	    {
		    InjectionAssembliesScan(false);
	    }
	    
	    internal static void InjectionAssembliesScan(bool forced)
		{
			if (!InjectionDetectorTargetCompatibleCheck() && !forced)
			{
				return;
			}

			#if (DEBUG || DEBUG_VERBOSE || DEBUG_PARANIOD)
			Stopwatch sw = Stopwatch.StartNew();
	#if (DEBUG_VERBOSE || DEBUG_PARANIOD)
			sw.Stop();
			Debug.Log("Injection Detector Assemblies Scan\n");
			Debug.Log("Paths:\n" +

			          "Assets: " + GlobalConstants.ASSETS_PATH + "\n" +
			          "Assemblies: " + GlobalConstants.ASSEMBLIES_PATH + "\n" +
			          "Injection Detector Data: " + GlobalConstants.INJECTION_DATA_PATH);
			sw.Start();
	#endif
			#endif

#if (DEBUG_VERBOSE || DEBUG_PARANIOD)
			sw.Stop();
			Debug.Log("Looking for all assemblies in current project...");
			sw.Start();
#endif
			allLibraries.Clear();
			allowedAssemblies.Clear();

			allLibraries.AddRange(WizardUtils.FindLibrariesAt(GlobalConstants.ASSETS_PATH));
			allLibraries.AddRange(WizardUtils.FindLibrariesAt(GlobalConstants.ASSEMBLIES_PATH));
#if (DEBUG_VERBOSE || DEBUG_PARANIOD)
			sw.Stop();
			Debug.Log("Total libraries found: " + allLibraries.Count);
			sw.Start();
#endif
			const string editorSubdir = "/editor/";
			string assembliesPathLowerCase = GlobalConstants.ASSEMBLIES_PATH_RELATIVE.ToLower();
			foreach (string libraryPath in allLibraries)
			{
				string libraryPathLowerCase = libraryPath.ToLower();
#if (DEBUG_PARANIOD)
				sw.Stop();
				Debug.Log("Checking library at the path: " + libraryPathLowerCase);
				sw.Start();
#endif
				if (libraryPathLowerCase.Contains(editorSubdir)) continue;
				if (libraryPathLowerCase.Contains("-editor.dll") && libraryPathLowerCase.Contains(assembliesPathLowerCase)) continue;

				try
				{
					AssemblyName assName = AssemblyName.GetAssemblyName(libraryPath);
					string name = assName.Name;
					int hash = WizardUtils.GetAssemblyHash(assName);

					AllowedAssembly allowed = allowedAssemblies.FirstOrDefault(allowedAssembly => allowedAssembly.name == name);

					if (allowed != null)
					{
						allowed.AddHash(hash);
					}
					else
					{
						allowed = new AllowedAssembly(name, new[] {hash});
						allowedAssemblies.Add(allowed);
					}
				}
				catch
				{
					// not a valid IL assembly, skipping
				}
			}

			#if (DEBUG || DEBUG_VERBOSE || DEBUG_PARANIOD)
			sw.Stop();
			string trace = "Found assemblies (" + allowedAssemblies.Count + "):\n";

			foreach (AllowedAssembly allowedAssembly in allowedAssemblies)
			{
				trace += "  Name: " + allowedAssembly.name + "\n";
				trace = allowedAssembly.hashes.Aggregate(trace, (current, hash) => current + ("    Hash: " + hash + "\n"));
			}

			Debug.Log(trace);
			sw.Start();
			#endif
			if (!Directory.Exists(GlobalConstants.RESOURCES_PATH))
			{
				#if (DEBUG_VERBOSE || DEBUG_PARANIOD)
				sw.Stop();
				Debug.Log("Creating resources folder: " + GlobalConstants.RESOURCES_PATH);
				sw.Start();
				#endif
				Directory.CreateDirectory(GlobalConstants.RESOURCES_PATH);
			}

			WizardUtils.RemoveReadOnlyAttribute(GlobalConstants.INJECTION_DATA_PATH);
			BinaryWriter bw = new BinaryWriter(new FileStream(GlobalConstants.INJECTION_DATA_PATH, FileMode.Create, FileAccess.Write, FileShare.Read));
			int allowedAssembliesCount = allowedAssemblies.Count;

			int totalWhitelistedAssemblies = 0;

			#if (DEBUG_VERBOSE || DEBUG_PARANIOD)
			sw.Stop();
			Debug.Log("Processing default whitelist");
			sw.Start();
			#endif

			string defaultWhitelistPath = WizardUtils.ResolveInjectionDefaultWhitelistPath();
			if (File.Exists(defaultWhitelistPath))
			{
				BinaryReader br = new BinaryReader(new FileStream(defaultWhitelistPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
				int assembliesCount = br.ReadInt32();
				totalWhitelistedAssemblies = assembliesCount + allowedAssembliesCount;

				bw.Write(totalWhitelistedAssemblies);

				for (int i = 0; i < assembliesCount; i++)
				{
					bw.Write(br.ReadString());
				}
				br.Close();
			}
			else
			{
				#if (DEBUG || DEBUG_VERBOSE || DEBUG_PARANIOD)
				sw.Stop();
				#endif
				bw.Close();
				Debug.LogError("Can't find " + GlobalConstants.INJECTION_DEFAULT_WHITELIST_FILE + " file!\nPlease, report to " + GlobalConstants.REPORT_EMAIL);
				return;
			}

			#if (DEBUG_VERBOSE || DEBUG_PARANIOD)
			sw.Stop();
			Debug.Log("Processing user whitelist");
			sw.Start();
			#endif

			string userWhitelistPath = WizardUtils.ResolveInjectionUserWhitelistPath();
			if (File.Exists(userWhitelistPath))
			{
				BinaryReader br = new BinaryReader(new FileStream(userWhitelistPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
				int assembliesCount = br.ReadInt32();

				bw.Seek(0, SeekOrigin.Begin);
				bw.Write(totalWhitelistedAssemblies + assembliesCount);
				bw.Seek(0, SeekOrigin.End);
				for (int i = 0; i < assembliesCount; i++)
				{
					bw.Write(br.ReadString());
				}
				br.Close();
			}

			#if (DEBUG_VERBOSE || DEBUG_PARANIOD)
			sw.Stop();
			Debug.Log("Processing project assemblies");
			sw.Start();
			#endif

			for (int i = 0; i < allowedAssembliesCount; i++)
			{
				AllowedAssembly assembly = allowedAssemblies[i];
				string name = assembly.name;
				string hashes = "";

				for (int j = 0; j < assembly.hashes.Length; j++)
				{
					hashes += assembly.hashes[j];
					if (j < assembly.hashes.Length - 1)
					{
						hashes += GlobalConstants.INJECTION_DATA_SEPARATOR;
					}
				}

				string line = SecuredString.EncryptDecrypt(name + GlobalConstants.INJECTION_DATA_SEPARATOR + hashes, "TinyPlay");
				
				#if (DEBUG_VERBOSE || DEBUG_PARANIOD)
				Debug.Log("Writing assembly:\n" + name + GlobalConstants.INJECTION_DATA_SEPARATOR + hashes);
				#endif
				bw.Write(line);
			}

			bw.Close();			 
			#if (DEBUG || DEBUG_VERBOSE || DEBUG_PARANIOD)
			sw.Stop();
			Debug.Log("Assemblies scan duration: " + sw.ElapsedMilliseconds + " ms.");
			#endif

			if (allowedAssembliesCount == 0)
			{
				Debug.LogError("Can't find any assemblies!\nPlease, report to " + GlobalConstants.REPORT_EMAIL);
			}

			AssetDatabase.Refresh();
			//EditorApplication.UnlockReloadAssemblies();
		}

		private static bool InjectionDetectorTargetCompatibleCheck()
		{
			#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_IPHONE || UNITY_ANDROID
			if (EditorPrefs.GetBool(GlobalConstants.PREFS_INJECTION_GLOBAL) && !EditorPrefs.GetBool(GlobalConstants.PREFS_INJECTION))
			{
				EditorPrefs.SetBool(GlobalConstants.PREFS_INJECTION, true);
			}

			return true;
			
			#else
			bool injectionEnabled = EditorPrefs.GetBool(GlobalConstants.PREFS_INJECTION_GLOBAL);
			if (injectionEnabled && EditorPrefs.GetBool(GlobalConstants.PREFS_INJECTION))
			{
				Debug.LogWarning("Injection Detector is not available on selected platform (" + EditorUserBuildSettings.activeBuildTarget + ") and will be disabled!");
				EditorPrefs.SetBool(GlobalConstants.PREFS_INJECTION, false);
				WizardUtils.CleanInjectionDetectorData();
			}
			return false;
			#endif
		}
    }
}