using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class VelocityCalculator : MonoBehaviour
    {
        Vector3 previous;
        Vector3 velocity;

        void Update()
        {
            velocity = (transform.position - previous) / Time.deltaTime;
            previous = transform.position;

            Debug.Log(velocity);
        }
    }

}