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
using PixelSecurity.Constants;
using UnityEngine;
using UnityEngine.UI;

namespace PixelSecurity.UI.TermsOfService
{
    /// <summary>
    /// Terms of Service View
    /// </summary>
    internal class TermsOfServiceView : BaseView
    {
        // UI Context
        public struct Context : IContext
        {
            public string WindowHeadlineText;
            public string TermsText;
            public string AcceptButtonText;
            public string UrlButtonText;
            public string TermsUrl;
            public Action OnAccepted;
            public Action OnUrlClicked;
        }
        private Context _context;
        
        [Header("UI References")] 
        [SerializeField] private Text _headline;
        [SerializeField] private Text _message;
        [SerializeField] private Button _acceptButton;
        [SerializeField] private Text _acceptButtonText;
        [SerializeField] private Button _goUrlButton;
        [SerializeField] private Text _goButtonText;
        
        /// <summary>
        /// Set UI Context
        /// </summary>
        /// <param name="context"></param>
        public void SetContext(Context context)
        {
            _context = context;
        }
        
        /// <summary>
        /// On UI Destroyed
        /// </summary>
        private void OnDestroy()
        {
            
        }
        
        /// <summary>
        /// Update View
        /// </summary>
        public void UpdateView()
        {
            // Setup Texts
            _headline.text = _context.WindowHeadlineText ?? "Privacy Policy";
            _message.text = string.IsNullOrEmpty(_context.TermsText) ? Resources.Load<TextAsset>("Templates/TermsOfUsage").text : _context.TermsText;
            _acceptButtonText.text = _context.AcceptButtonText ?? TextCodes.ACCEPT;
            _goButtonText.text = _context.UrlButtonText ?? TextCodes.READ_MORE;
            
            
            // Add Listeners
            _acceptButton.onClick.RemoveAllListeners();
            _acceptButton.onClick.AddListener(() =>
            {
                Hide();
                _context.OnAccepted?.Invoke();
            });
            _goUrlButton.onClick.RemoveAllListeners();
            _goUrlButton.onClick.AddListener(() =>
            {
                _context.OnUrlClicked?.Invoke();
                Application.OpenURL(_context.TermsUrl);
            });
            Show();
            Canvas.ForceUpdateCanvases();
        }
    }
}