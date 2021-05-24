using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace MomodoraCopy
{
    public class SceneTransferPoint : MonoBehaviour
    {
        public string transferSceneName;

        [SerializeField]
        float checkPlayerCycle;
        WaitForSeconds waitTime;

        public Vector3 checkPlayerArea;

        void Start()
        {
            checkPlayerCycle = 0.1f;
            waitTime = new WaitForSeconds(checkPlayerCycle);
            StartCoroutine(CheckPlayer());
        }
        IEnumerator CheckPlayer()
        {
            while (true)
            {
                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(transform.position, checkPlayerArea, 0);
                foreach (Collider2D collider in collider2Ds)
                {
                    if (collider.tag == "Player")
                    {
                        SceneManager.LoadScene(transferSceneName);
                    }
                }
                yield return waitTime;
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, checkPlayerArea);
        }
    }

}