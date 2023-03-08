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
namespace PixelSecurity.UI.PrivacyPolicy
{
    /// <summary>
    /// Privacy View
    /// </summary>
    internal class PrivacyView
    {
        // UI Context
        public struct Context : IContext { }
        private Context _context;
        
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
    }
}