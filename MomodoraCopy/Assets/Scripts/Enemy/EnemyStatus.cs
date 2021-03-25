using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public enum DamageType
    {
        Melee,
        Range
    }
    public class EnemyStatus : MonoBehaviour
    {
        EnemyMovement enemyMovement;
        Animator animator;
        BasicEnemyFsm fsm;

        public ParticleSystem hitEffect;
        ParticleSystemRenderer hitEffectRenderer;
        public ParticleSystem crushedDeathEffect;
        SpriteRenderer spriteRenderer;

        bool isCrushed;
        float direction = 1;
        public float totalVibrationTime = 0.5f;
        public float vibrationAmount = 0.1f;
        public float vibrationCycleTime = 0.1f;
        public float blinkCycleTime = 0.1f;
        WaitForSeconds vibrationWaitTime;
        WaitForSeconds blinkWaitTime;

        [SerializeField]
        float maxHp;
        [SerializeField]
        float hp;
        float Hp
        {
            get { return hp; }
            set
            {
                hp = Mathf.Clamp(value, 0, maxHp);
                currentHp = hp;
                if (hp <= 0)
                {
                    fsm.currentState = fsm.die;
                    if (isCrushed)
                    {
                        transform.parent.gameObject.SetActive(false);
                        return;
                    }
                    StartCoroutine(VibrateSprite());
                }
                else if(hp != maxHp)
                {
                    StartCoroutine(BlinkSprite());
                }
            }
        }
        public float currentHp;

        Transform enemyPhysics;
        IEnumerator BlinkSprite()
        {
            float currentTime = 0.5f;
            float i = 0;
            Color tmp = spriteRenderer.color;
            while (currentTime > 0)
            {
                i++;
                if (i % 2 == 0)
                {
                    tmp.a = 0.3f;
                    spriteRenderer.color = tmp;
                }
                else
                {
                    tmp.a = 1f;
                    spriteRenderer.color = tmp;
                }
                currentTime -= 0.1f;
                yield return new WaitForSeconds(0.1f);
            }
            tmp.a = 1f;
            spriteRenderer.color = tmp;
        }
        IEnumerator VibrateSprite()
        {
            float currentTime = totalVibrationTime;
            float i = 0;
            Color tmp = spriteRenderer.color;
            while(currentTime > 0)
            {
                transform.position = transform.position + 
                    (i % 2 == 0 ? Vector3.right : Vector3.left) * vibrationAmount * direction;
                transform.position = transform.position + Vector3.up * vibrationAmount * direction;
                direction *= -1;
                if(direction == 1)
                {
                    i++;
                }
                if(direction == -1)
                {
                    tmp = Color.red;
                    tmp.a = 0.5f;
                    spriteRenderer.color = tmp;
                }
                else
                {
                    tmp = Color.white;
                    tmp.a = 1f;
                    spriteRenderer.color = tmp;
                }
                currentTime -= 0.1f;
                yield return new WaitForSeconds(0.1f);
            }
            transform.position = enemyPhysics.position;
            enemyPhysics.gameObject.SetActive(false);
        }

        WaitForSeconds meleeKnockBackTime;
        WaitForSeconds rangeKnockBackTime;
        WaitForSeconds stateRecoveryTime;

        Coroutine coroutine;

        void Start()
        {
            enemyPhysics = transform.parent;
            fsm = GetComponent<BasicEnemyFsm>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            enemyMovement = enemyPhysics.GetComponent<EnemyMovement>();
            hitEffectRenderer = hitEffect.GetComponent<ParticleSystemRenderer>();

            blinkWaitTime = new WaitForSeconds(blinkCycleTime);
            vibrationWaitTime = new WaitForSeconds(vibrationCycleTime);

            maxHp = 100;
            Hp = maxHp;

            meleeKnockBackTime = new WaitForSeconds(0.3f); 
            rangeKnockBackTime = new WaitForSeconds(0.1f);
            stateRecoveryTime = new WaitForSeconds(0.6f);
        }

        public void TakeDamage(float damage, DamageType damageType, Quaternion damagedRotation)
        {
            if(Hp <= 0)
            {
                return;
            }
            Hp -= damage;
            if(Hp <= 0)
            {
                enemyMovement.direction.x = 0;
                return;
            }
            fsm.currentState = fsm.hurt;
            enemyPhysics.rotation = 
                Quaternion.Euler(enemyPhysics.rotation.x, damagedRotation.y == 0 ? 180 : 0, enemyPhysics.rotation.z);

            if (damageType == DamageType.Range)
            {
                this.RestartCoroutine(KnockBack(rangeKnockBackTime, damagedRotation), ref coroutine);
            }
            else
            {
                this.RestartCoroutine(KnockBack(meleeKnockBackTime, damagedRotation), ref coroutine);
            }
            this.RestartCoroutine(SetStateChase(), ref coroutine);
        }
        IEnumerator SetStateChase()
        {
            yield return stateRecoveryTime;
            if(fsm.currentState != fsm.die)
            {
                fsm.currentState = fsm.chase;
            }
        }
        IEnumerator KnockBack(WaitForSeconds waitTime, Quaternion damagedRotation)
        {
            enemyMovement.velocity.x = damagedRotation.y == 0 ? 5 : -5;
            enemyMovement.velocity.y = 5;
            hitEffectRenderer.flip = enemyPhysics.rotation.y == 0 ? new Vector3(1, 0, 0) : new Vector3(0, 0, 0);
            hitEffect.Play();
            yield return waitTime;
            enemyMovement.direction.x = 0;
        }

        public void CrushedDeath()
        {
            Hp -= maxHp;
            isCrushed = true;
            if (enemyMovement.currentVelocity.x < 0)
            {
                crushedDeathEffect.transform.position = enemyPhysics.position + Vector3.left * 0.5f;
                crushedDeathEffect.transform.rotation = Quaternion.Euler(0, 0, -90);
            }
            else if (enemyMovement.currentVelocity.x > 0)
            {
                crushedDeathEffect.transform.position = enemyPhysics.position + Vector3.right * 0.5f;
                crushedDeathEffect.transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            else if (enemyMovement.currentVelocity.y > 0)
            {
                crushedDeathEffect.transform.position = enemyPhysics.position + Vector3.up;
                crushedDeathEffect.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            else
            {
                crushedDeathEffect.transform.position = enemyPhysics.position + Vector3.down;
                crushedDeathEffect.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            crushedDeathEffect.Play();
        }
    }
}
