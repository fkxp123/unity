using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class DaggerSpawner : BaseSpawner
    {
        [HideInInspector]
        public Quaternion arrowRotation;
        public const float ACTIVATE_TIME = 1.5f;
        [HideInInspector]
        public PoolingObjectInfo info;

        void Start()
        {
            info = SetPoolingObjectInfo(prefab, gameObject, gameObject.transform.position, transform.rotation);
            CreatePoolingObjectQueue(info, size);
        }
    }

}
