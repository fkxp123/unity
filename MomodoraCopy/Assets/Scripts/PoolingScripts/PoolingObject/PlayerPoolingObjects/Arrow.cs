using UnityEngine;

namespace MomodoraCopy
{
    public class Arrow : MonoBehaviour
    {
        public float arrowSpeed = 40;
        [SerializeField]
        float arrowDamage = 10.0f;
        [SerializeField]
        GameObject arrowHitArea;
        [SerializeField]
        float areaRadius;
        bool isStuckInWall;

        void OnDisable()
        {
            transform.tag = "Projectile";
            gameObject.layer = LayerMask.NameToLayer("Default");
            isStuckInWall = false;
        }
        void Update()
        {
            if (isStuckInWall)
            {
                return;
            }
            Collider2D[] colliders = Physics2D.OverlapCircleAll(arrowHitArea.transform.position, areaRadius);
            foreach(Collider2D collider in colliders)
            {
                if(collider.tag == "Platform" && collider.tag != "Through")
                {
                    //gameObject.SetActive(false);
                    isStuckInWall = true;
                    transform.tag = "Through";
                    gameObject.layer = LayerMask.NameToLayer("Platform");
                }
                else if(collider.tag == "Enemy")
                {
                    gameObject.SetActive(false);
                    collider.transform.GetComponent<EnemyStatus>().TakeDamage(arrowDamage, DamageType.Range, transform.rotation);
                }
            }
            transform.Translate(Vector2.right * arrowSpeed * Time.deltaTime);
            if (!isStuckInWall)
            {
            }
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