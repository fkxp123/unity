using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class Explosion : MonoBehaviour
    {
        public float damage;
        public float radius;
        public float power;
        public float directionX;
        public float directionY;
        public float waitTime;
        public LayerMask target;
        WaitForSeconds waitSeconds;

        // Start is called before the first frame update
        void Start()
        {
            waitSeconds = new WaitForSeconds(waitTime);
            StartCoroutine(ExplosionCoroutin());
        }

        IEnumerator ExplosionCoroutin()
        {
            yield return waitSeconds;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, target);
            foreach (Collider2D collider in colliders)
            {
                collider.GetComponent<PlayerMovement>().velocity.x = directionX * power;
                collider.GetComponent<PlayerMovement>().velocity.y = directionY * power;
                Debug.Log("bomb!");
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position,radius);
        }
    }
}
