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
            // Memory Parameters
            public float ColorEpsilon = 0.1f;
            public float FloatEpsilon = 0.0001f;
            public float Vector2Epsilon = 0.1f;
            public float Vector3Epsilon = 0.1f;
            public float Vector4Epsilon = 0.1f;
            public float QuaternionEpsilon = 0.1f;
            public byte Color32Epsilon = 1;
        }
        private ModuleOptions _options;
        public ModuleOptions Options => _options;

        /// <summary>
        /// Secured Memory Module
        /// </summary>
        /// <param name="options"></param>
        public SecuredMemory(ModuleOptions options = null)
        {
            if (options == null)
                _options = new ModuleOptions();
            
        }

        /// <summary>
        /// On Module Destoryed
        /// </summary>
        ~SecuredMemory()
        {
            
        }
    }
}