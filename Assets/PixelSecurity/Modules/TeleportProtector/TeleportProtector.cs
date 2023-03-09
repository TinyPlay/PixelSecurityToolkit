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
using System.Collections.Generic;
using PixelSecurity.Constants;
using UnityEngine;

namespace PixelSecurity.Modules.TeleportProtector
{
    /// <summary>
    /// Teleport Protector Module
    /// </summary>
    public class TeleportProtector : ISecurityModule
    {
        [System.Serializable]
        public class ModuleOptions : IModuleConfig { }
        private ModuleOptions _options;
        public ModuleOptions Options => _options;

        // Teleport Seek Targets
        private bool _isSeeking = false;
        private List<TeleportTarget> _seekTargets = new List<TeleportTarget>();
        private float _seekTimer = 1f;
        
        /// <summary>
        /// Teleport Protector Module
        /// </summary>
        /// <param name="options"></param>
        public TeleportProtector(ModuleOptions options = null)
        {
            if (options == null)
                _options = new ModuleOptions();

            InitProtector();
        }

        /// <summary>
        /// On Module Destoryed
        /// </summary>
        ~TeleportProtector()
        {
            DisposeProtector();
        }

        /// <summary>
        /// Initialize Protector
        /// </summary>
        private void InitProtector()
        {
            _seekTimer = 1f;
            PixelGuard.Instance.OnLoopUpdate += OnUpdate;
        }

        /// <summary>
        /// Dispose Protector
        /// </summary>
        private void DisposeProtector()
        {
            PixelGuard.Instance.OnLoopUpdate -= OnUpdate;
        }
        
        /// <summary>
        /// On Game Loop Update
        /// </summary>
        /// <param name="deltaTime"></param>
        private void OnUpdate(float deltaTime)
        {
            if (_seekTimer <= 0f)
            {
                _seekTimer = 1f;
                if(_isSeeking) CheckDistances();
            }
            else
            {
                _seekTimer -= deltaTime;
            }
        }

        /// <summary>
        /// Check Target Distances
        /// </summary>
        private void CheckDistances()
        {
            for (int i = 0; i < _seekTargets.Count; i++)
            {
                if (Vector3.Distance(_seekTargets[i].TargetTransform.position, _seekTargets[i].LastPosition) >
                    _seekTargets[i].MaxDistancePerSecond)
                    DetectTeleport(_seekTargets[i]);
                else
                    _seekTargets[i].LastPosition = _seekTargets[i].TargetTransform.position;
            }
        }

        /// <summary>
        /// Pause / Resume Detection Check
        /// </summary>
        public void PauseSeeking(bool isPaused)
        {
            if (!isPaused)
            {
                for (int i = 0; i < _seekTargets.Count; i++)
                {
                    _seekTargets[i].LastPosition = _seekTargets[i].TargetTransform.position;
                }
            }
            _isSeeking = isPaused;
        }
        
        /// <summary>
        /// Add Target for seeking
        /// </summary>
        /// <param name="target"></param>
        public void AddTarget(TeleportTarget target)
        {
            _seekTargets.Add(target);
        }

        /// <summary>
        /// Remove Target from seeking list
        /// </summary>
        /// <param name="target"></param>
        public void RemoveTarget(TeleportTarget target)
        {
            if (_seekTargets.Contains(target))
                _seekTargets.Remove(target);
        }

        /// <summary>
        /// Clear Seek Targets
        /// </summary>
        public void ClearTargets()
        {
            _seekTargets.Clear();
        }

        /// <summary>
        /// Detect Teleport
        /// </summary>
        /// <param name="target"></param>
        private void DetectTeleport(TeleportTarget target){
            string teleportMessage = String.Format(TextCodes.TELEPORT_DETECTED, target.TargetTransform.gameObject.name, target.MaxDistancePerSecond);
            PixelGuard.Instance.CreateSecurityWarning(teleportMessage, this);
        }
    }
}