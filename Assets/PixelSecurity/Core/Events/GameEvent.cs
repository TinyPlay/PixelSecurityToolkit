using System;
using System.Collections.Generic;

namespace PixelSecurity.Core.Events
{
    /// <summary>
    /// Base Game Event
    /// </summary>
    public class GameEvent : IGameEvent
    {
        private readonly List<Action> _listeners = new List<Action>();
        
        ~GameEvent()
        {
            RemoveAllListeners();
        }
        
        /// <summary>
        /// Add Event Listener
        /// </summary>
        /// <param name="listener"></param>
        public void AddListener(Action listener)
        {
            if(!_listeners.Contains(listener))
                _listeners.Add(listener);
        }
        
        /// <summary>
        /// Remove Event Listener
        /// </summary>
        /// <param name="listener"></param>
        public void RemoveListener(Action listener)
        {
            if (_listeners.Contains(listener))
                _listeners.Remove(listener);
        }
        
        /// <summary>
        /// Remove All Listeners
        /// </summary>
        public void RemoveAllListeners()
        {
            _listeners.Clear();
        }
        
        /// <summary>
        /// Invoke Listeners
        /// </summary>
        /// <param name="inverted"></param>
        public void Invoke(bool inverted = false)
        {
            // Cleanup Old Listeners Before
            CleanupListeners();

            if (inverted)
            {
                for (int i = _listeners.Count - 1; i >= 0; i--)
                {
                    if (_listeners[i] != null)
                        _listeners[i]?.Invoke();
                }
            }
            else
            {
                for (int i = 0; i < _listeners.Count; i++)
                {
                    if (_listeners[i] != null)
                        _listeners[i]?.Invoke();
                }
            }
        }
        
        /// <summary>
        /// Remove All Empty Listeners
        /// </summary>
        private void CleanupListeners()
        {
            _listeners.RemoveAll(item => item == null);
        }
    }

    public class GameEvent<TArgs> : IGameEvent<TArgs>
    {
        private readonly List<Action<TArgs>> _listeners = new List<Action<TArgs>>();

        ~GameEvent()
        {
            RemoveAllListeners();
        }

        /// <summary>
        /// Add Listener
        /// </summary>
        /// <param name="listener"></param>
        public void AddListener(Action<TArgs> listener)
        {
            _listeners.Add(listener);
        }

        /// <summary>
        /// Remove listener
        /// </summary>
        /// <param name="listener"></param>
        public void RemoveListener(Action<TArgs> listener)
        {
            if (_listeners.Contains(listener))
            {
                _listeners.Remove(listener);
            }
        }

        /// <summary>
        /// Remove All Listeners
        /// </summary>
        public void RemoveAllListeners()
        {
            _listeners.Clear();
        }

        /// <summary>
        /// Invoke Event
        /// </summary>
        /// <param name="arguments"></param>
        public void Invoke(TArgs arguments, bool inverted = false)
        {
            // Cleanup Null Listeners
            CleanupListeners();

            if (inverted)
            {
                for (int i = _listeners.Count - 1; i >= 0; i--)
                {
                    if (_listeners[i] != null)
                        _listeners[i]?.Invoke(arguments);
                }
            }
            else
            {
                for (int i = 0; i < _listeners.Count; i++)
                {
                    if (_listeners[i] != null)
                        _listeners[i]?.Invoke(arguments);
                }
            }
        }

        /// <summary>
        /// Cleanup All Listeners
        /// </summary>
        private void CleanupListeners()
        {
            _listeners.RemoveAll(item => item == null);
        }
    }
}