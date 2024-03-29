﻿using UnityEngine;

namespace MomodoraCopy
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        public static T instance;

        //public bool isPersistent = true;
        public bool getNewInstance;

        public virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                if (getNewInstance)
                {
                    Destroy(instance.gameObject);
                    instance = this as T;
                    DontDestroyOnLoad(gameObject);
                    return;
                }
                Destroy(gameObject);
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