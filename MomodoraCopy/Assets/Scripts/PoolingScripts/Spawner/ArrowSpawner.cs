using UnityEngine;

namespace MomodoraCopy
{
    public class ArrowSpawner : BaseSpawner
    {
        [HideInInspector]
        public Quaternion arrowRotation;
        public const float ACTIVATE_TIME = 3.0f;
        [HideInInspector]
        public PoolingObjectInfo info;

        void Start()
        {
            info = SetPoolingObjectInfo(prefab, gameObject, gameObject.transform.position, transform.rotation);
            CreatePoolingObjectQueue(info, 10);
        }
    }

}