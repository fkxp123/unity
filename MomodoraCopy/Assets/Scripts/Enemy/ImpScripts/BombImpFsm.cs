using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MomodoraCopy
{
    public class BombImpFsm : BasicEnemyFsm
    {
        public bool wantInteract;

        public Transform wallCheck;
        public float wallCheckRadius;
        public Transform cliffCheck;
        public float cliffCheckRadius;
        public LayerMask platform;
        public bool isNearToWall;
        public bool isCliff;

        public GameObject findPlayerBox;
        public Vector3 findPlayerBoxArea;

        bool canAttack = true;

        public GameObject poisonBombSpawnerObject;
        PoisonBombSpawner poisonBombSpawner;
        Vector3 poisonBombSpawnPosition;
        PoolingObjectInfo poisonBombInfo;

        int throwDirection = 1;

        public ParticleSystem bloodEffect;
        public ParticleSystem hitEffect;

        int idleAnimHash;
        int knifeThrowAnimHash;
        int hurtAnimHash;
        int jumpAnimHash;
        int fallAnimHash;
        int patrolAnimHash;
        int interactAnimHash;
        int smileInteractAnimHash;
        int throwBombAnimHash;
        WaitForSeconds idleTime;
        WaitForSeconds knifeThrowTime;
        WaitForSeconds throwBombTime;
        WaitForSeconds hurtTime;

        float coroutineCycle = 0.1f;
        WaitForSeconds waitTime;

        public bool siegeMode;
        public bool shielderMode;

        LayerMask playerMask;
        public bool isInteract;
        public ImpFsm impFriend;

        protected override void Start()
        {
            base.Start();

            transform.position = new Vector3(transform.position.x, transform.position.y, Random.Range(0.0f, 1.0f));

            poisonBombSpawner = poisonBombSpawnerObject.GetComponent<PoisonBombSpawner>();
            poisonBombSpawnPosition = new Vector3(poisonBombSpawner.transform.localPosition.x,
                poisonBombSpawner.transform.localPosition.y, Random.Range(0.0f, 1.0f));

            siegeMode = true;
            shielderMode = false;

            waitTime = new WaitForSeconds(coroutineCycle);

            playerMask = 1 << 8;
        }
        protected override void CachingAnimation()
        {
            idleAnimHash = Animator.StringToHash("idle");
            knifeThrowAnimHash = Animator.StringToHash("knifeThrow");
            hurtAnimHash = Animator.StringToHash("hurt");
            fallAnimHash = Animator.StringToHash("fall");
            jumpAnimHash = Animator.StringToHash("jump");
            patrolAnimHash = Animator.StringToHash("patrol");
            interactAnimHash = Animator.StringToHash("interact");
            smileInteractAnimHash = Animator.StringToHash("smileInteract");
            throwBombAnimHash = Animator.StringToHash("throwBomb");

            knifeThrowTime = new WaitForSeconds(animTimeDictionary[knifeThrowAnimHash]);
            throwBombTime = new WaitForSeconds(animTimeDictionary[throwBombAnimHash]);
            hurtTime = new WaitForSeconds(animTimeDictionary[hurtAnimHash] * 4);
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
        void FindFriend()
        {
            Vector3 friendPosition;
            Collider2D[] colliders = Physics2D.OverlapBoxAll(findPlayerBox.transform.position, findPlayerBoxArea, 0);
            foreach (Collider2D collider in colliders)
            {
                if (collider.tag == "Enemy" && collider.name == "ImpPhysics")
                {
                    Debug.Log(collider.name);
                    friendPosition = collider.transform.position;
                    if (friendPosition.x < transform.position.x)
                    {
                        enemyPhysics.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                    }
                    else
                    {
                        enemyPhysics.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    }
                    if (currentState == EnemyState.Idle && collider.transform.GetChild(0).GetComponent<ImpFsm>().isInteract)
                    {
                        currentState = EnemyState.Interact;
                        collider.transform.GetChild(0).GetComponent<ImpFsm>().currentState = EnemyState.Interact;
                        impFriend = collider.transform.GetChild(0).GetComponent<ImpFsm>();
                    }
                }
            }
        }

        IEnumerator Idle()
        {
            enemyMovement.direction.x = 0;
            float randomTime = Random.Range(2.0f, 4.0f);
            int desicion = Random.Range(0, 4);
            animator.Play(idleAnimHash);
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
            {
                desicion = 3;
            }

            switch (desicion)
            {
                case 0:
                    animator.Play(idleAnimHash);
                    break;
                case 1:
                    animator.CrossFade("sit", 0.3f);
                    break;
                case 2:
                    animator.CrossFade("smileSit", 0.3f);
                    break;
                case 3:
                    animator.Play(idleAnimHash);
                    isInteract = true;
                    break;
                default:
                    break;
            }
            //isInteract = true;
            while (randomTime > 0)
            {
                if (enemyMovement.velocity.y < 0)
                {
                    animator.Play(fallAnimHash);
                }
                if (enemyMovement.velocity.y == 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("fall"))
                {
                    animator.Play(idleAnimHash);
                }
                if (currentState != EnemyState.Idle)
                {
                    isInteract = false;
                    yield break;
                }
                else
                {
                    FindPlayer();
                    //FindFriend();
                }
                randomTime -= coroutineCycle;
                yield return waitTime;
            }
            if (currentState == EnemyState.Idle)
            {
                switch (desicion)
                {
                    case 0:
                        currentState = EnemyState.Idle;
                        //currentState = EnemyState.Patrol;
                        break;
                    case 1:
                        currentState = EnemyState.Idle;
                        break;
                    default:
                        break;
                }
            }
            isInteract = false;
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
        IEnumerator Interact()
        {
            enemyMovement.direction.x = 0;
            float totalInteractTime = 5f;
            while (totalInteractTime > 0)
            {
                float randomTime = Random.Range(2.0f, 4.0f);
                while (randomTime > 0)
                {
                    float randomCycle = Random.Range(0.0f, 4.0f);
                    int interactDesicion = Random.Range(0, 2);
                    float interactTime;
                    if (interactDesicion == 1)
                    {
                        animator.Play(interactAnimHash);
                        interactTime = animTimeDictionary[interactAnimHash];
                    }
                    else
                    {
                        animator.Play(smileInteractAnimHash);
                        interactTime = animTimeDictionary[smileInteractAnimHash];
                    }
                    while (interactTime > 0)
                    {
                        if (currentState != EnemyState.Interact)
                        {
                            isInteract = false;
                            yield break;
                        }
                        else
                        {
                            FindPlayer();
                        }
                        interactTime -= coroutineCycle;
                        yield return waitTime;
                    }
                    if (currentState != EnemyState.Interact)
                    {
                        isInteract = false;
                        yield break;
                    }
                    else
                    {
                        FindPlayer();
                    }
                    randomTime -= coroutineCycle;
                    yield return waitTime;
                }
                totalInteractTime -= randomTime;
                if (totalInteractTime < 0)
                {
                    yield break;
                }
                yield return randomTime;
            }
            if (currentState == EnemyState.Interact)
            {
                currentState = EnemyState.Idle;
            }
            isInteract = false;
        }
        IEnumerator Attack()
        {
            enemyMovement.direction.x = 0;
            float lookAtPlayerRotationY;
            lookAtPlayerRotationY = GameManager.instance.playerPhysics.transform.position.x < enemyPhysics.position.x ? 180 : 0;
            enemyPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);

            float randomTime = Random.Range(0.0f, 1.0f);
            float attackTime;
            while (randomTime > 0)
            {
                if (currentState != EnemyState.Attack)
                {
                    yield break;
                }
                randomTime -= coroutineCycle;
                yield return waitTime;
            }

            animator.Play(throwBombAnimHash);
            attackTime = animTimeDictionary[throwBombAnimHash];

            while (attackTime > 0)
            {
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
            float lookAtPlayerRotationY;
            float randomTime = Random.Range(1.0f, 2.0f);
            animator.Play(idleAnimHash);
            while (randomTime > 0)
            {
                if (enemyMovement.velocity.y > 0)
                {
                    animator.Play(jumpAnimHash);
                }
                else if (enemyMovement.velocity.y < 0)
                {
                    animator.Play(fallAnimHash);
                }
                else
                {
                    enemyMovement.direction.x = 0;
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("fall"))
                    {
                        animator.Play(idleAnimHash);
                    }
                }

                lookAtPlayerRotationY = GameManager.instance.playerPhysics.transform.position.x < enemyPhysics.position.x ? 180 : 0;
                enemyPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);

                enemyMovement.direction.x = 0;

                if (currentState != EnemyState.Chase)
                {
                    yield break;
                }
                randomTime -= coroutineCycle;
                yield return waitTime;
            }
            Collider2D[] colliders = Physics2D.OverlapBoxAll(findPlayerBox.transform.position, findPlayerBoxArea, 0);
            foreach (Collider2D collider in colliders)
            {
                if (collider.tag == "Player")
                {
                    if (currentState == EnemyState.Chase)
                    {
                        currentState = EnemyState.Attack;
                    }
                }
            }
            if (currentState == EnemyState.Chase)
            {
                currentState = EnemyState.Idle;
            }
        }
        IEnumerator Hurt()
        {
            enemyMovement.direction.x = 0;
            bloodEffect.Play();
            float hurtTime = animTimeDictionary[hurtAnimHash] * 4;
            animator.Play(hurtAnimHash, -1, 0);
            currentState = EnemyState.Idle;
            while (hurtTime > 0)
            {
                hurtTime -= coroutineCycle;
                yield return waitTime;
                if (currentState == EnemyState.Hurt)
                {
                    enemyMovement.direction.x = 0;
                    bloodEffect.Play();
                    hurtTime = animTimeDictionary[hurtAnimHash] * 4;
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

        void SpawnBomb()
        {
            poisonBombInfo = poisonBombSpawner.info;
            throwDirection = enemyPhysics.rotation.y == 0 ? 1 : -1;
            poisonBombSpawnPosition.x = throwDirection * Mathf.Abs(poisonBombSpawnPosition.x);
            poisonBombInfo.position = transform.position + poisonBombSpawnPosition;
            poisonBombInfo.objectRotation = enemyPhysics.rotation;
            poisonBombSpawner.OperateDynamicSpawn(poisonBombInfo, PoisonBombSpawner.ACTIVATE_TIME);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(cliffCheck.position, cliffCheckRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(findPlayerBox.transform.position, findPlayerBoxArea);
        }
    }
}
