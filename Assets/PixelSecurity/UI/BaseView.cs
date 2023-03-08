using UnityEngine;

namespace PixelSecurity.UI
{
    /// <summary>
    /// Base View Class
    /// </summary>
    public class BaseView : MonoBehaviour, IView
    {
        private IContext _context;
        private bool _isEnabled = false;
        private Canvas _canvas = null;

        /// <summary>
        /// On Awake
        /// </summary>
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }

        /// <summary>
        /// Set Context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual IView SetContext(IContext context)
        {
            _context = context;
            return this;
        }

        /// <summary>
        /// Show UI
        /// </summary>
        public void Show()
        {
            if (_canvas != null)
                _canvas.enabled = true;
            else
                this.gameObject.SetActive(true);
        }

        /// <summary>
        /// Hide UI
        /// </summary>
        public void Hide()
        {
            if (_canvas != null)
                _canvas.enabled = false;
            else
                this.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Get Context
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public TContext GetContext<TContext>() where TContext : IContext
        {
            return (TContext) _context;
        }
        
        /// <summary>
        /// Set as Global View
        /// </summary>
        /// <returns></returns>
        public IView SetAsGlobalView()
        {
            DontDestroyOnLoad(this);
            return this;
        }
        
        /// <summary>
        /// Set Position
        /// </summary>
        /// <param name="position"></param>
        private void SetPosition(Vector3 position)
        {
            transform.position = position;
        }
        
        /// <summary>
        /// Set Rotation
        /// </summary>
        /// <param name="rotation"></param>
        private void SetRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }
        
        /// <summary>
        /// Set Scale
        /// </summary>
        /// <param name="scale"></param>
        private void SetScale(Vector3 scale)
        {
            transform.localScale = scale;
        }
        
        /// <summary>
        /// Get View Transform
        /// </summary>
        /// <returns></returns>
        public Transform GetViewTransform()
        {
            return transform;
        }
    }
}