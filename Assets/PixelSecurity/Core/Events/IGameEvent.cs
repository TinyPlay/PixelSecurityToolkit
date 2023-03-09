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
    /// Base Game Event Interface
    /// </summary>
    public interface IGameEvent : IEvent, IListener, IInvoker { }
    
    /// <summary>
    /// Base Game Event Interface with Arguments
    /// </summary>
    /// <typeparam name="TArgs"></typeparam>
    public interface IGameEvent<TArgs> : IEvent, IListener<TArgs>, IInvoker<TArgs> { }
}