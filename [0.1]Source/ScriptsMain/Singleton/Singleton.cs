using UnityEngine;

namespace Utility.Singleton
{
    public class SingletonDefault<T> where T : new()
    {
        private static bool _isApplicationQuitting;
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_isApplicationQuitting)
                    return default;

                if (_instance == null)
                {
                    _instance = new T();
                    Debug.Log($"[SINGLETON] {typeof(T)}");
                }

                return _instance;
            }
        }

        public virtual void OnDestroy()
        {
            _isApplicationQuitting = true;
        }
    }

    // **********************************************************************

    public class SingletonForMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool _isApplicationQuitting;
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_isApplicationQuitting)
                    return null;

                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        var singleton = new GameObject("[SINGLETON] " + typeof(T));
                        _instance = singleton.AddComponent<T>();
                        DontDestroyOnLoad(singleton);
                    }
                }

                return _instance;
            }
        }

        public virtual void OnDestroy()
        {
            _isApplicationQuitting = true;
        }
    }
}