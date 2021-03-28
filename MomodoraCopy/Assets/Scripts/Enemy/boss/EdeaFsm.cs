using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MomodoraCopy
{
    public class EdeaFsm : BasicEnemyFsm
    {
        System.Random random = new System.Random();

        public GameObject findPlayerBox;
        public Vector3 findPlayerBoxArea;

        public Transform wallCheck;
        public float wallCheckRadius;
        public Transform cliffCheck;
        public float cliffCheckRadius;
        public LayerMask platform;
        public bool isNearToWall;
        public bool isCliff;

        public GameObject attackBox;
        public Vector3 attackBoxArea;
        float attackDamage = 20f;
        bool canAttack;
        WaitForSeconds findPlayerCycle;

        int idleAnimHash;
        int attackAnimHash;
        int tailAttackAnimHash;
        int preBreathAnimHash;
        int breathAnimHash;
        int postBreathAnimHash;
        int vomitAnimHash;
        int hurtAnimHash;
        int deathAnimHash;
        WaitForSeconds idleTime;
        WaitForSeconds attackTime;
        WaitForSeconds tailAttackTime;
        WaitForSeconds preBreathTime;
        WaitForSeconds breathTime;
        WaitForSeconds postBreathTime;
        WaitForSeconds vomitTime;
        WaitForSeconds hurtTime;
        WaitForSeconds deathTime;

        float coroutineCycle = 0.1f;
        WaitForSeconds waitTime;

        public ParticleSystem bloodEffect;

        protected override void Start()
        {
            base.Start();
            waitTime = new WaitForSeconds(coroutineCycle);
            StartCoroutine(Fsm());
        }
        protected override void CachingAnimation()
        {
            idleAnimHash = Animator.StringToHash("idle");
            attackAnimHash = Animator.StringToHash("attack");
            tailAttackAnimHash = Animator.StringToHash("tailAttack");
            preBreathAnimHash = Animator.StringToHash("preBreath");
            breathAnimHash = Animator.StringToHash("breath");
            postBreathAnimHash = Animator.StringToHash("postBreath");
            vomitAnimHash = Animator.StringToHash("vomit");
            hurtAnimHash = Animator.StringToHash("hurt");
            deathAnimHash = Animator.StringToHash("death");

            idleTime = new WaitForSeconds(animTimeDictionary[idleAnimHash] * 4);
            attackTime = new WaitForSeconds(animTimeDictionary[attackAnimHash]);
            tailAttackTime = new WaitForSeconds(animTimeDictionary[tailAttackAnimHash]);
            preBreathTime = new WaitForSeconds(animTimeDictionary[preBreathAnimHash]);
            breathTime = new WaitForSeconds(animTimeDictionary[breathAnimHash] * 6);
            postBreathTime = new WaitForSeconds(animTimeDictionary[postBreathAnimHash]);
            vomitTime = new WaitForSeconds(animTimeDictionary[vomitAnimHash]);
            hurtTime = new WaitForSeconds(animTimeDictionary[hurtAnimHash]);
            deathTime = new WaitForSeconds(animTimeDictionary[deathAnimHash]);
        }
        void FindPlayer()
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(findPlayerBox.transform.position, findPlayerBoxArea, 0);
            foreach (Collider2D collider in colliders)
            {
                if (collider.tag == "Player")
                {
                    playerPosition = collider.transform.position;
                    if (playerPosition.x < transform.position.x)
                    {
                        enemyPhysics.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                    }
                    else
                    {
                        enemyPhysics.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    }
                    currentState = State.Chase;
                }
            }
        }

        IEnumerator Fsm()
        {
            yield return null;
            while (true)
            {
                if (enemyStatus.currentHp <= 0)
                {
                    currentState = State.Die;
                }
                if (temporaryState != State.None)
                {
                    yield return StartCoroutine(temporaryState.ToString());
                }
                else
                {
                    yield return StartCoroutine(currentState.ToString());
                }
            }
        }
        IEnumerator Idle()
        {
            enemyMovement.direction.x = 0;
            int desicion = random.Next(0, 3);
            float randomTime = random.Next(2, 4) + (float)random.NextDouble();
            animator.Play(idleAnimHash);
            while (randomTime > 0)
            {
                if (currentState != State.Idle)
                {
                    randomTime = 0;
                }
                FindPlayer();
                randomTime -= coroutineCycle;
                yield return waitTime;
            }
            if (currentState == State.Idle)
            {
                switch (desicion)
                {
                    case 0:
                        currentState = State.Idle;
                        break;
                    case 1:
                        currentState = State.Attack;
                        break;
                    case 2:
                        currentState = State.Chase;
                        break;
                    default:
                        currentState = State.Idle;
                        break;
                }
            }
        }
        IEnumerator Chase()
        {
            float lookAtPlayerRotationY;
            lookAtPlayerRotationY = GameManager.instance.playerPhysics.transform.position.x < enemyPhysics.position.x ? 180 : 0;
            enemyPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);
            enemyMovement.direction.x = enemyPhysics.rotation.y == 0 ? 1 : -1;
            float randomTime = 1 + (float)random.NextDouble();
            animator.Play("move");
            while (randomTime > 0)
            {
                Collider2D[] colliders = Physics2D.OverlapBoxAll(attackBox.transform.position, attackBoxArea, 0);
                foreach (Collider2D collider in colliders)
                {
                    if (collider.tag == "Player")
                    {
                        currentState = State.Attack;
                        randomTime = 0;
                    }
                }
                if (currentState != State.Chase)
                {
                    randomTime = 0;
                }
                randomTime -= coroutineCycle;
                yield return waitTime;
            }
            if (currentState == State.Chase)
            {
                currentState = State.Idle;
            }
        }
        IEnumerator Attack()
        {
            enemyMovement.direction.x = 0;
            float lookAtPlayerRotationY;
            lookAtPlayerRotationY = GameManager.instance.playerPhysics.transform.position.x < enemyPhysics.position.x ? 180 : 0;
            enemyPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);
            animator.Play(attackAnimHash);
            float attackTime = animTimeDictionary[attackAnimHash];
            while (attackTime > 0)
            {
                if (canAttack)
                {
                    Collider2D[] colliders = Physics2D.OverlapBoxAll(attackBox.transform.position, attackBoxArea, 0);
                    foreach (Collider2D collider in colliders)
                    {
                        if (collider.tag == "Player")
                        {
                            collider.transform.GetChild(1).GetComponent<PlayerStatus>().
                                TakeDamage(attackDamage, DamageType.Melee, transform.rotation);
                        }
                        canAttack = false;
                    }
                }
                if (currentState != State.Attack)
                {
                    attackTime = 0;
                }
                attackTime -= coroutineCycle;
                yield return waitTime;
            }
            if (currentState == State.Attack)
            {
                currentState = State.Chase;
            }
        }
        void SetAbleAttackBox()
        {
            canAttack = true;
        }
        void SetDisableAttackBox()
        {
            canAttack = false;
        }
        IEnumerator Hurt()
        {
            enemyMovement.direction.x = 0;
            bloodEffect.Play();
            float hurtTime = animTimeDictionary[hurtAnimHash];
            animator.Play(hurtAnimHash, -1, 0);
            currentState = State.Idle;
            while (hurtTime > 0)
            {
                hurtTime -= coroutineCycle;
                yield return waitTime;
                if (currentState == State.Hurt)
                {
                    enemyMovement.direction.x = 0;
                    bloodEffect.Play();
                    hurtTime = animTimeDictionary[hurtAnimHash];
                    animator.Play(hurtAnimHash, -1, 0);
                    currentState = State.Idle;
                }
            }
        }
        IEnumerator Die()
        {
            enemyMovement.direction.x = 0;
            animator.Play(hurtAnimHash);
            yield return null;
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(cliffCheck.position, cliffCheckRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(findPlayerBox.transform.position, findPlayerBoxArea);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(attackBox.transform.position, attackBoxArea);
        }
    }
}