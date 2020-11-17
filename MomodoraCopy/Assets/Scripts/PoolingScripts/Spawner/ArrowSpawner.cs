using UnityEngine;

namespace MomodoraCopy
{
    public class ArrowSpawner : BaseSpawner
    {
        public GameObject arrowPrefab;
        [HideInInspector]
        public float arrowRotateZ;
        public const float ACTIVATE_TIME = 1.5f;
        [HideInInspector]
        public PoolingObjectInfo info;
        public Vector3 standingArrowSpawnerPosition = new Vector3(0, 0.65f, 0);
        public Vector3 crouchArrowSpawnerPosition = new Vector3(0, -0.1f, 0);
        public Vector3 airArrowSpawnerPosition = new Vector3(0, 0.75f, 0);

        void Start()
        {
            info = SetPoolingObjectInfo(arrowPrefab, gameObject, arrowRotateZ);
            CreatePoolingObjectQueue(info, 10);
        }

        public void SetSpawnerPosition(Vector3 originPosition, Vector3 moveAmount)
        {
            transform.position = originPosition + moveAmount;
        }

        public float SetPoolingObjectRotateZ(bool isFlip)
        {
            if (isFlip)
            {
                return 180.0f;
            }
            return 0.0f;
        }
    }

}