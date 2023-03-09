using PixelSecurity.Modules;

namespace PixelSecurity.Handlers
{
    /// <summary>
    /// Security Warning Handler
    /// </summary>
    [System.Serializable]
    public class SecurityWarningHandler
    {
        public string message;
        public ISecurityModule module;
    }
}