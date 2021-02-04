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

        [SerializeField]
        ParticleSystem hitEffect;
        ParticleSystemRenderer hitEffectRenderer;
        [SerializeField]
        ParticleSystem crushedDeathEffect;

        [SerializeField]
        float maxHp = 100.0f;
        [SerializeField]
        float hp = 100.0f;
        float Hp
        {
            get { return hp; }
            set
            {
                hp = Mathf.Clamp(value, 0, maxHp);
                if (hp <= 0)
                {
                    //animator.play(hurt), vibration, particle, invoke-setactive(false);
                    gameObject.SetActive(false);
                }
            }
        }
        int currentHp;

        float knockBackTime = 0.5f;

        void Start()
        {
            enemyMovement = GetComponent<EnemyMovement>();
            animator = GetComponent<Animator>();
            fsm = GetComponent<BasicEnemyFsm>();
            hitEffectRenderer = hitEffect.GetComponent<ParticleSystemRenderer>();

            //maxHp += 20;
            //Hp += 20;
        }

        public void TakeDamage(float damage, DamageType damageType, Quaternion damagedRotation)
        {
            Hp -= damage;
            fsm.currentState = BasicEnemyFsm.State.Hurt;
            transform.rotation = Quaternion.Euler(transform.rotation.x, damagedRotation.y == 0 ? 180 : 0, transform.rotation.z);
            enemyMovement.direction.x = damagedRotation.y == 0 ? 1 : -1;
            hitEffectRenderer.flip = transform.rotation.y == 0 ? new Vector3(1,0,0) : new Vector3(0,0,0);
            hitEffect.Play();
            if(damageType == DamageType.Range)
            {
                CancelInvoke();
                Invoke("ResetMove", 0.25f);
                Invoke("SetStateChase", knockBackTime);
            }
            else
            {
                CancelInvoke();
                Invoke("ResetMove", knockBackTime);
                Invoke("SetStateChase", knockBackTime);
            }
        }
        void SetStateChase()
        {
            fsm.currentState = BasicEnemyFsm.State.Chase;
        }
        void ResetMove()
        {
            enemyMovement.direction.x = 0;
        }

        public void CrushedDeath()
        {
            Hp -= maxHp;

            if (enemyMovement.currentVelocity.x < 0)
            {
                crushedDeathEffect.transform.position = transform.position + Vector3.left * 0.5f;
                crushedDeathEffect.transform.rotation = Quaternion.Euler(0, 0, -90);
            }
            else if (enemyMovement.currentVelocity.x > 0)
            {
                crushedDeathEffect.transform.position = transform.position + Vector3.right * 0.5f;
                crushedDeathEffect.transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            else if (enemyMovement.currentVelocity.y > 0)
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
