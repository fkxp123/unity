using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class Explosion : MonoBehaviour
    {
        public float damage;
        public float radius;
        public float horizontalPower;
        public float verticalPower;
        //public float directionX;
        //public float directionY;
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
                float directionX = collider.transform.position.x < transform.position.x ? -1 : 1;
                collider.GetComponent<PlayerMovement>().TakeExplosion(new Vector3(horizontalPower * directionX, verticalPower));
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
