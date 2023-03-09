using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using PixelSecurity.Constants;
using PixelSecurity.Models;
using PixelSecurity.Modules;
using PixelSecurity.UI.CheatDetector;
using PixelSecurity.Util;
using UnityEngine;

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
namespace PixelSecurity{
    /// <summary>
    /// General Wrapper of Pixel Security Toolkit
    /// </summary>
    public class PixelGuard
    {
        // Security Instance
        private static PixelGuard _instance;
        public static PixelGuard Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PixelGuard();
                return _instance;
            }
        }
        
        // Mono Wrapper
        private PixelGuardOptions _toolkitSetup;
        private PixelMono _monoWrapper = null;
        private List<ISecurityModule> _modules = new List<ISecurityModule>();

        // Security Events
        public delegate void SecurityWarningHandler(string message, ISecurityModule module = null);     // Security Events
        [CanBeNull] public event SecurityWarningHandler OnSecurityMessage;                              // Security Message

        // Lifecycle Events
        public delegate void GameLoopUpdate(float deltaTime);
        public delegate void GameLoopFixedUpdate(float deltaTime);
        [CanBeNull] public event GameLoopUpdate OnLoopUpdate;
        [CanBeNull] public event GameLoopFixedUpdate OnLoopFixedUpdate;
        
        // Other parameters
        private bool _hasUI = false;

        /// <summary>
        /// Security Wrapper Constructor
        /// </summary>
        private PixelGuard()
        {
            CreateMonoWrapper();
            PreloadToolkitSetup();
        }

        /// <summary>
        /// Preload Toolkit Setup
        /// </summary>
        private void PreloadToolkitSetup()
        {
            // Load Data
            TextAsset loadedData = Resources.Load<TextAsset>(GlobalConstants.ConfigResourcePath);
            if (loadedData == null || string.IsNullOrEmpty(loadedData.text))
                return;
            
            // Convert from JSON
            _toolkitSetup = JsonUtility.FromJson<PixelGuardOptions>(loadedData.text);
            if (_toolkitSetup == null)
                return;
            
            // Check Auto UI
            if(_toolkitSetup.IsAutoUI)
                SetupDetectionUI();
        }

        #region UI Management
        /// <summary>
        /// Setup Automatic Detection UI
        /// </summary>
        private void SetupDetectionUI(Action onWindowClosed = null)
        {
            if(_hasUI)
                return;
            
            GameObject viewObject = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/CheatDetectionView"));
            CheatDetectedView cheatUI = viewObject.GetComponent<CheatDetectedView>();
            cheatUI.SetContext(new CheatDetectedView.Context
            {
                OnWindowClosed = onWindowClosed
            });
            _hasUI = true;
        }
        
        #endregion

        #region Mono Wrapper
        /// <summary>
        /// Create Mono Wrapper
        /// </summary>
        private void CreateMonoWrapper()
        {
            if(_monoWrapper != null)
                return;

            // Create Mono Wrapper
            GameObject wrapperObject = new GameObject("__PIXEL_WRAPPER__");
            _monoWrapper = wrapperObject.AddComponent<PixelMono>();
        }

        /// <summary>
        /// Invoke Coroutine at mono wrapper
        /// </summary>
        /// <param name="coroutine"></param>
        public void InvokeCoroutine(IEnumerator coroutine)
        {
            _monoWrapper.InvokeCoroutine(coroutine);
        }
        #endregion

        #region Modules Management
        /// <summary>
        /// Setup Security Module
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public ISecurityModule SetupModule<T>(ISecurityModule module) where T : ISecurityModule
        {
            if (_modules.Contains(module))
                return module;

            _modules.Add(module);
            return module;
        }

        /// <summary>
        /// Get Security Module
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ISecurityModule GetModule<T>() where T : ISecurityModule
        {
            foreach (ISecurityModule module in _modules)
            {
                if (module is T)
                    return module;
            }

            return null;
        }
        
        /// <summary>
        /// Check Module Contains
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool HasModule<T>() where T : ISecurityModule
        {
            return (GetModule<T>() != null);
        }

        /// <summary>
        /// Remove Security Module
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public void RemoveModule<T>() where T : ISecurityModule
        {
            for (int i = 0; i < _modules.Count; i++)
            {
                if (_modules[i] is T)
                {
                    _modules.Remove(_modules[i]);
                    return;
                }
            }
        }
        #endregion

        #region Event Fire
        /// <summary>
        /// Create Security Warning
        /// </summary>
        /// <param name="message"></param>
        /// <param name="module"></param>
        public void CreateSecurityWarning(string message, ISecurityModule module = null)
        {
            OnSecurityMessage?.Invoke(message, module);
        }

        /// <summary>
        /// Call Game Loop
        /// </summary>
        /// <param name="isFixed"></param>
        /// <param name="deltaTime"></param>
        public void CallGameLoop(bool isFixed, float deltaTime)
        {
            if(isFixed)
                OnLoopFixedUpdate?.Invoke(deltaTime);
            else
                OnLoopUpdate?.Invoke(deltaTime);
        }
        #endregion
    }
}
