using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MomodoraCopy
{
    public class ShieldImpFsm : BasicEnemyFsm
    {
        public Transform forwardWallCheck;
        public Transform backwardWallCheck;
        public float wallCheckRadius;
        public Transform cliffCheck;
        public float cliffCheckRadius;
        public LayerMask platform;
        public bool isForwardWall;
        public bool isBackwardWall;
        public bool isCliff;

        public GameObject findPlayerBox;
        public Vector3 findPlayerBoxArea;

        bool canAttack = true;

        public GameObject daggerSpawnerObject;
        DaggerSpawner daggerSpawner;
        Vector3 daggerSpawnPosition;
        PoolingObjectInfo daggerInfo;

        int throwDirection = 1;

        public ParticleSystem bloodEffect;
        public ParticleSystem hitEffect;

        int shieldIdleAnimHash;
        int shieldHurtAnimHash;
        int shieldMoveAnimHash;
        int shieldKnifeThrowAnimHash;
        int preShieldFallAnimHash;
        int shieldFallAnimHash;
        int shieldJumpAnimHash;

        float coroutineCycle = 0.1f;
        WaitForSeconds waitTime;

        LayerMask playerMask;

        protected override void Start()
        {
            base.Start();

            transform.position = new Vector3(transform.position.x, transform.position.y, Random.Range(0.0f, 1.0f));

            daggerSpawner = daggerSpawnerObject.GetComponent<DaggerSpawner>();
            daggerSpawnPosition = new Vector3(daggerSpawner.transform.localPosition.x,
                daggerSpawner.transform.localPosition.y, Random.Range(0.0f, 1.0f));

            waitTime = new WaitForSeconds(coroutineCycle);

            playerMask = 1 << 8;
        }
        protected override void CachingAnimation()
        {
            shieldIdleAnimHash = Animator.StringToHash("shieldIdle");
            shieldHurtAnimHash = Animator.StringToHash("shieldHurt");
            shieldKnifeThrowAnimHash = Animator.StringToHash("shieldKnifeThrow");
            preShieldFallAnimHash = Animator.StringToHash("preShieldFall");
            shieldFallAnimHash = Animator.StringToHash("shieldFall");
            shieldJumpAnimHash = Animator.StringToHash("shieldJump");
            shieldMoveAnimHash = Animator.StringToHash("shieldMove");
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
                    currentState = EnemyState.Chase;
                }
            }
        }

        void CheckGuard()
        {
            if(enemyPhysics.rotation.y == 0)
            {
                if(GameManager.instance.playerPhysics.transform.position.x > enemyPhysics.transform.position.x)
                {
                    enemyPhysics.transform.tag = "Untagged";
                }
                else if(GameManager.instance.playerPhysics.transform.position.x < enemyPhysics.transform.position.x)
                {
                    enemyPhysics.transform.tag = "Enemy";
                }
            }
            else
            {
                if (GameManager.instance.playerPhysics.transform.position.x > enemyPhysics.transform.position.x)
                {
                    enemyPhysics.transform.tag = "Enemy";
                }
                else if (GameManager.instance.playerPhysics.transform.position.x < enemyPhysics.transform.position.x)
                {
                    enemyPhysics.transform.tag = "Untagged";
                }
            }

        }

        IEnumerator Idle()
        {
            enemyMovement.direction.x = 0;
            float randomTime = Random.Range(2.0f, 4.0f);
            animator.Play(shieldIdleAnimHash);
            while (randomTime > 0)
            {
                if (enemyMovement.velocity.y < 0)
                {
                    animator.Play(shieldFallAnimHash);
                }
                if (enemyMovement.velocity.y == 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("shieldFall"))
                {
                    animator.Play(shieldIdleAnimHash);
                }
                if (currentState != EnemyState.Idle)
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
            animator.Play(shieldMoveAnimHash);
            int directionDesicion = Random.Range(0, 2);
            float randomTime = Random.Range(1.0f, 2.0f);
            switch (directionDesicion)
            {
                case 0:
                    enemyPhysics.rotation = Quaternion.Euler(0.0f, enemyPhysics.rotation.y == 0.0f ? 180.0f : 0.0f, 0.0f);
                    break;
                default:
                    break;
            }
            enemyMovement.direction.x = enemyPhysics.rotation.y == 0.0f ? 1 : -1;
            while (randomTime > 0)
            {
                isForwardWall = Physics2D.OverlapCircle(forwardWallCheck.position, wallCheckRadius, platform);
                isCliff = Physics2D.OverlapCircle(cliffCheck.position, cliffCheckRadius, platform);
                if (isForwardWall)
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

            float attackTime;
            animator.Play(shieldKnifeThrowAnimHash);
            attackTime = animTimeDictionary[shieldKnifeThrowAnimHash];

            while (attackTime > 0)
            {
                CheckGuard();
                if (currentState != EnemyState.Attack)
                {
                    yield break;
                }
                attackTime -= coroutineCycle;
                yield return waitTime;
            }
            animator.Play(shieldIdleAnimHash);
            if (currentState == EnemyState.Attack)
            {
                currentState = EnemyState.Chase;
            }
        }
        IEnumerator Chase()
        {
            //float lookAtPlayerRotationY =
            //    GameManager.instance.playerPhysics.transform.position.x < enemyPhysics.position.x ? 180 : 0;
            //enemyPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);
            //enemyMovement.direction.x = enemyPhysics.rotation.y == 0 ? 1 : -1;
            float attackWaitTime = Random.Range(1.5f, 2.5f);
            float randomTime = Random.Range(0.0f, 2.0f);
            //float flipTime = 1f;
            while(currentState == EnemyState.Chase)
            {
                CheckGuard();
                if (enemyMovement.velocity.y > 0)
                {
                    animator.Play(shieldJumpAnimHash);
                }
                else if (enemyMovement.velocity.y < 0)
                {
                    animator.Play(shieldFallAnimHash);
                }
                else
                {
                    enemyMovement.direction.x = 0;
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("shieldFall"))
                    {
                        animator.Play(shieldIdleAnimHash);
                    }
                }
                //if ((enemyPhysics.rotation.y == 0 &&
                //    GameManager.instance.playerPhysics.transform.position.x > enemyPhysics.transform.position.x) ||
                //    (enemyPhysics.rotation.y == 180 &&
                //    GameManager.instance.playerPhysics.transform.position.x < enemyPhysics.transform.position.x))
                //{
                //    flipTime -= coroutineCycle;
                //    if(flipTime < 0)
                //    {
                //        lookAtPlayerRotationY =
                //            GameManager.instance.playerPhysics.transform.position.x < enemyPhysics.position.x ? 180 : 0;
                //        enemyPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);
                //        flipTime = 1f;
                //    }
                //}
                float lookAtPlayerRotationY =
                    GameManager.instance.playerPhysics.transform.position.x < enemyPhysics.position.x ? 180 : 0;
                enemyPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);
                if (Mathf.Abs(GameManager.instance.playerPhysics.transform.position.x - enemyPhysics.position.x) < 5)
                {
                    enemyMovement.direction.x = enemyPhysics.rotation.y == 0 ? -1 : 1;
                }
                else if (Mathf.Abs(GameManager.instance.playerPhysics.transform.position.x - enemyPhysics.position.x) > 5
                    && Mathf.Abs(GameManager.instance.playerPhysics.transform.position.x - enemyPhysics.position.x) <= 15)
                {
                    enemyMovement.direction.x = enemyPhysics.rotation.y == 0 ? 1 : -1;
                }
                else
                {
                    enemyMovement.direction.x = enemyPhysics.rotation.y == 0 ? 1 : -1;
                }
                animator.Play(shieldMoveAnimHash);
                attackWaitTime -= coroutineCycle;
                if(attackWaitTime <= 0)
                {
                    currentState = EnemyState.Attack;
                }
                isForwardWall = Physics2D.OverlapCircle(forwardWallCheck.position, wallCheckRadius, platform);
                isBackwardWall = Physics2D.OverlapCircle(backwardWallCheck.position, wallCheckRadius, platform);
                if(enemyPhysics.rotation.y == 0)
                {
                    if (isForwardWall && enemyMovement.direction.x > 0)
                    {
                        enemyMovement.velocity.y = Random.Range(12.0f, 15.0f);
                    }
                    else if (isBackwardWall && enemyMovement.direction.x < 0)
                    {
                        enemyMovement.velocity.y = Random.Range(12.0f, 15.0f);
                    }
                }
                else
                {
                    if (isForwardWall && enemyMovement.direction.x < 0)
                    {
                        enemyMovement.velocity.y = Random.Range(12.0f, 15.0f);
                    }
                    else if (isBackwardWall && enemyMovement.direction.x > 0)
                    {
                        enemyMovement.velocity.y = Random.Range(12.0f, 15.0f);
                    }
                }
                
                yield return waitTime;
            }
        }
        IEnumerator Hurt()
        {
            enemyPhysics.transform.tag = "Enemy";
            enemyMovement.direction.x = 0;
            bloodEffect.Play();
            float hurtTime = 0.5f;
            animator.Play(shieldHurtAnimHash, -1, 0);
            currentState = EnemyState.Chase;
            while (hurtTime > 0)
            {
                hurtTime -= coroutineCycle;
                yield return waitTime;
                if (currentState == EnemyState.Hurt)
                {
                    enemyPhysics.transform.tag = "Enemy";
                    enemyMovement.direction.x = 0;
                    bloodEffect.Play();
                    hurtTime = 0.5f;
                    animator.Play(shieldHurtAnimHash, -1, 0);
                    currentState = EnemyState.Chase;
                }
            }
        }
        IEnumerator Die()
        {
            enemyMovement.direction.x = 0;
            animator.Play(shieldHurtAnimHash);
            yield return null;
        }

        void SpawnDagger()
        {
            daggerInfo = daggerSpawner.info;
            throwDirection = enemyPhysics.rotation.y == 0 ? 1 : -1;
            daggerSpawnPosition.x = throwDirection * Mathf.Abs(daggerSpawnPosition.x);
            daggerInfo.position = transform.position + daggerSpawnPosition;
            daggerInfo.objectRotation = enemyPhysics.rotation;
            daggerSpawner.OperateDynamicSpawn(daggerInfo, DaggerSpawner.ACTIVATE_TIME);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(forwardWallCheck.position, wallCheckRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(cliffCheck.position, cliffCheckRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(findPlayerBox.transform.position, findPlayerBoxArea);
        }
    }
}
