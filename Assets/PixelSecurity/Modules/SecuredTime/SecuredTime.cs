using System;
using System.Collections;
using System.Reflection;
using PixelSecurity.Constants;
using PixelSecurity.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace PixelSecurity.Modules.SecuredTime
{
    /// <summary>
    /// Time Rewind Protector
    /// </summary>
    public class SecuredTime : ISecurityModule
    {
        [System.Serializable]
        public class ModuleOptions : IModuleConfig
        {
            public float CheckInterval = 10f;
            public int AvailableTolerance = 60;
            
            public bool NetworkCompare = false;
            public string TimeServerUrl = "https://worldtimeapi.org/api/timezone/Europe/London";
            public string TimeServerMethod = "GET";
        }
        private ModuleOptions _options;
        public ModuleOptions Options => _options;
        
        // Current Options
        private float _timeCheckInterval;
        private int _availableTolerance;
        private bool _networkCompare;
        private string _timeServerUrl;
        private string _timeServerMethod;

        private float _checkTimer = 0f;
        private int _lastTime = 0;
        private int _lastNetworkTime = 0;
        
        /// <summary>
        /// Teleport Protector Module
        /// </summary>
        /// <param name="options"></param>
        public SecuredTime(ModuleOptions options = null)
        {
            if (options == null)
                _options = new ModuleOptions();
            
            // Setup Configuration
            _timeCheckInterval = _options.CheckInterval;
            _availableTolerance = _options.AvailableTolerance;
            _networkCompare = _options.NetworkCompare;
            _timeServerUrl = _options.TimeServerUrl;
            _timeServerMethod = _options.TimeServerMethod;

            InitProtector();
        }
        
        /// <summary>
        /// On Module Destoryed
        /// </summary>
        ~SecuredTime()
        {
            DisposeProtector();
        }
        
        /// <summary>
        /// Initialize Protector
        /// </summary>
        private void InitProtector()
        {
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
            if (_checkTimer <= 0f)
            {
                _checkTimer = _timeCheckInterval;
                CheckTime();
            }
            else
            {
                _checkTimer -= deltaTime;
            }
        }

        /// <summary>
        /// Check Time Difference
        /// </summary>
        private void CheckTime()
        {
            if (_networkCompare)
            {
                if (_lastTime == 0)
                {
                    _lastTime = GetCurrentLocalTime();
                    GetCurrentNetworkTime(networkTime =>
                    {
                        _lastNetworkTime = networkTime;
                    });
                    return;
                }
                
                GetCurrentNetworkTime(networkTime =>
                {
                    bool isDifferenceApplied = CompareTimestamps(networkTime, GetCurrentLocalTime());
                    if(!isDifferenceApplied)
                        DetectTimeChanged();
                    
                    _lastTime = GetCurrentLocalTime();
                    _lastNetworkTime = networkTime;
                });
            }
            else
            {
                if (_lastTime == 0)
                {
                    _lastTime = GetCurrentLocalTime();
                    _lastNetworkTime = GetCurrentLocalTime();
                    return;
                }

                if ((GetCurrentLocalTime() - _lastTime) - _timeCheckInterval > _availableTolerance || ((GetCurrentLocalTime() - _lastTime) - _timeCheckInterval < 0))
                {
                    DetectTimeChanged();
                }
                
                _lastTime = GetCurrentLocalTime();
                _lastNetworkTime = GetCurrentLocalTime();
            }
        }
        
        /// <summary>
        /// Detect Time Changed
        /// </summary>
        /// <param name="target"></param>
        private void DetectTimeChanged(){
            PixelGuard.Instance.CreateSecurityWarning(TextCodes.TIMECHANGE_DETECTED, this);
        }
        
        /// <summary>
        /// Get Current Local Unix Time
        /// </summary>
        /// <returns></returns>
        public int GetCurrentLocalTime()
        {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            int currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
            return currentEpochTime;
        }

        /// <summary>
        /// Get Current Network Time
        /// </summary>
        /// <param name="onTimeDetected"></param>
        /// <returns></returns>
        public void GetCurrentNetworkTime(Action<int> onTimeDetected)
        {
            PixelGuard.Instance.InvokeCoroutine(RequestNetworkTime(onTimeDetected, (error) =>
            {
                PixelGuard.Instance.CreateSecurityWarning(error, this);
            }));
        }

        /// <summary>
        /// Request Network Time
        /// </summary>
        /// <param name="onTimeReceived"></param>
        /// <param name="onRequestError"></param>
        /// <returns></returns>
        private IEnumerator RequestNetworkTime(Action<int> onTimeReceived, Action<string> onRequestError)
        {
            UnityWebRequest webRequest = new UnityWebRequest(_timeServerUrl, _timeServerMethod);
            DownloadHandlerBuffer dH = new DownloadHandlerBuffer();
            webRequest.downloadHandler = dH;
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                NetworkTimeResponse response = JsonUtility.FromJson<NetworkTimeResponse>(webRequest.downloadHandler.text);
                if (response != null && response.unixtime > 0)
                {
                    onTimeReceived?.Invoke(response.unixtime);
                }
                else
                {
                    onRequestError?.Invoke("Failed to parse network time. Please, check network model and try again.");
                }
            }
            else
            {
                onRequestError?.Invoke(webRequest.error);
            }
            
            webRequest.Dispose();
        }
        
        /// <summary>
        /// CompareTimestamps
        /// </summary>
        private bool CompareTimestamps(int currentNetworkTime, int currentLocalTime)
        {
            int networkTimeDiff = 0;
            int localTimeDiff = 0;
            int avgTimeDiff = 0;
            
            networkTimeDiff = currentNetworkTime - _lastNetworkTime;
            localTimeDiff = currentLocalTime - _lastTime;
            avgTimeDiff = (localTimeDiff > networkTimeDiff)
                ? localTimeDiff - networkTimeDiff
                : networkTimeDiff - localTimeDiff;

            return (avgTimeDiff > _availableTolerance) ? false : true;
        }
    }
}