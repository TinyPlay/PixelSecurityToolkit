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
using PixelSecurity.Constants;
using PixelSecurity.Editor.Windows;
using PixelSecurity.Models;
using UnityEditor;
using UnityEngine;

namespace PixelSecurity.Editor
{
    /// <summary>
    /// Pixel Security Toolkit Wizard
    /// </summary>
    public class ToolkitWizard : EditorWindow
    {
        // Window Scroll
        private Vector2 _scrollPosition = Vector2.zero;
        private static EditorWindow _wizardWindow;
        
        // States
        private bool _isOptionsLoaded = false;
        private PixelGuardOptions _options = new PixelGuardOptions();
        private bool _enableInjectionDetector = false;
        private bool _isAutoUI = false;
        
        /// <summary>
        /// Show SDK Setup Window
        /// </summary>
        [MenuItem("Pixel Security/Settings", false, 0)]
        internal static void ShowWindow()
        {
            _wizardWindow = GetWindow<ToolkitWizard>(false, "Pixel Security Setup Wizard", true);
            _wizardWindow.minSize = new Vector2(500, 200);
            _wizardWindow.maxSize = new Vector2(500, 800);
        }
        
        /// <summary>
        /// Load Options
        /// </summary>
        private void LoadSetup()
        {
            // Load Asset from Resources
            TextAsset loadedData = Resources.Load<TextAsset>(GlobalConstants.ConfigResourcePath);
            if (loadedData == null || string.IsNullOrEmpty(loadedData.text))
                return;
            
            // Convert from JSON
            _options = JsonUtility.FromJson<PixelGuardOptions>(loadedData.text);

            // Not Specified Already
            if (_options == null)
                return;

            // Load Options
            _isAutoUI = _options.IsAutoUI;
            _isOptionsLoaded = true;
        }

        /// <summary>
        /// Save Setup
        /// </summary>
        private void SaveSetup()
        {
            // Setup Options
            _options.IsAutoUI = _isAutoUI;
            
            // Setup File Data
            string fileData = JsonUtility.ToJson(_options);
            
            // Save to File
            string path = $"Assets/PixelSecurity/Resources/{GlobalConstants.ConfigResourcePath}.json";
            string str = fileData;
            using (FileStream fs = new FileStream(path, FileMode.Create)){
                using (StreamWriter writer = new StreamWriter(fs)){
                    writer.Write(str);
                }
            }
            
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh ();
            #endif
        }

        /// <summary>
        /// On Draw Window GUI
        /// </summary>
        private void OnGUI()
        {
            if(!_isOptionsLoaded)
                LoadSetup();
            
            // Setup Scroll
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true, GUILayout.Width(500));
            
            // Draw Logo and Docs
            GUILayout.Box(Resources.Load("Wizard/Splash") as Texture2D, GetLogoStyle());
            if (GUILayout.Button("Open Documentation", GetButtonStyle())){
                Application.OpenURL(GlobalConstants.SDKDocumentation);
            }
            if (GUILayout.Button("Open GitHub", GetButtonStyle())){
                Application.OpenURL(GlobalConstants.SDKGit);
            }
            
            // General Setup
            DrawLine(1);
            DrawGeneralSettings();
            
            // Injection Protector
            DrawLine(1);
            DrawInjectionProtectorSettings();
            

            DrawLine(1);
            if (GUILayout.Button("Save Settings", GetButtonStyle()))
            {
                SaveSetup();
            }
            GUILayout.EndScrollView();
            
            // Apply Injection Protector
            SaveInjectionProtectorSettings();
        }

        /// <summary>
        /// Draw General Settings
        /// </summary>
        private void DrawGeneralSettings()
        {
            GUILayout.Label("General Setup", GetHeadlineStyle());
            GUILayout.BeginVertical(GetVerticalStyle());
            _isAutoUI = GUILayout.Toggle(_isAutoUI, "Enable Automatic UI for Cheating Detection");
            EditorGUILayout.Space();
            
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Injection Protector Settings
        /// </summary>
        private void DrawInjectionProtectorSettings()
        {
            GUILayout.Label("Injection Protector", GetHeadlineStyle());
            _enableInjectionDetector = false;
            _enableInjectionDetector = EditorPrefs.GetBool(GlobalConstants.PREFS_INJECTION_GLOBAL);
            GUILayout.BeginVertical(GetVerticalStyle());
            _enableInjectionDetector = GUILayout.Toggle(_enableInjectionDetector, "Enable Injection Detector");
            GUILayout.EndVertical();
            EditorGUILayout.Space();
            if (_enableInjectionDetector)
            {
                if (GUILayout.Button("Edit Whitelist", GetButtonStyle())){
                    AssembliesWhitelist.ShowWindow();
                }
            }
        }

        /// <summary>
        /// Save Injection Protector Settings
        /// </summary>
        private void SaveInjectionProtectorSettings()
        {
            if (GUI.changed || EditorPrefs.GetBool(GlobalConstants.PREFS_INJECTION) != _enableInjectionDetector)
            {
                EditorPrefs.SetBool(GlobalConstants.PREFS_INJECTION, _enableInjectionDetector);
                EditorPrefs.SetBool(GlobalConstants.PREFS_INJECTION_GLOBAL, _enableInjectionDetector);
            }
            if (!_enableInjectionDetector)
            {
                WizardUtils.CleanInjectionDetectorData();
            }
            else if (!File.Exists(GlobalConstants.INJECTION_DATA_PATH))
            {
                PostProcessor.InjectionAssembliesScan();
            }
        }
        
        /// <summary>
        /// On Lost Window Focus
        /// </summary>
        private void OnLostFocus()
        {
            SaveSetup();
        }

        /// <summary>
        /// On Focus Window
        /// </summary>
        private void OnFocus()
        {
            LoadSetup();
        }
        
        /// <summary>
        /// Get Logo Style
        /// </summary>
        /// <returns></returns>
        private GUIStyle GetLogoStyle()
        {
            GUIStyle logoStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                border = new RectOffset(0, 0, 0, 0),
                fixedWidth = 487
            };
            logoStyle.margin = new RectOffset(0, 0, 0, 0);
            logoStyle.overflow = new RectOffset(0, 0, 0, 0);
            logoStyle.padding = new RectOffset(0, 0, 0, 0);

            return logoStyle;
        }
        
        /// <summary>
        /// Get Button Style
        /// </summary>
        /// <returns></returns>
        private GUIStyle GetButtonStyle()
        {
            GUIStyle buttonStyle = new GUIStyle()
            {
                fixedHeight = 40,
                alignment = TextAnchor.MiddleCenter,
                border = new RectOffset(1,1,1,1),
                fontSize = 16,
                fontStyle = FontStyle.Bold
            };
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.normal.background = Resources.Load("Wizard/Button") as Texture2D;
            buttonStyle.margin = new RectOffset(15, 15, 15, 15);
            
            buttonStyle.active.textColor = Color.white;
            buttonStyle.active.background = Resources.Load("Wizard/ButtonHover") as Texture2D;

            return buttonStyle;
        }
        
        /// <summary>
        /// Get Vertical Area Style
        /// </summary>
        /// <returns></returns>
        private GUIStyle GetVerticalStyle()
        {
            GUIStyle areaStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter
            };
            areaStyle.margin = new RectOffset(15, 15, 0, 0);

            return areaStyle;
        }
        
        /// <summary>
        /// Get Headline Style
        /// </summary>
        /// <returns></returns>
        private GUIStyle GetHeadlineStyle()
        {
            GUIStyle headlineStyle = new GUIStyle()
            {
                fixedHeight = 40,
                alignment = TextAnchor.MiddleCenter,
                fontSize = 20,
                fontStyle = FontStyle.Bold
            };
            headlineStyle.normal.textColor = Color.white;

            return headlineStyle;
        }
        
        /// <summary>
        /// Get Description Style
        /// </summary>
        /// <returns></returns>
        private GUIStyle GetDescriptionStyle()
        {
            GUIStyle headlineStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                fontStyle = FontStyle.Normal
            };
            headlineStyle.normal.textColor = Color.gray;

            return headlineStyle;
        }
        
        /// <summary>
        /// Draw Separator Line
        /// </summary>
        /// <param name="height"></param>
        private void DrawLine(int height)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, height );
            rect.height = height;
            EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 0.5f ) );
        }
    }
}