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
namespace PixelSecurity.Modules.InjectionProtector
{
    /// <summary>
    /// Allowed Assembly Class
    /// </summary>
    public class AllowedAssembly
    {
        public readonly string Name;
        public readonly int[] Hashes;

        public AllowedAssembly(string name, int[] hashes)
        {
            this.Name = name;
            this.Hashes = hashes;
        }
    }
}