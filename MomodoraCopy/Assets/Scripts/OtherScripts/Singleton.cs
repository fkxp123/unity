using UnityEngine;

namespace MomodoraCopy
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        public static T instance;

        //public bool isPersistent = true;
        //public bool getNewInstance;

        public virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);

            }
            else if (instance != this)
            {
                //Instance is not the same as the one we have, destroy old one, and reset to newest one
                Destroy(instance.gameObject);
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }

            //if (!isPersistent)
            //{
            //    instance = this as T;
            //    return;
            //}
            //if (instance == null)
            //{
            //    instance = this as T;
            //    DontDestroyOnLoad(gameObject);
            //    return;
            //}
            //Destroy(gameObject);
        }
    }
}