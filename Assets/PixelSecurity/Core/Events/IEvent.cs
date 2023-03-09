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
namespace PixelSecurity.Core.Events
{
    /// <summary>
    /// Base Event Interface
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Remove All Listeners
        /// </summary>
        void RemoveAllListeners();
    }
}