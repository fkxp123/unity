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

        void Start()
        {
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

                if(gemsRandom == 0)
                {
                    GameObject gem = Instantiate(diamondPrefab, new Vector3(transform.position.x, transform.position.y, Random.Range(-1f, 1f)), Quaternion.identity);
                    gem.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(minMoveAmountX, maxMoveAmountX), 
                        Random.Range(minMoveAmountY, maxMoveAmountY), 0), ForceMode2D.Impulse);
                }
                else if (gemsRandom == 1)
                {
                    GameObject gem = Instantiate(rubyPrefab, new Vector3(transform.position.x, transform.position.y, Random.Range(-1f, 1f)), Quaternion.identity);
                    gem.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(minMoveAmountX, maxMoveAmountX), 
                        Random.Range(minMoveAmountY, maxMoveAmountY), 0), ForceMode2D.Impulse);
                }
                else if (gemsRandom == 2)
                {
                    GameObject gem = Instantiate(emeraldPrefab, new Vector3(transform.position.x, transform.position.y, Random.Range(-1f, 1f)), Quaternion.identity);
                    gem.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(minMoveAmountX, maxMoveAmountX), 
                        Random.Range(minMoveAmountY, maxMoveAmountY), 0), ForceMode2D.Impulse);
                }
                else if (gemsRandom == 3)
                {
                    GameObject gem = Instantiate(sapphirePrefab, new Vector3(transform.position.x, transform.position.y, Random.Range(-1f, 1f)), Quaternion.identity);
                    gem.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(minMoveAmountX, maxMoveAmountX),
                        Random.Range(minMoveAmountY, maxMoveAmountY), 0), ForceMode2D.Impulse);
                }
                else if (gemsRandom == 4)
                {
                    GameObject gem = Instantiate(goldBarPrefab, new Vector3(transform.position.x, transform.position.y, Random.Range(-1f, 1f)), Quaternion.identity);
                    gem.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(minMoveAmountX, maxMoveAmountX),
                        Random.Range(minMoveAmountY, maxMoveAmountY), 0), ForceMode2D.Impulse);
                }
                else if (gemsRandom == 5)
                {
                    GameObject gem = Instantiate(idolPrefab, new Vector3(transform.position.x, transform.position.y, Random.Range(-1f, 1f)), Quaternion.identity);
                    gem.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(minMoveAmountX, maxMoveAmountX),
                        Random.Range(minMoveAmountY, maxMoveAmountY), 0), ForceMode2D.Impulse);
                }
            }
        }
    }

}