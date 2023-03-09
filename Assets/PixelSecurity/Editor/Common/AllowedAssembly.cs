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

using System;

namespace PixelSecurity.Editor.Common
{
    /// <summary>
    /// Allowed Assembly
    /// </summary>
    internal class AllowedAssembly
    {
        public string name;
        public int[] hashes;

        public AllowedAssembly(string name, int[] hashes)
        {
            this.name = name;
            this.hashes = hashes;
        }

        public bool AddHash(int hash)
        {
            if (Array.IndexOf(hashes, hash) != -1) return false;

            int oldLen = hashes.Length;
            int newLen = oldLen + 1;

            int[] newHashesArray = new int[newLen];
            Array.Copy(hashes, newHashesArray, oldLen);

            hashes = newHashesArray;
            hashes[oldLen] = hash;

            return true;
        }

        public override string ToString()
        {
            return name + " (hashes: " + hashes.Length + ")";
        }
    }
}