using UnityEngine;

namespace MomodoraCopy
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        public static T instance;

        public bool isPersistant = true;

        public virtual void Awake()
        {
            if (!isPersistant)
            {
                instance = this as T;
                return;
            }
            if (instance == null)
            {
                DontDestroyOnLoad(gameObject);
                instance = this as T;
                return;
            }
            Destroy(gameObject);
        }
    }
}