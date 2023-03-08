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
using PixelSecurity;
using PixelSecurity.Modules.InjectionProtector;
using PixelSecurity.Modules.SecuredMemory;
using UnityEngine;

namespace Demo.Scripts
{
    /// <summary>
    /// Example Scene Controller with integrated Security Module
    /// </summary>
    internal class ExampleController : MonoBehaviour
    {
        /// <summary>
        /// On Scene Awake
        /// </summary>
        private void Awake()
        {
            
        }

        /// <summary>
        /// On Scene Started
        /// </summary>
        private void Start()
        {
            // Setup Pixel Guard Modules
            PixelGuard.Instance.SetupModule<SecuredMemory>(new SecuredMemory(
                new SecuredMemory.ModuleOptions
                {
                    
                }));
            PixelGuard.Instance.SetupModule<InjectionProtector>(new InjectionProtector(
                new InjectionProtector.ModuleOptions
                {

                }));
            
        }
    }
}