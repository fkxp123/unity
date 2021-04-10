using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class PoisonBreath : MonoBehaviour
    {
        void OnParticleCollision(GameObject other)
        {
            if (other.tag == "Player")
            {
                Debug.Log("player poison!!!");
            }
        }
    }
}