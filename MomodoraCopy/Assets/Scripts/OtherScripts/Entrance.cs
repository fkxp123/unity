using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class Entrance : MonoBehaviour
    {
        void Start()
        {
            GameManager.instance.playerPhysics.transform.position = transform.position;
        }
    }

}