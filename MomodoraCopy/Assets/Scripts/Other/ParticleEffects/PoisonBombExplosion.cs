using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class PoisonBombExplosion : MonoBehaviour
    {
        void OnParticleCollision(GameObject other)
        {
            if (other.tag == "Player")
            {
                GameManager.instance.playerPhysics.transform.GetChild(0)
                    .GetComponent<PlayerStatus>().TakeDamage(0, DamageType.Poisoned, transform.rotation);
            }
        }
    }

}