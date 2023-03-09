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
using PixelSecurity.UI.TermsOfService;
using UnityEngine;

namespace PixelSecurity.Modules.TermsAccepter
{
    /// <summary>
    /// Terms of Usage Accept Class
    /// </summary>
    public class TermsAccepter : ISecurityModule
    {
        [System.Serializable]
        public class ModuleOptions : IModuleConfig
        {
            public bool ShowOnce = true;

            public string WindowHeadlineText = "";
            public string TermsOfServiceText = "";
            public string ReadButtonText = "";
            public string AcceptButtonText = "";
            public string TermsOfServiceUrl = "";

            public Action OnUrlClicked = () => {};
            public Action OnAccepted = () => {};
        }
        private ModuleOptions _options;
        public ModuleOptions Options => _options;
        
        private const string PrefabPath = "Prefabs/TermsWindowView";
        private const string AcceptedKey = "IsTermsAccepted";
        private TermsOfServiceView _viewInstance = null;
        
        /// <summary>
        /// Terms of Service Module
        /// </summary>
        /// <param name="options"></param>
        public TermsAccepter(ModuleOptions options = null)
        {
            if (options == null)
                _options = new ModuleOptions();
            else
                _options = options;
            
            if(_options.ShowOnce && IsAccepted())
                return;

            GameObject viewObject = GameObject.Instantiate(Resources.Load<GameObject>(PrefabPath));
            _viewInstance = viewObject.GetComponent<TermsOfServiceView>();
            _viewInstance.SetAsGlobalView();
            _viewInstance.SetContext(new TermsOfServiceView.Context
            {
                OnAccepted = () =>
                {
                    SetAsAccepted();
                    _options.OnAccepted?.Invoke();
                },
                OnUrlClicked = _options.OnUrlClicked ?? (()=>{}),
                AcceptButtonText = _options.AcceptButtonText,
                TermsText = _options.TermsOfServiceText,
                TermsUrl = _options.TermsOfServiceUrl,
                UrlButtonText = _options.ReadButtonText,
                WindowHeadlineText = _options.WindowHeadlineText
            });
            _viewInstance.UpdateView();
        }
        
        /// <summary>
        /// Is Accepted Early
        /// </summary>
        /// <returns></returns>
        private bool IsAccepted()
        {
            bool isAccepted = (PlayerPrefs.GetInt(AcceptedKey, 0) == 1);
            return isAccepted;
        }

        /// <summary>
        /// Set As Accepted
        /// </summary>
        private void SetAsAccepted()
        {
            PlayerPrefs.SetInt(AcceptedKey, 1);
        }
    }
}