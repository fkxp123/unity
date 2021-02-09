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

        bool isCrushed = false;
        float direction = 1;

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
                if (hp <= 0)
                {
                    fsm.currentState = fsm.die;
                    if (isCrushed)
                    {
                        transform.parent.gameObject.SetActive(false);
                        return;
                    }
                    TurnSpriteRed();
                    StartCoroutine(VibrateSprite());
                    StartCoroutine(KillObject());
                }
            }
        }

        Transform enemyPhysics;

        void TurnSpriteRed()
        {

        }
        IEnumerator VibrateSprite()
        {
            float currentTime = 2.5f;
            while(currentTime <= 0)
            {
                transform.position = transform.position + Vector3.right * 0.015f * direction;
                transform.position = transform.position + Vector3.up * 0.015f * direction;
                direction *= -1;
                currentTime -= Time.deltaTime;
                yield return null;
            }
            transform.position = enemyPhysics.position;
        }
        IEnumerator KillObject()
        {
            float currentTime = 5.5f;
            while(currentTime <= 0)
            {
                currentTime -= Time.deltaTime;
                yield return null;
            }
            //enemyPhysics.gameObject.SetActive(false);
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
            enemyMovement = enemyPhysics.GetComponent<EnemyMovement>();
            hitEffectRenderer = hitEffect.GetComponent<ParticleSystemRenderer>();

            maxHp = 100;
            Hp = maxHp;

            meleeKnockBackTime = new WaitForSeconds(0.3f); 
            rangeKnockBackTime = new WaitForSeconds(0.1f);
            stateRecoveryTime = new WaitForSeconds(0.6f);
        }

        public void TakeDamage(float damage, DamageType damageType, Quaternion damagedRotation)
        {
            Hp -= damage;
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
