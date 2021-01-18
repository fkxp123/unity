using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class Dagger : MonoBehaviour
    {
        public float arrowSpeed = 40;

        void Update()
        {
            transform.Translate(Vector2.right * arrowSpeed * Time.deltaTime);
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.transform.tag == "enemy")
            {
                return;
            }
            gameObject.SetActive(false);
            if (other.transform.tag == "player")
            {
                //player hit event
            }
        }
    }
}