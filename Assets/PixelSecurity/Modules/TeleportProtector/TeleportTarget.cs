using PixelSecurity.Core.SecuredTypes;
using UnityEngine;

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
namespace PixelSecurity.Modules.TeleportProtector
{
    /// <summary>
    /// Teleport Protector Target
    /// </summary>
    [System.Serializable]
    public class TeleportTarget
    {
        public SecuredFloat MaxDistancePerSecond = 3f;
        public Transform TargetTransform;
        public Vector3 LastPosition;
    }
}