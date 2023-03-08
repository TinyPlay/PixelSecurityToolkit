using System;
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
        /// On Update
        /// </summary>
        private void Update()
        {
            PixelGuard.Instance.CallGameLoop(false, Time.deltaTime);
        }

        /// <summary>
        /// On Fixed Update
        /// </summary>
        private void FixedUpdate()
        {
            PixelGuard.Instance.CallGameLoop(true, Time.fixedDeltaTime);
        }
    }
}