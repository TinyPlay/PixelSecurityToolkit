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

using PixelSecurity;
using PixelSecurity.Modules.InjectionProtector;
using PixelSecurity.Modules.PrivacyAccepter;
using PixelSecurity.Modules.SecuredMemory;
using PixelSecurity.Modules.SecuredTime;
using PixelSecurity.Modules.SpeedHackProtector;
using PixelSecurity.Modules.TeleportProtector;
using PixelSecurity.Modules.TermsAccepter;
using PixelSecurity.Modules.WallHackProtector;
using UnityEngine;

namespace Demo.Scripts
{
    /// <summary>
    /// Example Scene Controller with integrated Security Module
    /// </summary>
    internal class ExampleController : MonoBehaviour
    {
        [Header("Scene Setup")] 
        [SerializeField] private GameObject _player;
        /// <summary>
        /// On Scene Started
        /// </summary>
        private void Start()
        {
            InitializeAdditionalModules();
            InitializeAdditionalModules();
        }

        /// <summary>
        /// Initialize Anti-Cheat
        /// </summary>
        private void InitializeAntiCheatModules()
        {
            // Setup Memory Protector
            SecuredMemory memoryProtector = (SecuredMemory)PixelGuard.Instance.SetupModule<SecuredMemory>(new SecuredMemory());
            
            // Setup Injection Protector
            InjectionProtector injectionProtector = (InjectionProtector)PixelGuard.Instance.SetupModule<InjectionProtector>(new InjectionProtector());
            
            // Setup Speedhack Protector
            SpeedhackProtector speedhackProtector = (SpeedhackProtector)PixelGuard.Instance.SetupModule<SpeedhackProtector>(new SpeedhackProtector());
            
            // Setup Wallhack Protector
            WallHackProtector wallHackProtector = (WallHackProtector)PixelGuard.Instance.SetupModule<WallHackProtector>(new WallHackProtector());
            
            // Setup Secured Time Protector
            SecuredTime securedTime = (SecuredTime)PixelGuard.Instance.SetupModule<SecuredTime>(new SecuredTime());
            
            // Setup Teleport Protector
            TeleportProtector teleportProtector = (TeleportProtector)PixelGuard.Instance.SetupModule<TeleportProtector>(new TeleportProtector());
            teleportProtector.AddTarget(new TeleportTarget
            {
                LastPosition = _player.transform.position,
                MaxDistancePerSecond = 20f,
                TargetTransform = _player.transform
            });
        }

        /// <summary>
        /// Initialize Additional Modules
        /// </summary>
        private void InitializeAdditionalModules()
        {
            // Privacy Policy Accepter Class
            PrivacyAccepter privacyAccepter =
                (PrivacyAccepter)PixelGuard.Instance.SetupModule<PrivacyAccepter>(new PrivacyAccepter(new PrivacyAccepter.ModuleOptions
                {
                    ShowOnce = true,
                    WindowHeadlineText = "Privacy Policy",
                    PrivacyPolicyText = "", // If null or empty - will be loaded from Resources Template
                    ReadButtonText = "Read More",
                    AcceptButtonText = "Accept",
                    PrivacyUrl = "https://example.com/privacy",
                    OnUrlClicked = () =>
                    {
                        Debug.Log("Privacy Policy URL Shown");
                    },
                    OnAccepted = () =>
                    {
                        Debug.Log("Privacy Policy has been accepted");
                    }
                }));
            
            // Terms of Service Accepter Class
            TermsAccepter termsAccepter =
                (TermsAccepter)PixelGuard.Instance.SetupModule<TermsAccepter>(new TermsAccepter(new TermsAccepter.ModuleOptions
                {
                    ShowOnce = true,
                    WindowHeadlineText = "Terms of Service",
                    TermsOfServiceText = "",    // If null or empty - will be loaded from Resources Template
                    ReadButtonText = "Read More",
                    AcceptButtonText = "Accept",
                    TermsOfServiceUrl = "https://example.com/terms_of_service",
                    OnUrlClicked = () =>
                    {
                        Debug.Log("Terms of Service URL Shown");
                    },
                    OnAccepted = () =>
                    {
                        Debug.Log("Terms of Service has been accepted");
                    }
                }));
        }
    }
}