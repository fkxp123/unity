using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class PoisonBomb : MonoBehaviour
    {
        public Vector3 target;
        public float bombRadius = 0.4f;
        public ParticleSystem poisonBombEffect;
        public float speed = 3.0f;
        Transform sprite;

        Vector3 start;
        float amount = 0;
        float direction;
        void Start()
        {
            start = transform.position;
            target = GameManager.instance.playerPhysics.transform.position + Vector3.down * 1.27f;
            sprite = transform.GetChild(0);
            direction = Mathf.Sign(start.x - target.x);
            if(direction < 0)
            {
                sprite.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
        void Update()
        {
            sprite.Rotate(0, 0, 10);
            amount += speed * Time.deltaTime;
            amount = Mathf.Clamp01(amount);

            transform.position = DrawParabolaCurve(start, target, Mathf.Abs(start.x - target.x) * 0.5f, amount);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, bombRadius);
            foreach (Collider2D collider in colliders)
            {
                if(collider.tag == "Platform" || collider.tag == "Player")
                {
                    poisonBombEffect.transform.position = transform.position;
                    poisonBombEffect.Play();
                    gameObject.SetActive(false);
                }
            }
        }


        Vector3 DrawParabolaCurve(Vector3 start, Vector3 target, float height, float value)
        {
            Vector3 p1 = new Vector3(start.x, start.y);
            Vector3 p2 = new Vector3(start.x, start.y + height);
            Vector3 p3 = new Vector3(target.x, target.y + height);
            Vector3 p4 = new Vector3(target.x, target.y);

            Vector3 a = Vector3.Lerp(p1, p2, value);
            Vector3 b = Vector3.Lerp(p2, p3, value);
            Vector3 c = Vector3.Lerp(p3, p4, value);

            Vector3 d = Vector3.Lerp(a, b, value);
            Vector3 e = Vector3.Lerp(b, c, value);

            Vector3 f = Vector3.Lerp(d, e, value);

            return f;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, bombRadius);
        }
    }

}