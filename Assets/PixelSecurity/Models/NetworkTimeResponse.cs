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
namespace PixelSecurity.Models
{
    /// <summary>
    /// Network Time Response
    /// </summary>
    [System.Serializable]
    public class NetworkTimeResponse
    {
        public string abbreviation = "";
        public string client_ip = "";
        public string datetime = "";
        public int day_of_week = 0;
        public int day_of_year = 0;
        public bool dst = false;
        public int raw_offset = 0;
        public int unixtime = 0;
    }
}