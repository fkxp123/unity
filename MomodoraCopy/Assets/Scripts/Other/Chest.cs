using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class Chest : MonoBehaviour
    {
        GameObject closedChest;
        GameObject openedChest;

        public GameObject diamondPrefab;
        public GameObject rubyPrefab;
        public GameObject emeraldPrefab;
        public GameObject sapphirePrefab;
        public GameObject goldBarPrefab;
        public GameObject idolPrefab;

        public int minGemsCount;
        public int maxGemsCount;

        public float minMoveAmountX;
        public float maxMoveAmountX;
        public float minMoveAmountY;
        public float maxMoveAmountY;

        bool isClosed = true;

        List<PoolingObjectInfo> gemsInfo = new List<PoolingObjectInfo>();

        List<GameObject> gemList = new List<GameObject>();

        void Start()
        {
            gemList.Add(diamondPrefab);
            gemList.Add(rubyPrefab);
            gemList.Add(emeraldPrefab);
            gemList.Add(sapphirePrefab);
            gemList.Add(goldBarPrefab);
            gemList.Add(idolPrefab);

            for(int i = 0; i < gemList.Count; i++)
            {
                PoolingObjectInfo gemInfo = new PoolingObjectInfo
                {
                    prefab = gemList[i],
                    spawner = gameObject,
                    position = transform.position,
                    objectRotation = transform.rotation
                };
                gemsInfo.Add(gemInfo);
                ObjectPooler.instance.CreatePoolingObjects(gemsInfo[i], 5);
            }

            closedChest = transform.GetChild(0).gameObject;
            openedChest = transform.GetChild(1).gameObject;

            closedChest.SetActive(true);
            openedChest.SetActive(false);
        }

        public void OpenChest()
        {
            if (!isClosed)
            {
                return;
            }
            isClosed = false;

            closedChest.SetActive(false);
            openedChest.SetActive(true);

            int numberRandom = Random.Range(minGemsCount, maxGemsCount);

            for(int i = 0; i < numberRandom + 1; i++)
            {
                int gemsRandom = Random.Range(0, 6);

                gemsInfo[gemsRandom].position = transform.position;
                GameObject gem = ObjectPooler.instance.GetStaticPoolingObject(gemsInfo[gemsRandom]);
                gem.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(minMoveAmountX, maxMoveAmountX),
                    Random.Range(minMoveAmountY, maxMoveAmountY), 0), ForceMode2D.Impulse);
            }
        }
    }

}