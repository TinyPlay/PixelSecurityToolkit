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

using UnityEngine;

namespace PixelSecurity.UI
{
    /// <summary>
    /// Base UI Interface
    /// </summary>
    public interface IView
    {
        IView SetContext(IContext context);
        TContext GetContext<TContext>() where TContext : IContext;
        IView SetAsGlobalView();
        Transform GetViewTransform();
    }
}