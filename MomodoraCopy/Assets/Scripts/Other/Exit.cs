using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MomodoraCopy
{
    public class Exit : MonoBehaviour
    {
        [SerializeField]
        float checkPlayerCycle;
        WaitForSeconds waitTime;

        public GameObject checkPlayer;
        public Vector3 checkPlayerArea;

        int nextSceneIndex;

        void Start()
        {
            nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            checkPlayerCycle = 0.1f;
            waitTime = new WaitForSeconds(checkPlayerCycle);
            StartCoroutine(CheckPlayer());
            
        }
        IEnumerator CheckPlayer()
        {
            while (true)
            {
                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(checkPlayer.transform.position, checkPlayerArea, 0);
                foreach (Collider2D collider in collider2Ds)
                {
                    if (collider.tag == "Player")
                    {
                        SceneManager.LoadScene(nextSceneIndex);
                    }
                }
                yield return waitTime;
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(checkPlayer.transform.position, checkPlayerArea);
        }
    }
}