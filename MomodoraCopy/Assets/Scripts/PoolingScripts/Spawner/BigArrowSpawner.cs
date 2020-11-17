using UnityEngine;

namespace MomodoraCopy
{
    public class BigArrowSpawner : BaseSpawner
    {
        public GameObject arrowPrefab;
        float arrowRotateY;
        public const float ACTIVATE_TIME = 1.5f;
        [HideInInspector]
        public PoolingObjectInfo info;

        void Start()
        {
            arrowRotateY = 90.0f;
            info = SetPoolingObjectInfo(arrowPrefab, gameObject, arrowRotateY);
            CreatePoolingObjectQueue(info, 1);
            SetAutoSpawn(info, ACTIVATE_TIME);
        }
    }

}