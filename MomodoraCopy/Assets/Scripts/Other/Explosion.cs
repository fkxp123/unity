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

        Vector3 DrawBezierCurve(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float value)
        {
            Vector3 a = Vector3.Lerp(p1, p2, value);
            Vector3 b = Vector3.Lerp(p2, p3, value);
            Vector3 c = Vector3.Lerp(p3, p4, value);

            Vector3 d = Vector3.Lerp(a, b, value);
            Vector3 e = Vector3.Lerp(b, c, value);

            Vector3 f = Vector3.Lerp(d, e, value);

            return f;
        }
        Vector3 DrawBlownUpCurve(Vector3 start, Vector3 end, float value)
        {
            Vector3 p1 = new Vector3(start.x, start.y);
            Vector3 p2 = new Vector3(start.x, start.y + end.y);
            Vector3 p3 = new Vector3(start.x + end.x, start.y + end.y);
            Vector3 p4 = new Vector3(start.x + end.x, start.y);

            Vector3 a = Vector3.Lerp(p1, p2, value);
            Vector3 b = Vector3.Lerp(p2, p3, value);
            Vector3 c = Vector3.Lerp(p3, p4, value);

            Vector3 d = Vector3.Lerp(a, b, value);
            Vector3 e = Vector3.Lerp(b, c, value);

            Vector3 f = Vector3.Lerp(d, e, value);

            return f;
        }
    }
}
