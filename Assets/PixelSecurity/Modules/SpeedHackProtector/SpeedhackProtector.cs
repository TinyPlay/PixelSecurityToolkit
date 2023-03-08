using System;
using PixelSecurity.Constants;
using UnityEngine;

namespace PixelSecurity.Modules.SpeedHackProtector
{
    /// <summary>
    /// Speedhack Protector
    /// </summary>
    public class SpeedhackProtector : ISecurityModule
    {
        [System.Serializable]
        public class ModuleOptions : IModuleConfig
        {
            public float CheckInterval = 1f;
            public byte MaxFalsePositives = 3;
            public int CoolDown = 30;
        }
        private ModuleOptions _options;
        public ModuleOptions Options => _options;
        
        private const long TICKS_PER_SECOND = TimeSpan.TicksPerMillisecond * 1000;
        private const int THRESHOLD = 5000000;
        
        private readonly float _interval = 1f;
        private readonly byte _maxFalsePositives = 3;
        private readonly int _coolDown = 30;
        
        private byte _currentFalsePositives;
        private int _currentCooldownShots;
        private long _ticksOnStart;
        private long _vulnerableTicksOnStart;
        private long _prevTicks;
        private long _prevIntervalTicks;
        
        /// <summary>
        /// Teleport Protector Module
        /// </summary>
        /// <param name="options"></param>
        public SpeedhackProtector(ModuleOptions options = null)
        {
            if (options == null)
                _options = new ModuleOptions();
            
            _interval = _options.CheckInterval;
            _maxFalsePositives = _options.MaxFalsePositives;
            _coolDown = _options.CoolDown;

            InitProtector();
        }

        /// <summary>
        /// On Module Destoryed
        /// </summary>
        ~SpeedhackProtector()
        {
            DisposeProtector();
        }
        
        /// <summary>
        /// Initialize Protector
        /// </summary>
        private void InitProtector()
        {
            PixelGuard.Instance.OnLoopUpdate += OnUpdate;
            ResetStartTicks();
            _currentFalsePositives = 0;
            _currentCooldownShots = 0;
        }
        
        /// <summary>
        /// Reset Start Ticks
        /// </summary>
        private void ResetStartTicks()
        {
            _ticksOnStart = DateTime.UtcNow.Ticks;
            _vulnerableTicksOnStart = System.Environment.TickCount * TimeSpan.TicksPerMillisecond;
            _prevTicks = _ticksOnStart;
            _prevIntervalTicks = _ticksOnStart;
        }

        /// <summary>
        /// Dispose Protector
        /// </summary>
        private void DisposeProtector()
        {
            PixelGuard.Instance.OnLoopUpdate -= OnUpdate;
        }

        /// <summary>
        /// Pause / Resume Detection Check
        /// </summary>
        /// <param name="isPaused"></param>
        public void PauseSeek(bool isPaused)
        {
            if(!isPaused)
                ResetStartTicks();
        }

        /// <summary>
        /// On Game Loop Update
        /// </summary>
        /// <param name="deltaTime"></param>
        private void OnUpdate(float deltaTime)
        {
            long ticks = DateTime.UtcNow.Ticks;
            long ticksSpentSinceLastUpdate = ticks - _prevTicks;
            if (ticksSpentSinceLastUpdate < 0 || ticksSpentSinceLastUpdate > TICKS_PER_SECOND)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("SpeedHack Protector: System DateTime change or > 1 second game freeze detected!");
                ResetStartTicks();
                return;
            }
            _prevTicks = ticks;
            
            long intervalTicks = (long)(_interval * TICKS_PER_SECOND);
            
            if (ticks - _prevIntervalTicks >= intervalTicks)
            {
                long vulnerableTicks = System.Environment.TickCount * TimeSpan.TicksPerMillisecond;

                if (Mathf.Abs((vulnerableTicks - _vulnerableTicksOnStart) - (ticks - _ticksOnStart)) > THRESHOLD)
                {
                    _currentFalsePositives++;
                    if (_currentFalsePositives > _maxFalsePositives)
                    {
                        if (Debug.isDebugBuild) Debug.LogWarning("SpeedHack Protector: final detection!");
                        PixelGuard.Instance.CreateSecurityWarning(TextCodes.SPEEDHACK_DETECTED, this);
                        _currentFalsePositives = 0;
                        _currentCooldownShots = 0;
                        ResetStartTicks();
                    }
                    else
                    {
                        if (Debug.isDebugBuild) Debug.LogWarning("SpeedHack Protector: detection! Allowed false positives left: " + (_maxFalsePositives - _currentFalsePositives));
                        _currentCooldownShots = 0;
                        ResetStartTicks();
                    }
                }
                else if (_currentFalsePositives > 0 && _coolDown > 0)
                {
                    if (Debug.isDebugBuild) Debug.LogWarning("SpeedHack Protector: success shot! Shots till Cooldown: " + (_coolDown - _currentCooldownShots));
                    _currentCooldownShots++;
                    if (_currentCooldownShots >= _coolDown)
                    {
                        if (Debug.isDebugBuild) Debug.LogWarning("SpeedHack Protector: Cooldown!");
                        _currentFalsePositives = 0;
                    }
                }

                _prevIntervalTicks = ticks;
            }
        }
    }
}