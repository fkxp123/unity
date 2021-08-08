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
        bool isStuckInWall;
        DaggerSpawner daggerSpawner;

        void Start()
        {
            daggerSpawner = transform.parent.GetComponent<DaggerSpawner>();
        }

        void OnEnable()
        {
            transform.tag = "Projectile";
            isStuckInWall = false;
            //arrowWind.Play();
        }
        void OnDisable()
        {
            transform.tag = "Projectile";
            isStuckInWall = false;
        }

        void Update()
        {
            if (isStuckInWall)
            {
                return;
            }
            Collider2D[] colliders = Physics2D.OverlapCircleAll(daggerHitArea.transform.position, areaRadius);
            foreach (Collider2D collider in colliders)
            {
                if (collider.tag == "Platform" || collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
                {
                    transform.SetParent(collider.transform);
                    isStuckInWall = true;
                    transform.tag = "Untagged";
                }
                else if (collider.tag == "Player")
                {
                    ObjectPooler.instance.RecyclePoolingObject(daggerSpawner.info, gameObject);
                    collider.transform.GetChild(0).GetComponent<PlayerStatus>().TakeDamage(daggerDamage, DamageType.Range, transform.rotation);
                }
            }
            transform.Translate(Vector2.right * arrowSpeed * Time.deltaTime);
        }
    }
}