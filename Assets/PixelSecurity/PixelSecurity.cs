using System.Collections;
using System.Collections.Generic;
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
    public class PixelSecurity
    {
        
        // Mono Wrapper
        private PixelMono _monoWrapper = null;

        /// <summary>
        /// Security Wrapper Constructor
        /// </summary>
        public PixelSecurity()
        {
            
            CreateMonoWrapper();
        }

        /// <summary>
        /// Create Mono Wrapper
        /// </summary>
        private void CreateMonoWrapper()
        {
            if(_monoWrapper != null)
                return;

            // Create Mono Wrapper
            GameObject _wrapperObject = new GameObject("__PIXEL_WRAPPER__");
            _monoWrapper = _wrapperObject.AddComponent<PixelMono>();
            _monoWrapper.Setup(this);
        }
        
    }
}
