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
using PixelSecurity.Modules;
using UnityEngine;
using UnityEngine.UI;

namespace PixelSecurity.UI.CheatDetector
{
    /// <summary>
    /// Cheat Detection UI
    /// </summary>
    internal class CheatDetectedView : BaseView
    {
        // UI Context
        public struct Context : IContext
        {
            public Action OnWindowClosed;
        }
        private Context _context;

        [Header("UI References")] 
        [SerializeField] private Text _headline;
        [SerializeField] private Text _message;
        [SerializeField] private Button _acceptButton;
        [SerializeField] private Text _buttonText;

        /// <summary>
        /// Set UI Context
        /// </summary>
        /// <param name="context"></param>
        public void SetContext(Context context)
        {
            _context = context;
            PixelGuard.Instance.OnSecurityMessage += OnCheatingDetected;
            Hide();
        }

        /// <summary>
        /// On UI Destroyed
        /// </summary>
        private void OnDestroy()
        {
            PixelGuard.Instance.OnSecurityMessage -= OnCheatingDetected;
        }

        /// <summary>
        /// Cheating Detected
        /// </summary>
        /// <param name="message"></param>
        /// <param name="module"></param>
        private void OnCheatingDetected(string message, ISecurityModule module)
        {
            UpdateView(TextCodes.CHEATING_DETECTED, message, ()=>{}, TextCodes.ACCEPT);
        }

        /// <summary>
        /// Show Dialogue
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="onAccepted"></param>
        /// <param name="buttonText"></param>
        public void ShowDialogue(string title, string message, Action onAccepted = null, string buttonText = TextCodes.ACCEPT)
        {
            UpdateView(title, message, onAccepted, buttonText);
        }
        
        /// <summary>
        /// Show Dialogue
        /// </summary>
        /// <param name="message"></param>
        /// <param name="onAccepted"></param>
        /// <param name="buttonText"></param>
        public void ShowDialogue(string message, Action onAccepted = null, string buttonText = TextCodes.ACCEPT)
        {
            UpdateView(TextCodes.CHEATING_DETECTED, message, onAccepted, buttonText);
        }

        /// <summary>
        /// Update View
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="onAccepted"></param>
        /// <param name="buttonText"></param>
        private void UpdateView(string title, string message, Action onAccepted = null, string buttonText = TextCodes.ACCEPT)
        {
            // Setup Texts
            _headline.text = title;
            _message.text = message;
            _buttonText.text = buttonText;
            
            // Add Listeners
            _acceptButton.onClick.RemoveAllListeners();
            _acceptButton.onClick.AddListener(() =>
            {
                Hide();
                onAccepted?.Invoke();
                _context.OnWindowClosed?.Invoke();
            });
            Show();
        }
    }
}