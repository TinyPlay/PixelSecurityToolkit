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

namespace Demo.Scripts
{
    /// <summary>
    /// Simple Example Character Controller
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    internal class SimpleCharacterController : MonoBehaviour
    {
        private CharacterController _controller;
        private Vector3 _playerVelocity;
        private bool _groundedPlayer;
        private readonly float _playerSpeed = 2.0f;
        private readonly float _jumpHeight = 1.0f;
        private readonly float _gravityValue = -9.81f;
        
        private void Start()
        {
            _controller = gameObject.GetComponent<CharacterController>();
        }

        private void Update()
        {
            _groundedPlayer = _controller.isGrounded;
            if (_groundedPlayer && _playerVelocity.y < 0)
            {
                _playerVelocity.y = 0f;
            }

            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            _controller.Move(move * Time.deltaTime * _playerSpeed);

            if (move != Vector3.zero)
            {
                gameObject.transform.forward = move;
            }

            // Changes the height position of the player..
            if (Input.GetButtonDown("Jump") && _groundedPlayer)
            {
                _playerVelocity.y += Mathf.Sqrt(_jumpHeight * -3.0f * _gravityValue);
            }

            _playerVelocity.y += _gravityValue * Time.deltaTime;
            _controller.Move(_playerVelocity * Time.deltaTime);
        }
    }
}