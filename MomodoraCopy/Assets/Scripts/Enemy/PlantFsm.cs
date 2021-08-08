using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MomodoraCopy
{
    public class PlantFsm : BasicEnemyFsm
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
        int patrolAnimHash;
        int hurtAnimHash;
        WaitForSeconds idleTime;
        WaitForSeconds attackTime;
        WaitForSeconds patrolTime;
        WaitForSeconds hurtTime;

        float coroutineCycle = 0.1f;
        WaitForSeconds waitTime;

        public ParticleSystem bloodEffect;

        protected override void Start()
        {
            base.Start();
            waitTime = new WaitForSeconds(coroutineCycle);
            //StartCoroutine(FindPlayer());
            StartCoroutine(Fsm());
        }
        protected override void CachingAnimation()
        {
            idleAnimHash = Animator.StringToHash("idle");
            attackAnimHash = Animator.StringToHash("attack");
            patrolAnimHash = Animator.StringToHash("patrol");
            hurtAnimHash = Animator.StringToHash("hurt");

            idleTime = new WaitForSeconds(animTimeDictionary[idleAnimHash] * 4);
            attackTime = new WaitForSeconds(animTimeDictionary[attackAnimHash]);
            patrolTime = new WaitForSeconds(animTimeDictionary[patrolAnimHash]);
            hurtTime = new WaitForSeconds(animTimeDictionary[hurtAnimHash]);
        }
        //IEnumerator FindPlayer()
        //{
        //    while (true)
        //    {
        //        if (currentState != State.Chase && currentState != State.Attack && 
        //            currentState != State.Hurt && currentState != State.Die)
        //        {
        //            Collider2D[] colliders = Physics2D.OverlapBoxAll(findPlayerBox.transform.position, findPlayerBoxArea, 0);
        //            foreach (Collider2D collider in colliders)
        //            {
        //                if (collider.tag == "Player")
        //                {
        //                    playerPosition = collider.transform.position;
        //                    if (playerPosition.x < transform.position.x)
        //                    {
        //                        enemyPhysics.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        //                    }
        //                    else
        //                    {
        //                        enemyPhysics.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        //                    }
        //                    currentState = State.Chase;
        //                }
        //            }
        //        }
        //        yield return waitTime; 
        //    }
        //}
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
                    currentState = EnemyState.Chase;
                }
            }
        }

        IEnumerator Idle()
        {
            enemyMovement.direction.x = 0;
            int desicion = random.Next(0, 2);
            float randomTime = random.Next(2, 4) + (float)random.NextDouble();
            animator.Play(idleAnimHash);
            while(randomTime > 0)
            {
                if(currentState != EnemyState.Idle)
                {
                    yield break;
                }
                else
                {
                    FindPlayer();
                }
                randomTime -= coroutineCycle;
                yield return waitTime;
            }
            if(currentState == EnemyState.Idle)
            {
                switch (desicion)
                {
                    case 0:
                        currentState = EnemyState.Patrol;
                        break;
                    case 1:
                        currentState = EnemyState.Idle;
                        break;
                    default:
                        break;
                }
            }
        }
        IEnumerator Patrol()
        {
            animator.Play(patrolAnimHash);
            int directionDesicion = random.Next(0, 2);
            float randomTime = (float)(1 + random.NextDouble());
            switch (directionDesicion)
            {
                case 0:
                    enemyPhysics.rotation = Quaternion.Euler(0.0f, enemyPhysics.rotation.y == 0.0f ? 180.0f : 0.0f, 0.0f);
                    break;
                case 1:
                    break;
                default:
                    break;
            }
            enemyMovement.direction.x = enemyPhysics.rotation.y == 0.0f ? 1 : -1;
            while(randomTime > 0)
            {
                isNearToWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, platform);
                isCliff = Physics2D.OverlapCircle(cliffCheck.position, cliffCheckRadius, platform);
                if (isNearToWall)
                {
                    enemyPhysics.rotation = Quaternion.Euler(0.0f, enemyPhysics.rotation.y == 0.0f ? 180.0f : 0.0f, 0.0f);
                    enemyMovement.direction.x = enemyPhysics.rotation.y == 0.0f ? 1 : -1;
                    randomTime = (float)(1 + random.NextDouble());
                }
                if (!isCliff)
                {
                    enemyPhysics.rotation = Quaternion.Euler(0.0f, enemyPhysics.rotation.y == 0.0f ? 180.0f : 0.0f, 0.0f);
                    enemyMovement.direction.x = enemyPhysics.rotation.y == 0.0f ? 1 : -1;
                    randomTime = random.Next(2, 4) + (float)random.NextDouble();
                }
                FindPlayer();
                if (currentState != EnemyState.Patrol)
                {
                    yield break;
                }
                randomTime -= coroutineCycle;
                yield return waitTime;
            }
            if(currentState == EnemyState.Patrol)
            {
                currentState = EnemyState.Idle;
            }
        }
        IEnumerator Chase()
        {
            float lookAtPlayerRotationY;
            lookAtPlayerRotationY = GameManager.instance.playerPhysics.transform.position.x < enemyPhysics.position.x ? 180 : 0;
            enemyPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);
            enemyMovement.direction.x = enemyPhysics.rotation.y == 0 ? 1 : -1;
            float randomTime = 1 + (float)random.NextDouble();
            animator.Play("patrol");
            while (randomTime > 0)
            {
                Collider2D[] colliders = Physics2D.OverlapBoxAll(attackBox.transform.position, attackBoxArea, 0);
                foreach (Collider2D collider in colliders)
                {
                    if (collider.tag == "Player")
                    {
                        currentState = EnemyState.Attack;
                        yield break;
                    }
                }
                if(currentState != EnemyState.Chase)
                {
                    yield break;
                }
                randomTime -= coroutineCycle;
                yield return waitTime;
            }
            if(currentState == EnemyState.Chase)
            {
                currentState = EnemyState.Idle;
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
                            GameManager.instance.playerStatus.TakeDamage(attackDamage, DamageType.Melee, transform.rotation);
                        }
                        canAttack = false;
                    }
                }
                if(currentState != EnemyState.Attack)
                {
                    yield break;
                }
                attackTime -= coroutineCycle;
                yield return waitTime;
            }
            if(currentState == EnemyState.Attack)
            {
                currentState = EnemyState.Chase;
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
            currentState = EnemyState.Idle;
            while(hurtTime > 0)
            {
                hurtTime -= coroutineCycle;
                yield return waitTime;
                if(currentState == EnemyState.Hurt)
                {
                    enemyMovement.direction.x = 0;
                    bloodEffect.Play();
                    hurtTime = animTimeDictionary[hurtAnimHash];
                    animator.Play(hurtAnimHash, -1, 0);
                    currentState = EnemyState.Idle;
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