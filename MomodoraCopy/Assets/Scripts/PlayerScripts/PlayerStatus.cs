using System.Collections;
using UnityEngine;

namespace MomodoraCopy
{
    public class PlayerStatus : MonoBehaviour
    {
        [SerializeField]
        float maxHp = 100.0f;
        [SerializeField]
        float hp;
        public float Hp
        {
            get { return hp; }
            set
            {
                healthBar.LoseHp(hp - value);
                hp = Mathf.Clamp(value, 0, maxHp);

                if (hp <= 0)
                {
                    //animator.play(hurt), vibration, particle, invoke-setactive(false);
                    gameObject.SetActive(false);
                    //Debug.Log("CrushedDeath");
                }
            }
        }

        public float meleeAtk = 10.0f;
        public int alpha = 0;

        Animator animator;
        SpriteRenderer spriteRenderer;
        [SerializeField]

        public bool isHit;
        public bool noHitMode;

        PlayerMovement playerMovement;
        public ParticleSystem crushedDeathEffect;

        public GameObject uiCanvas;
        HealthBar healthBar;

        Player playerFsm;

        void Start()
        {
            playerFsm = GetComponent<Player>();
            animator = GetComponent<Animator>();;
            spriteRenderer = GetComponent<SpriteRenderer>();
            playerMovement = GetComponent<PlayerMovement>();
            healthBar = uiCanvas.GetComponent<HealthBar>();
            Hp = maxHp;
        }

        public void Hit(int enemyAtk)
        {
            if (!noHitMode)
            {
                if (!isHit)
                {
                    StartCoroutine("HitCoroutine", enemyAtk);
                }
            }
        }
        IEnumerator HitCoroutine(int enemyAtk)
        {
            isHit = true;
            //CurrentHp -= enemyAtk;
            //HealthBar.CurrentHp -= enemyAtk;
            //HealthBar.SetBlinkImg();
            //Debug.Log("hp : " + CurrentHp);
            animator.SetTrigger("takeDamage");
            //player.stopAllInput = true;
            //player.stopMoving_X = true;
            //boxCollider.isTrigger = true;
            //player.directionalInput = new Vector2(0, 0);
            //if (CurrentHp <= 0)
            //{
            //    Debug.Log("game over");
            //}
            //if (enemy.transform.position.x <= transform.position.x)
            //{
            //    //rigid.velocity = new Vector2(HitDistance, rigid.velocity.y);
            //}
            //else
            //{
            //    //rigid.velocity = new Vector2(-1 * HitDistance, rigid.velocity.y);
            //}
            yield return new WaitForSeconds(0.6f);
            //rigid.velocity = new Vector2(0, rigid.velocity.y);
            //player.stopMoving_X = false;
            //player.stopAllInput = false;
        }
        IEnumerator HitGracePeriod()
        {
            Debug.Log("im hit");
            for (int i = 0; i < 10; i++)
            {
                spriteRenderer.material.color = new Color(1f, 1f, 1f, .5f);
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.material.color = new Color(1f, 1f, 1f, 1f);
                yield return new WaitForSeconds(0.1f);
            }
            isHit = false;
            //boxCollider.isTrigger = false;
            StopCoroutine("HitGracePeriod");
        }

        //void Update()
        //{
        //    if (isHit)
        //    {
        //        StartCoroutine("HitGracePeriod");
        //    }
        //}

        public void TakeDamage(float damage, DamageType damageType, Quaternion damagedRotation)
        {
            Hp -= damage;
            //    playerFsm.currentState = playerFsm.hurt;

            //    enemyPhysics.rotation =
            //        Quaternion.Euler(enemyPhysics.rotation.x, damagedRotation.y == 0 ? 180 : 0, enemyPhysics.rotation.z);

            //    if (damageType == DamageType.Range)
            //    {
            //        this.RestartCoroutine(KnockBack(rangeKnockBackTime, damagedRotation), ref coroutine);
            //    }
            //    else
            //    {
            //        this.RestartCoroutine(KnockBack(meleeKnockBackTime, damagedRotation), ref coroutine);
            //    }
            //    this.RestartCoroutine(SetStateChase(), ref coroutine);
            //}
            //IEnumerator SetStateChase()
            //{
            //    yield return stateRecoveryTime;
            //    if (playerFsm.currentState != playerFsm.die)
            //    {
            //        playerFsm.currentState = playerFsm.chase;
            //    }
            //}
            //IEnumerator KnockBack(WaitForSeconds waitTime, Quaternion damagedRotation)
            //{
            //    enemyMovement.velocity.x = damagedRotation.y == 0 ? 5 : -5;
            //    enemyMovement.velocity.y = 5;
            //    hitEffectRenderer.flip = enemyPhysics.rotation.y == 0 ? new Vector3(1, 0, 0) : new Vector3(0, 0, 0);
            //    hitEffect.Play();
            //    yield return waitTime;
            //    enemyMovement.direction.x = 0;
        }

        public void CrushedDeath()
        {
            Hp -= maxHp;

            if (playerMovement.currentVelocity.x < 0)
            {
                crushedDeathEffect.transform.position = transform.position + Vector3.left * 0.5f;
                crushedDeathEffect.transform.rotation = Quaternion.Euler(0, 0, -90);
            }
            else if (playerMovement.currentVelocity.x > 0)
            {
                crushedDeathEffect.transform.position = transform.position + Vector3.right * 0.5f;
                crushedDeathEffect.transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            else if (playerMovement.currentVelocity.y > 0)
            {
                crushedDeathEffect.transform.position = transform.position + Vector3.up;
                crushedDeathEffect.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            else
            {
                crushedDeathEffect.transform.position = transform.position + Vector3.down;
                crushedDeathEffect.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            crushedDeathEffect.Play();
        }
    }

}