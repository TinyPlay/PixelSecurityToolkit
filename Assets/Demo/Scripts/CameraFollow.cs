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
namespace Demo.Scripts
{
    using UnityEngine;

    /// <summary>
    /// Simple Example Camera Follow
    /// </summary>
    internal class CameraFollow : MonoBehaviour
    {
        public Transform target;

        public bool isCustomOffset;
        public Vector3 offset;

        public float smoothSpeed = 0.1f;

        private void Start()
        {
            // You can also specify your own offset from inspector
            // by making isCustomOffset bool to true
            if (!isCustomOffset)
            {
                offset = transform.position - target.position;
            }
        }

        private void LateUpdate()
        {
            SmoothFollow();   
        }

        public void SmoothFollow()
        {
            Vector3 targetPos = target.position + offset;
            Vector3 smoothFollow = Vector3.Lerp(transform.position,
                targetPos, smoothSpeed);

            transform.position = smoothFollow;
            transform.LookAt(target);
        }
    }
}