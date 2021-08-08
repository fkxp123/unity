using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class PoisonBombSpawner : BaseSpawner
    {
        public const float ACTIVATE_TIME = 5.0f;
        public PoolingObjectInfo info;
        public bool isSpecific;
        public bool isParabolla = true;
        public Queue<GameObject> poolingObjectQueue;

        void Start()
        {
            info = SetPoolingObjectInfo(prefab, gameObject, gameObject.transform.position, transform.rotation);
            if (!isSpecific)
            {
                CreatePoolingObjectQueue(info, size);
                return;
            }

            poolingObjectQueue = ObjectPooler.instance.GetPoolingObjectQueue(info, size);
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).transform.GetChild(0).GetComponent<PoisonBombController>().isParabolla = isParabolla;
            }
        }
    }

}
