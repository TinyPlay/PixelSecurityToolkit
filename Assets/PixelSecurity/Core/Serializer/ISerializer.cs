﻿/*
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
namespace PixelSecurity.Core.Serializer
{
    /// <summary>
    /// Base Serializer Interface
    /// </summary>
    public interface ISerializer<TObject> where TObject : class
    {
        void SaveObject(TObject objectToSave);
        TObject LoadObject(TObject objectToLoad = null);
    }
}