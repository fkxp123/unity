using System.Collections;
using UnityEngine;

namespace MomodoraCopy
{
    public class PlayerStatus : MonoBehaviour
    {
        public float maxHp = 100.0f;
        [SerializeField]
        float hp;
        public float Hp
        {
            get { return hp; }
            set
            {
                if (healthBar != null)
                {
                    healthBar.LoseHp(hp - value);
                }
                hp = Mathf.Clamp(value, 0, maxHp);

                if (hp <= 0)
                {
                    //animator.play(hurt), vibration, particle, invoke-setactive(false);
                    transform.parent.gameObject.SetActive(false);
                    //Debug.Log("CrushedDeath");
                }
            }
        }

        public float meleeAtk = 10.0f;
        public int alpha = 0;

        //Animator animator;
        SpriteRenderer spriteRenderer;
        [SerializeField]

        public bool isHit;
        public bool noHitMode;

        PlayerMovement playerMovement;
        public ParticleSystem crushedDeathEffect;

        public GameObject uiCanvas;
        HealthBar healthBar;

        Player playerFsm;

        Transform playerPhysics;
        public float hurtTime = 0.5f;
        Coroutine coroutine;
        WaitForSeconds meleeKnockBackTime;
        WaitForSeconds rangeKnockBackTime;

        float coroutineCycle = 0.1f;
        WaitForSeconds waitTime;

        float poisonedCycle = 1.0f;
        WaitForSeconds poisonWaitTime;

        public ParticleSystem poisonedEffect;

        void Awake()
        {
            Hp = maxHp;
        }
        void Start()
        {
            playerFsm = GetComponent<Player>();
            //animator = GetComponent<Animator>();;
            spriteRenderer = GetComponent<SpriteRenderer>();
            uiCanvas.transform.SetParent(transform);
            healthBar = uiCanvas.GetComponent<HealthBar>();

            playerPhysics = transform.parent;
            playerMovement = playerPhysics.GetComponent<PlayerMovement>();

            meleeKnockBackTime = new WaitForSeconds(hurtTime);
            rangeKnockBackTime = new WaitForSeconds(hurtTime);

            waitTime = new WaitForSeconds(coroutineCycle);
            poisonWaitTime = new WaitForSeconds(poisonedCycle);
        }

        IEnumerator Poisoned()
        {
            float poisonedTime = 10f;
            poisonedEffect.Play();
            float totalDamage = 0;
            healthBar.PoisonedHealth();
            while(poisonedTime > 0)
            {
                poisonedTime -= poisonedCycle;
                yield return poisonWaitTime;
                Hp -= 2f;
                totalDamage += 2f;
            }
            poisonedEffect.Stop();
            healthBar.NormalHealth();
        }

        //public void Hit(int enemyAtk)
        //{
        //    if (!noHitMode)
        //    {
        //        if (!isHit)
        //        {
        //            StartCoroutine("HitCoroutine", enemyAtk);
        //        }
        //    }
        //}
        //IEnumerator HitCoroutine(int enemyAtk)
        //{
        //    isHit = true;
        //    //CurrentHp -= enemyAtk;
        //    //HealthBar.CurrentHp -= enemyAtk;
        //    //HealthBar.SetBlinkImg();
        //    //Debug.Log("hp : " + CurrentHp);
        //    animator.SetTrigger("takeDamage");
        //    //player.stopAllInput = true;
        //    //player.stopMoving_X = true;
        //    //boxCollider.isTrigger = true;
        //    //player.directionalInput = new Vector2(0, 0);
        //    //if (CurrentHp <= 0)
        //    //{
        //    //    Debug.Log("game over");
        //    //}
        //    //if (enemy.transform.position.x <= transform.position.x)
        //    //{
        //    //    //rigid.velocity = new Vector2(HitDistance, rigid.velocity.y);
        //    //}
        //    //else
        //    //{
        //    //    //rigid.velocity = new Vector2(-1 * HitDistance, rigid.velocity.y);
        //    //}
        //    yield return new WaitForSeconds(0.6f);
        //    //rigid.velocity = new Vector2(0, rigid.velocity.y);
        //    //player.stopMoving_X = false;
        //    //player.stopAllInput = false;
        //}
        //IEnumerator HitGracePeriod()
        //{
        //    Debug.Log("im hit");
        //    for (int i = 0; i < 10; i++)
        //    {
        //        spriteRenderer.material.color = new Color(1f, 1f, 1f, .5f);
        //        yield return new WaitForSeconds(0.1f);
        //        spriteRenderer.material.color = new Color(1f, 1f, 1f, 1f);
        //        yield return new WaitForSeconds(0.1f);
        //    }
        //    isHit = false;
        //    //boxCollider.isTrigger = false;
        //    StopCoroutine("HitGracePeriod");
        //}

        public void TakeDamage(float damage, DamageType damageType, Quaternion damagedRotation)
        {
            if(playerFsm.stateMachine.CurrentState == playerFsm.hurt || 
               playerFsm.stateMachine.CurrentState == playerFsm.roll)
            {
                return;
            }
            if(damage <= 0)
            {
                if(damageType == DamageType.Poisoned)
                {
                    this.RestartCoroutine(Poisoned(), ref coroutine);
                }
                return;
            }
            Hp -= damage;
            if(Hp <= 0)
            {
                return;
            }
            playerFsm.stateMachine.SetState(playerFsm.hurt);
            playerPhysics.rotation =
                Quaternion.Euler(playerPhysics.rotation.x, damagedRotation.y == 0 ? 180 : 0, playerPhysics.rotation.z);
            if (damageType == DamageType.Range)
            {
                StartCoroutine(KnockBack(rangeKnockBackTime, damagedRotation));
            }
            else if (damageType == DamageType.Poisoned)
            {
                StartCoroutine(KnockBack(rangeKnockBackTime, damagedRotation));
                this.RestartCoroutine(Poisoned(), ref coroutine);
                //StartCoroutine(Poisoned());
            }
            else
            {
                StartCoroutine(KnockBack(meleeKnockBackTime, damagedRotation));
            }
        }

        IEnumerator KnockBack(WaitForSeconds waitTime, Quaternion damagedRotation)
        {
            playerMovement.velocity.x = damagedRotation.y == 0 ? 5 : -5;
            playerMovement.velocity.y = 5;
            playerMovement.stopCheckFlip = true;
            playerMovement.canInput = false;
            //hitEffectRenderer.flip = enemyPhysics.rotation.y == 0 ? new Vector3(1, 0, 0) : new Vector3(0, 0, 0);
            //hitEffect.Play();
            yield return waitTime;
            playerMovement.stopCheckFlip = false;
            playerMovement.canInput = true;
            playerMovement.velocity.x = 0;
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