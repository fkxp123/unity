using UnityEngine;

namespace MomodoraCopy
{
    public class Arrow : MonoBehaviour
    {
        public float arrowSpeed = 40;
        [SerializeField]
        public float arrowDamage = 10.0f;
        public GameObject arrowHitArea;
        public float areaRadius;
        bool isStuckInWall;
        bool isHit;

        ArrowSpawner arrowSpawner;

        void Start()
        {
            arrowSpawner = transform.parent.GetComponent<ArrowSpawner>();
        }
        //void Start()
        //{
        //    InvokeRepeating("HitCheck", 0, 0.01f);
        //}
        void OnEnable()
        {
            transform.tag = "Projectile";
            gameObject.layer = LayerMask.NameToLayer("Default");
            isStuckInWall = false;
        }
        //void OnDisable()
        //{
        //    transform.tag = "Projectile";
        //    gameObject.layer = LayerMask.NameToLayer("Default");
        //    isStuckInWall = false;
        //}
        //void HitCheck()
        //{
        //    if (isStuckInWall)
        //    {
        //        return;
        //    }
        //    if (!isHit)
        //    {
        //        Collider2D[] colliders = Physics2D.OverlapCircleAll(arrowHitArea.transform.position, areaRadius);
        //        foreach (Collider2D collider in colliders)
        //        {
        //            if (collider.tag == "Platform" && collider.tag != "Through")
        //            {
        //                //gameObject.SetActive(false);
        //                isStuckInWall = true;
        //                transform.tag = "Through";
        //                gameObject.layer = LayerMask.NameToLayer("Platform");
        //            }
        //            else if (collider.tag == "Enemy")
        //            {
        //                gameObject.SetActive(false);
        //                collider.transform.GetComponent<EnemyStatus>().TakeDamage(arrowDamage, DamageType.Range, transform.rotation);
        //                isHit = true;
        //            }
        //        }
        //    }
        //    transform.Translate(Vector2.right * arrowSpeed * 0.01f);
        //}
        void Update()
        {
            if (isStuckInWall)
            {
                return;
            }
            Collider2D[] colliders = Physics2D.OverlapCircleAll(arrowHitArea.transform.position, areaRadius);
            foreach (Collider2D collider in colliders)
            {
                if (collider.tag == "Platform" || collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
                {
                    transform.SetParent(collider.transform);
                    isStuckInWall = true;
                    transform.tag = "Through";
                    gameObject.layer = LayerMask.NameToLayer("ArrowPlatform");
                }
                else if (collider.tag == "Enemy")
                {
                    ObjectPooler.instance.RecyclePoolingObject(arrowSpawner.info, gameObject);
                    collider.transform.GetChild(0).GetComponent<EnemyStatus>().TakeDamage(arrowDamage, DamageType.Range, transform.rotation);
                }
            }
            transform.Translate(Vector2.right * arrowSpeed * Time.deltaTime);
        }
        //void OnCollisionEnter2D(Collision2D other)
        //{
        //    if (other.transform.tag == "platform")
        //    {
        //        gameObject.SetActive(false);
        //    }
        //    if (other.transform.tag == "enemy")
        //    {
        //        gameObject.SetActive(false);
        //        other.transform.GetComponent<EnemyStatus>().TakeDamage(arrowDamage, DamageType.Range, transform.rotation);
        //    }
        //}
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(arrowHitArea.transform.position, areaRadius);
        }
    }
}