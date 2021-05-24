using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class Gems : MonoBehaviour
    {
        public int gemValue;

        void OnCollisionEnter2D(Collision2D coll)
        {
            if(coll.gameObject.tag == "Player")
            {
                GameManager.instance.SetTotalScore(gemValue);
                gameObject.SetActive(false);
            }
        }
    }

}