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
namespace PixelSecurity.Modules.SecuredMemory
{
    /// <summary>
    /// Secured Memory Module
    /// </summary>
    public class SecuredMemory : ISecurityModule
    {
        [System.Serializable]
        public class ModuleOptions : IModuleConfig
        {
            
        }
        private ModuleOptions _options;
        
        /// <summary>
        /// Secured Memory Module
        /// </summary>
        /// <param name="options"></param>
        public SecuredMemory(ModuleOptions options = null)
        {
            
        }
    }
}