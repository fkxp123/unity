using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MomodoraCopy
{
    public class MonkeyFsm : BasicEnemyFsm
    {
        int idleAnimHash;
        int attackAnimHash;
        int patrolAnimHash;
        int hurt1AnimHash;
        int hurt2AnimHash;
        float idleTime;
        float attackTime;

        int hitCount;

        public Transform wallCheck;
        public float wallCheckRadius;
        public Transform cliffCheck;
        public float cliffCheckRadius;
        public LayerMask platform;
        public bool isNearToWall;
        public bool isCliff;

        public GameObject findPlayerBox;
        public Vector3 findPlayerBoxArea;
        public GameObject attackBox;
        public Vector3 attackBoxArea;
        float attackDamage = 20f;
        bool canAttack;

        public ParticleSystem bloodEffect;
        public ParticleSystem hitEffect;

        LayerMask playerMask;
        float coroutineCycle = 0.1f;
        WaitForSeconds waitTime;

        protected override void Start()
        {
            base.Start();
            waitTime = new WaitForSeconds(coroutineCycle);
            playerMask = 1 << 8;
        }

        protected override void CachingAnimation()
        {
            idleAnimHash = Animator.StringToHash("idle");
            idleTime = animTimeDictionary[idleAnimHash];

            attackAnimHash = Animator.StringToHash("attack");
            attackTime = animTimeDictionary[attackAnimHash];

            patrolAnimHash = Animator.StringToHash("patrol");
            hurt1AnimHash = Animator.StringToHash("hurt1");
            hurt2AnimHash = Animator.StringToHash("hurt2");
        }
        void FindPlayer()
        {
            #region find player by distance
            //Vector3 playerPos = GameManager.instance.playerPhysics.transform.position;
            //if (enemyPhysics.transform.rotation.y == 0 ? 
            //    (playerPos.x >= transform.position.x) : (playerPos.x <= transform.position.x) &&
            //    Mathf.Abs(playerPos.x - transform.position.x) < 10 && Mathf.Abs(playerPos.y - transform.position.y) >= 0)
            //{
            //    if (playerPos.x < transform.position.x)
            //    {
            //        enemyPhysics.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            //    }
            //    else
            //    {
            //        enemyPhysics.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            //    }
            //    currentState = EnemyState.Chase;
            //}
            #endregion

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

            float randomTime = Random.Range(2.0f, 4.0f);
            animator.Play(idleAnimHash);

            while (randomTime > 0)
            {
                if(currentState != EnemyState.Idle)
                {
                    yield break;
                }
                FindPlayer();
                randomTime -= coroutineCycle;
                yield return waitTime;
            }
            if (currentState == EnemyState.Idle)
            {
                int desicion = Random.Range(0, 3);
                switch (desicion)
                {
                    case 0:
                        currentState = EnemyState.Idle;
                        break;
                    case 1:
                        currentState = EnemyState.Patrol;
                        break;
                    case 2:
                        currentState = EnemyState.Patrol;
                        break;
                    default:
                        break;
                }
            }
        }
        IEnumerator Patrol()
        {
            animator.Play(patrolAnimHash);
            int directionDesicion = Random.Range(0, 2);
            float randomTime = Random.Range(1.0f, 2.0f);
            switch (directionDesicion)
            {
                case 0:
                    enemyPhysics.rotation = Quaternion.Euler(0.0f, enemyPhysics.rotation.y == 0.0f ? 180.0f : 0.0f, 0.0f);
                    break;
                case 1:
                    enemyPhysics.rotation = Quaternion.Euler(0.0f, enemyPhysics.rotation.y == 0.0f ? 0.0f : 180.0f, 0.0f);
                    break;
                default:
                    break;
            }
            enemyMovement.direction.x = enemyPhysics.rotation.y == 0.0f ? 1 : -1;
            while (randomTime > 0)
            {
                isNearToWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, platform);
                isCliff = Physics2D.OverlapCircle(cliffCheck.position, cliffCheckRadius, platform);
                if (isNearToWall)
                {
                    enemyPhysics.rotation = Quaternion.Euler(0.0f, enemyPhysics.rotation.y == 0.0f ? 180.0f : 0.0f, 0.0f);
                    enemyMovement.direction.x = enemyPhysics.rotation.y == 0.0f ? 1 : -1;
                    randomTime = Random.Range(1.0f, 2.0f);
                }
                if (!isCliff)
                {
                    enemyPhysics.rotation = Quaternion.Euler(0.0f, enemyPhysics.rotation.y == 0.0f ? 180.0f : 0.0f, 0.0f);
                    enemyMovement.direction.x = enemyPhysics.rotation.y == 0.0f ? 1 : -1;
                    randomTime = Random.Range(2.0f, 4.0f);
                }
                FindPlayer();
                if (currentState != EnemyState.Patrol)
                {
                    yield break;
                }
                randomTime -= coroutineCycle;
                yield return waitTime;
            }
            if (currentState == EnemyState.Patrol)
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

            //float waitAttackTime = 0.5f;
            //while (waitAttackTime > 0)
            //{
            //    if (currentState != EnemyState.Attack)
            //    {
            //        yield break;
            //    }
            //    waitAttackTime -= coroutineCycle;
            //    yield return waitTime;
            //}

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
                if (currentState != EnemyState.Attack)
                {
                    yield break;
                }
                attackTime -= coroutineCycle;
                yield return waitTime;
            }

            animator.Play(idleAnimHash);
            if (currentState == EnemyState.Attack)
            {
                currentState = EnemyState.Chase;
            }
        }
        IEnumerator Chase()
        {
            animator.Play(idleAnimHash);
            float recoverTime = Random.Range(0.1f, 0.3f);
            while(recoverTime > 0)
            {
                recoverTime -= Time.deltaTime;
                if (currentState != EnemyState.Chase)
                {
                    yield break;
                }
                yield return null;
            }

            while(currentState == EnemyState.Chase)
            {
                float lookAtPlayerRotationY = 
                    GameManager.instance.playerPhysics.transform.position.x < enemyPhysics.position.x ? 180 : 0;
                enemyPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);
                enemyMovement.direction.x = enemyPhysics.rotation.y == 0 ? 1 : -1;
                animator.Play(patrolAnimHash);
                if (currentState != EnemyState.Chase)
                {
                    yield break;
                }
                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackBox.transform.position, attackBoxArea, 0);
                foreach (Collider2D collider in collider2Ds)
                {
                    if (collider.tag == "Player")
                    {
                        currentState = EnemyState.Attack;
                        currentTime = 0;
                        yield break;
                    }
                }
                isNearToWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, platform);
                if (isNearToWall)
                {
                    enemyMovement.velocity.y = Random.Range(12.0f, 15.0f);
                }

                yield return waitTime;
            }
        }
        IEnumerator Hurt()
        {
            enemyMovement.direction.x = 0;
            bloodEffect.Play();

            float hurtTime = 0;

            hitCount += 1;
            if(hitCount % 2 == 0)
            {
                hurtTime = animTimeDictionary[hurt1AnimHash];
                animator.Play(hurt1AnimHash, -1, 0);
            }
            else
            {
                hurtTime = animTimeDictionary[hurt2AnimHash];
                animator.Play(hurt2AnimHash, -1, 0);
            }

            currentState = EnemyState.Idle;
            while (hurtTime > 0)
            {
                hurtTime -= coroutineCycle;
                yield return waitTime;
                if (currentState == EnemyState.Hurt)
                {
                    enemyMovement.direction.x = 0;
                    bloodEffect.Play();

                    hitCount += 1;
                    if (hitCount % 2 == 0)
                    {
                        hurtTime = animTimeDictionary[hurt1AnimHash];
                        animator.Play(hurt1AnimHash, -1, 0);
                    }
                    else
                    {
                        hurtTime = animTimeDictionary[hurt2AnimHash];
                        animator.Play(hurt2AnimHash, -1, 0);
                    }

                    currentState = EnemyState.Idle;
                }
            }
        }
        IEnumerator Die()
        {
            enemyMovement.direction.x = 0;
            if (hitCount % 2 == 0)
            {
                animator.Play(hurt1AnimHash, -1, 0);
            }
            else
            {
                animator.Play(hurt2AnimHash, -1, 0);
            }
            yield return null;
        }

        #region Update-based Fsm
        //protected override void Update()
        //{
        //    if (temporaryState != EnemyState.None)
        //    {
        //        ExecuteState(temporaryState);
        //        return;
        //    }
        //    if (enemyStatus.currentHp <= 0)
        //    {
        //        currentState = EnemyState.Die;
        //    }
        //    if (currentState == EnemyState.Die)
        //    {
        //        animator.Play(hurt1AnimHash);
        //        return;
        //    }

        //    if (currentState != EnemyState.Chase && currentState != EnemyState.Attack && currentState != EnemyState.Hurt)
        //    {
        //        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(findPlayerBox.transform.position, findPlayerBoxArea, 0);
        //        foreach (Collider2D collider in collider2Ds)
        //        {
        //            if (collider.tag == "Player")
        //            {
        //                playerPosition = collider.transform.position;
        //                if (playerPosition.x < transform.position.x)
        //                {
        //                    enemyPhysics.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        //                }
        //                else
        //                {
        //                    enemyPhysics.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        //                }
        //                currentState = EnemyState.Chase;
        //                currentTime = 0;
        //            }
        //        }
        //    }
        //    switch (currentState)
        //    {
        //        case EnemyState.Idle:
        //            DoIdle();
        //            break;
        //        case EnemyState.Patrol:
        //            DoPatrol();
        //            break;
        //        case EnemyState.Chase:
        //            DoChase();
        //            break;
        //        case EnemyState.Attack:
        //            DoAttack();
        //            break;
        //        case EnemyState.Hurt:
        //            DoHurt();
        //            break;
        //        case EnemyState.Die:
        //            DoDie();
        //            break;
        //        default:
        //            break;
        //    }
        //}
        //protected override void DoIdle()
        //{
        //    enemyMovement.direction.x = 0;

        //    if (currentTime <= 0)
        //    {
        //        int desicion = Random.Range(0, 2);
        //        currentTime = idleTime * 4;
        //        switch (desicion)
        //        {
        //            case 0:
        //                animator.Play("idle");
        //                transitionState = EnemyState.Patrol;
        //                break;
        //            case 1:
        //                animator.Play("idle");
        //                transitionState = EnemyState.Idle;
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //    currentTime -= Time.deltaTime;
        //    if (currentTime <= 0)
        //    {
        //        currentState = transitionState;
        //    }
        //}
        //protected override void DoPatrol()
        //{
        //    if (currentTime <= 0.0f)
        //    {
        //        animator.Play("patrol");
        //        int directionDesicion = Random.Range(0, 2);
        //        currentTime = Random.Range(1.0f, 2.0f);

        //        switch (directionDesicion)
        //        {
        //            case 0:
        //                enemyPhysics.rotation = Quaternion.Euler(0.0f, enemyPhysics.rotation.y == 0.0f ? 180.0f : 0.0f, 0.0f);
        //                break;
        //            case 1:
        //                break;
        //            default:
        //                break;
        //        }
        //       enemyMovement.direction.x = enemyPhysics.rotation.y == 0.0f ? 1 : -1;
        //    }
        //    isNearToWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, platform);
        //    isCliff = Physics2D.OverlapCircle(cliffCheck.position, cliffCheckRadius, platform);
        //    if (isNearToWall)
        //    {
        //        enemyPhysics.rotation = Quaternion.Euler(0.0f, enemyPhysics.rotation.y == 0.0f ? 180.0f : 0.0f, 0.0f);
        //        enemyMovement.direction.x = enemyPhysics.rotation.y == 0.0f ? 1 : -1;
        //        currentTime = Random.Range(1.0f, 2.0f);
        //    }
        //    if (!isCliff)
        //    {
        //        enemyPhysics.rotation = Quaternion.Euler(0.0f, enemyPhysics.rotation.y == 0.0f ? 180.0f : 0.0f, 0.0f);
        //        enemyMovement.direction.x = enemyPhysics.rotation.y == 0.0f ? 1 : -1;
        //        currentTime = Random.Range(1.0f, 2.0f);
        //    }
        //    currentTime -= Time.deltaTime;

        //    if (currentTime <= 0.0f)
        //    {
        //        currentState = EnemyState.Idle;
        //    }

        //}
        //protected override void DoHurt()
        //{
        //    enemyMovement.direction.x = 0;
        //    bloodEffect.Play();
        //    currentTime = 0;
        //    animator.Play(hurt1AnimHash, -1, 0);
        //}
        //protected override void DoAttack()
        //{
        //    enemyMovement.direction.x = 0;
        //    if (currentTime <= 0.0f)
        //    {
        //        animator.Play(attackAnimHash);
        //        currentTime = attackTime;
        //    }
        //    currentTime -= Time.deltaTime;
        //    if (canAttack)
        //    {
        //        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackBox.transform.position, attackBoxArea, 0);
        //        foreach (Collider2D collider in collider2Ds)
        //        {
        //            if (collider.tag == "Player")
        //            {
        //                GameManager.instance.playerStatus.TakeDamage(attackDamage, DamageType.Melee, transform.rotation);
        //            }
        //            canAttack = false;
        //        }
        //    }
        //    if (currentTime <= 0.0f)
        //    {
        //        currentState = EnemyState.Idle;
        //    }
        //}
        //protected override void DoChase()
        //{
        //    float lookAtPlayerRotationY;
        //    if(currentTime <= 0)
        //    {
        //        lookAtPlayerRotationY = GameManager.instance.playerPhysics.transform.position.x < enemyPhysics.position.x ? 180 : 0;
        //        enemyPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);
        //        enemyMovement.direction.x = enemyPhysics.rotation.y == 0 ? 1 : -1;
        //        currentTime = Random.Range(1.0f, 1.5f);
        //        animator.Play("patrol");
        //        //enemyMovement.velocity.x = impPhysics.rotation.y == 0 ? -5 : 5;
        //    }
        //    currentTime -= Time.deltaTime;
        //    Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackBox.transform.position, attackBoxArea, 0);
        //    foreach (Collider2D collider in collider2Ds)
        //    {
        //        if (collider.tag == "Player")
        //        {
        //            currentState = EnemyState.Attack;
        //            currentTime = 0;
        //            return;
        //        }
        //    }
        //    if (currentTime <= 0)
        //    {
        //        currentState = EnemyState.Idle;
        //    }
        //}
        #endregion

        void SetAbleAttackBox()
        {
            canAttack = true;
        }
        void SetDisableAttackBox()
        {
            canAttack = false;
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
