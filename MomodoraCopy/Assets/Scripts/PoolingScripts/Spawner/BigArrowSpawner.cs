using UnityEngine;

namespace MomodoraCopy
{
    public class BigArrowSpawner : BaseSpawner
    {
        public GameObject arrowPrefab;
        Quaternion arrowRotation;
        public const float ACTIVATE_TIME = 1.5f;
        [HideInInspector]
        public PoolingObjectInfo info;

        void Start()
        {
            arrowRotation = Quaternion.Euler(0, 0, 90);
            info = SetPoolingObjectInfo(arrowPrefab, gameObject, gameObject.transform.position, arrowRotation);
            CreatePoolingObjectQueue(info, 1);
            SetAutoSpawn(info, ACTIVATE_TIME);
        }
    }

}