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
    /// Base Listener Interface
    /// </summary>
    public interface IListener
    {
        /// <summary>
        /// Invoke Listener
        /// </summary>
        /// <param name="inverted"></param>
        void Invoke(bool inverted);
    }
    
    /// <summary>
    /// Listener Interface with Args
    /// </summary>
    /// <typeparam name="TArgs"></typeparam>
    public interface IListener<TArgs>
    {
        /// <summary>
        /// Invoke Listener
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="inverted"></param>
        void Invoke(TArgs arguments, bool inverted);
    }
}