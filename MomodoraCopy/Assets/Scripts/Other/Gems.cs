using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class Gems : MonoBehaviour
    {
        public int gemValue;

        void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.tag == "Player")
            {
                GameManager.instance.SetTotalScore(gemValue);
                gameObject.SetActive(false);
            }
        }
    }

}