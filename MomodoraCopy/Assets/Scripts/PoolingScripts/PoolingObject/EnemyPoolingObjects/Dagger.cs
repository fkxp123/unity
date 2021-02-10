using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class Dagger : MonoBehaviour
    {
        public float arrowSpeed = 40;

        public GameObject daggerHitArea;
        public float areaRadius;
        public float daggerDamage = 10;

        DaggerSpawner daggerSpawner;

        void Start()
        {
            daggerSpawner = transform.parent.GetComponent<DaggerSpawner>();
        }

        void Update()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(daggerHitArea.transform.position, areaRadius);
            foreach (Collider2D collider in colliders)
            {
                if (collider.tag == "Platform" || collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
                {
                    ObjectPooler.instance.RecyclePoolingObject(daggerSpawner.info, gameObject);
                }
                else if (collider.tag == "Player")
                {
                    ObjectPooler.instance.RecyclePoolingObject(daggerSpawner.info, gameObject);
                    collider.transform.GetComponent<PlayerStatus>().TakeDamage(daggerDamage, DamageType.Range, transform.rotation);
                }
            }
            transform.Translate(Vector2.right * arrowSpeed * Time.deltaTime);
        }
    }
}