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
namespace PixelSecurity.Util
{
    /// <summary>
    /// Pixel Security Toolkit Mono Wrapper
    /// </summary>
    [DisallowMultipleComponent]
    internal class PixelMono : MonoBehaviour
    {
        private PixelSecurity _securityInstance = null;

        /// <summary>
        /// On Awake
        /// </summary>
        private void Awake()
        {
            
        }

        /// <summary>
        /// On Start
        /// </summary>
        private void Start()
        {
            
            // Infinite Life for this Object
            DontDestroyOnLoad(this);
        }
        
        /// <summary>
        /// Setup Pixel Mono
        /// </summary>
        /// <param name="securityInstance"></param>
        public void Setup(PixelSecurity securityInstance)
        {
            
        }
    }
}