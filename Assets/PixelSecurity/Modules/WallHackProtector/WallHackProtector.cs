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

using PixelSecurity.Constants;
using PixelSecurity.Handlers;
using UnityEngine;

namespace PixelSecurity.Modules.WallHackProtector
{
    /// <summary>
    /// Wallhack Protector Module
    /// </summary>
    public class WallHackProtector : ISecurityModule
    {
        [System.Serializable]
        public class ModuleOptions : IModuleConfig
        {
            // Memory Parameters
            public Vector3 ModuleSpawn = new Vector3(0,0,0);
        }
        private ModuleOptions _options;
        public ModuleOptions Options => _options;
        
        // Wallhack Service
        private const string SERVICE_CONTAINER_NAME = "__WALLHACK_SERVICE__";
        private readonly Vector3 _rigidPlayerVelocity = new Vector3(0, 0, 1f);
        
        // Spawn Position
        private readonly Vector3 _spawnPosition;
        
        private int _whLayer = -1;
        private GameObject _serviceContainer;
        private Rigidbody _rigidPlayer;
        private CharacterController _charControllerPlayer;
        private float _charControllerVelocity = 0;

        private bool _invoked = false;
        private float _invokeTimer = 4f;
        
        #if DEBUG
        private bool _rigidDetected = false;
        private bool _controllerDetected = false;
        #endif

        /// <summary>
        /// Wallhack Module
        /// </summary>
        /// <param name="options"></param>
        public WallHackProtector(ModuleOptions options = null)
        {
            if (options == null)
                _options = new ModuleOptions();
            else
                _options = options;

            _spawnPosition = _options.ModuleSpawn;
            InitDetector();
        }

        /// <summary>
        /// On Module Destoryed
        /// </summary>
        ~WallHackProtector()
        {
            DisposeDetector();
        }
        
        /// <summary>
        /// Initialize Detector
        /// </summary>
        private void InitDetector()
        {
            InitCommon();
            InitRigidModule();
            InitControllerModule();

            StartRigidModule();
            StartControllerModule();

            PixelGuard.Instance.OnLoopUpdate.AddListener(OnUpdate);
            PixelGuard.Instance.OnLoopFixedUpdate.AddListener(OnFixedUpdate);
        }
        
        /// <summary>
        /// Dispose Detector
        /// </summary>
        private void DisposeDetector()
        {
            PixelGuard.Instance.OnLoopUpdate.RemoveListener(OnUpdate);
            PixelGuard.Instance.OnLoopFixedUpdate.RemoveListener(OnFixedUpdate);
            
            StopRigidModule();
            StopControllerModule();
            GameObject.Destroy(_serviceContainer);
        }

        /// <summary>
        /// On Game Loop Update
        /// </summary>
        /// <param name="handler"></param>
        private void OnUpdate(DeltaTimeHandler handler)
        {
            if(!_invoked) InvokeDetector(handler.DeltaTime);
            if (_charControllerVelocity > 0)
            {
                _charControllerPlayer.Move(new Vector3(Random.Range(-0.002f, 0.002f), 0, _charControllerVelocity));

                if (_charControllerPlayer.transform.localPosition.z > 1f)
                {
                    #if DEBUG
                    _controllerDetected = true;
                    #endif
                    StopControllerModule();

                    Detect();
                }
            }
        }

        /// <summary>
        /// On GameLoop Fixed Update
        /// </summary>
        /// <param name="handler"></param>
        private void OnFixedUpdate(DeltaTimeHandler handler)
        {
            if (_rigidPlayer.transform.localPosition.z > 1f)
            {
                #if DEBUG
                _rigidDetected = true;
                #endif
                StopRigidModule();

                Detect();
            }
        }

        /// <summary>
        /// Invoke Detector
        /// </summary>
        /// <param name="deltaTime"></param>
        private void InvokeDetector(float deltaTime)
        {
            if (_invokeTimer <= 0f)
            {
                _invokeTimer = 4f;
                StartRigidModule();
                StartControllerModule();
                _invoked = true;
            }
            else
            {
                _invokeTimer -= deltaTime;
            }
        }
        
        /// <summary>
        /// Initialize General
        /// </summary>
        private void InitCommon()
        {
            if (_whLayer == -1) _whLayer = LayerMask.NameToLayer("Ignore Raycast");

            _serviceContainer = new GameObject(SERVICE_CONTAINER_NAME);
            _serviceContainer.layer = _whLayer;
            _serviceContainer.transform.position = _spawnPosition;
            GameObject.DontDestroyOnLoad(_serviceContainer);

            GameObject wall = new GameObject("Wall");
            wall.AddComponent<BoxCollider>();
            wall.layer = _whLayer;
            wall.transform.parent = _serviceContainer.transform;
            wall.transform.localPosition = Vector3.zero;

            wall.transform.localScale = new Vector3(3, 3, 0.5f);
        }
        
        /// <summary>
        /// Initialize RigidBody
        /// </summary>
        private void InitRigidModule()
        {
            GameObject player = new GameObject("RigidPlayer");
            player.AddComponent<CapsuleCollider>().height = 2;
            player.layer = _whLayer;
            player.transform.parent = _serviceContainer.transform;
            player.transform.localPosition = new Vector3(0.75f, 0, -1f);
            _rigidPlayer = player.AddComponent<Rigidbody>();
            _rigidPlayer.useGravity = false;
        }
        
        /// <summary>
        /// Initialize Controller Module
        /// </summary>
        private void InitControllerModule()
        {
            GameObject player = new GameObject("ControlledPlayer");
            player.AddComponent<CapsuleCollider>().height = 2;
            player.layer = _whLayer;
            player.transform.parent = _serviceContainer.transform;
            player.transform.localPosition = new Vector3(-0.75f, 0, -1f);
            _charControllerPlayer = player.AddComponent<CharacterController>();
        }
        
        /// <summary>
        /// Start RigidBody Module
        /// </summary>
        private void StartRigidModule()
        {
            _rigidPlayer.rotation = Quaternion.identity;
            _rigidPlayer.angularVelocity = Vector3.zero;
            _rigidPlayer.transform.localPosition = new Vector3(0.75f, 0, -1f);
            _rigidPlayer.velocity = _rigidPlayerVelocity;
        }
        
        /// <summary>
        /// Stop Rigid Module
        /// </summary>
        private void StopRigidModule()
        {
            _rigidPlayer.velocity = Vector3.zero;
        }
        
        /// <summary>
        /// Start Controller Module
        /// </summary>
        private void StartControllerModule()
        {
            _charControllerPlayer.transform.localPosition = new Vector3(-0.75f, 0, -1f);
            _charControllerVelocity = 0.01f;
        }
        
        /// <summary>
        /// Stop Controller Module
        /// </summary>
        private void StopControllerModule()
        {
            _charControllerVelocity = 0;
        }
        
        
        /// <summary>
        /// Detect Wallhack
        /// </summary>
        private void Detect()
        {
            PixelGuard.Instance.CreateSecurityWarning(TextCodes.WALLHACK_DETECTED, this);
        }
        
        /// <summary>
        /// Draw Gismos
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_spawnPosition, new Vector3(3, 3, 3));
        }

        #if DEBUG
        /// <summary>
        /// Debug GUI Labels
        /// </summary>
        private void OnGUI()
        {
            GUILayout.Label("RigidBody detected: " + _rigidDetected);
            GUILayout.Label("Controller detected: " + _controllerDetected);
        }
        #endif
    }
}