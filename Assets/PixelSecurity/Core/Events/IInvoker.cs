using System;
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
    /// Base Invoker Interface
    /// </summary>
    public interface IInvoker
    {
        void AddListener(Action listener);
        void RemoveListener(Action listener);
    }
    
    /// <summary>
    /// Base Invoker Interface with Arguments
    /// </summary>
    /// <typeparam name="TArgs"></typeparam>
    public interface IInvoker<TArgs>
    {
        void AddListener(Action<TArgs> listener);
        void RemoveListener(Action<TArgs> listener);
    }
}