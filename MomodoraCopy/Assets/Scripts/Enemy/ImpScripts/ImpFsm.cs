using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MomodoraCopy
{
    public class ImpFsm : BasicEnemyFsm
    {
        //System.Random random = new System.Random();

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
        
        public GameObject daggerSpawnerObject;
        DaggerSpawner daggerSpawner;
        Vector3 daggerSpawnPosition;
        PoolingObjectInfo daggerInfo;

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

        float coroutineCycle = 0.05f;
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

            daggerSpawner = daggerSpawnerObject.GetComponent<DaggerSpawner>();
            daggerSpawnPosition = new Vector3(0.8f, -0.3f, Random.Range(0.0f, 1.0f));
            poisonBombSpawner = poisonBombSpawnerObject.GetComponent<PoisonBombSpawner>();
            poisonBombSpawnPosition = new Vector3(0.8f, -0.3f, Random.Range(0.0f, 1.0f));

            siegeMode = true;
            shielderMode = false;

            waitTime = new WaitForSeconds(coroutineCycle);
            StartCoroutine(Fsm());

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
            #region find player by distance
            //Vector3 playerPos = GameManager.instance.playerPhysics.transform.position;
            //if (GameManager.instance.playerPhysics.transform.rotation.y != enemyPhysics.transform.rotation.y &&
            //    Mathf.Abs(playerPos.x - transform.position.x) < 10 && Mathf.Abs(playerPos.y - transform.position.y) < 5)
            //{
            //    if (playerPos.x < transform.position.x)
            //    {
            //        enemyPhysics.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            //    }
            //    else
            //    {
            //        enemyPhysics.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            //    }
            //    currentState = State.Chase;
            //}
            #endregion

            Collider2D[] colliders = Physics2D.OverlapBoxAll(findPlayerBox.transform.position, findPlayerBoxArea, playerMask);
            foreach (Collider2D collider in colliders)
            {
                if (collider.tag == "Player")
                {
                    playerPosition = collider.transform.position;
                    //siegeMode = Mathf.Abs(playerPosition.x - transform.position.x) > 10f ? true : false;
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
                    if (currentState == State.Idle && collider.transform.GetChild(0).GetComponent<ImpFsm>().isInteract)
                    {
                        currentState = State.Interact;
                        collider.transform.GetChild(0).GetComponent<ImpFsm>().currentState = State.Interact;
                        impFriend = collider.transform.GetChild(0).GetComponent<ImpFsm>();
                    }
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
            //float randomTime = random.Next(2, 4) + (float)random.NextDouble();
            float randomTime = Random.Range(2.0f, 4.0f);
            //int desicion = random.Next(0, 4);
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
                    //transitionState = State.Patrol;
                    break;
                case 1:
                    animator.CrossFade("sit", 0.3f);
                    //transitionState = State.Idle;
                    break;
                case 2:
                    animator.CrossFade("smileSit", 0.3f);
                    //transitionState = State.Idle;
                    break;
                case 3:
                    animator.Play(idleAnimHash);
                    isInteract = true;
                    //transitionState = State.Idle;
                    break;
                default:
                    break;
            }
            //isInteract = true;
            while(randomTime > 0)
            {
                if (enemyMovement.velocity.y < 0)
                {
                    animator.Play(fallAnimHash);
                }
                if (enemyMovement.velocity.y == 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("fall"))
                {
                    animator.Play(idleAnimHash);
                }
                if(currentState != State.Idle)
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
            if (currentState == State.Idle)
            {
                switch (desicion)
                {
                    case 0:
                        currentState = State.Patrol;
                        break;
                    case 1:
                        currentState = State.Idle;
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
            //int directionDesicion = random.Next(0, 2);
            int directionDesicion = Random.Range(0, 2);
            //float randomTime = (float)(1 + random.NextDouble());
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
                    //randomTime = (float)(1 + random.NextDouble());
                    randomTime = Random.Range(1.0f, 2.0f);
                }
                if (!isCliff)
                {
                    enemyPhysics.rotation = Quaternion.Euler(0.0f, enemyPhysics.rotation.y == 0.0f ? 180.0f : 0.0f, 0.0f);
                    enemyMovement.direction.x = enemyPhysics.rotation.y == 0.0f ? 1 : -1;
                    //randomTime = random.Next(2, 4) + (float)random.NextDouble();
                    randomTime = Random.Range(2.0f, 4.0f);
                }
                FindPlayer();
                if (currentState != State.Patrol)
                {
                    yield break;
                }
                randomTime -= coroutineCycle;
                yield return waitTime;
            }
            if (currentState == State.Patrol)
            {
                currentState = State.Idle;
            }
        }
        IEnumerator Interact()
        {
            enemyMovement.direction.x = 0;
            float totalInteractTime = 5f;
            while(totalInteractTime > 0)
            {
                float randomTime = Random.Range(2.0f, 4.0f);
                while(randomTime > 0)
                {
                    float randomCycle = Random.Range(0.0f, 4.0f);
                    int interactDesicion = Random.Range(0, 2);
                    float interactTime; 
                    if(interactDesicion == 1)
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
                        if (currentState != State.Interact)
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
                    if (currentState != State.Interact)
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
                if(totalInteractTime < 0)
                {
                    yield break;
                }
                yield return randomTime;
            }
            if(currentState == State.Interact)
            {
                currentState = State.Idle;
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
            while(randomTime > 0)
            {
                if (currentState != State.Attack)
                {
                    yield break;
                }
                randomTime -= coroutineCycle;
                yield return waitTime;
            }

            if (siegeMode)
            {
                animator.Play(throwBombAnimHash);
                attackTime = animTimeDictionary[throwBombAnimHash];
            }
            else
            {
                animator.Play(knifeThrowAnimHash);
                attackTime = animTimeDictionary[knifeThrowAnimHash];
            }
            while (attackTime > 0)
            {
                if (currentState != State.Attack)
                {
                    yield break;
                }
                attackTime -= coroutineCycle;
                yield return waitTime;
            }
            animator.Play(idleAnimHash);
            if (currentState == State.Attack)
            {
                currentState = State.Chase;
            }
        }
        IEnumerator Chase()
        {
            float lookAtPlayerRotationY;
            //enemyMovement.direction.x = enemyPhysics.rotation.y == 0 ? 1 : -1;
            //float randomTime = 1 + (float)random.NextDouble();
            float randomTime = Random.Range(1.0f, 2.0f);
            animator.Play(idleAnimHash);
            while (randomTime > 0)
            {
                if (enemyMovement.velocity.y > 0)
                {
                    animator.Play("jump");
                }
                else if (enemyMovement.velocity.y < 0)
                {
                    animator.Play("fall");
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

                if (siegeMode)
                {
                    enemyMovement.direction.x = 0;
                }
                else if (enemyMovement.velocity.y == 0 && controller.collisions.below)
                {
                    enemyMovement.direction.x = enemyPhysics.rotation.y == 0 ? -1 : 1;
                    enemyMovement.velocity.y = Random.Range(12.0f, 15.0f);
                }

                if (currentState != State.Chase)
                {
                    yield break;
                }
                randomTime -= coroutineCycle;
                yield return waitTime;
            }
            Collider2D[] colliders = Physics2D.OverlapBoxAll(findPlayerBox.transform.position + 
                (enemyPhysics.rotation.y == 0 ? Vector3.right * 4 : Vector3.left * 4), new Vector3(17, 17f, 0), 0);
            foreach (Collider2D collider in colliders)
            {
                if (collider.tag == "Player")
                {
                    if (currentState == State.Chase)
                    {
                        currentState = State.Attack;
                    }
                }
            }
            if (currentState == State.Chase)
            {
                currentState = State.Idle;
            }
        }
        IEnumerator Hurt()
        {
            enemyMovement.direction.x = 0;
            bloodEffect.Play();
            float hurtTime = animTimeDictionary[hurtAnimHash] * 4;
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
                    hurtTime = animTimeDictionary[hurtAnimHash] * 4;
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

        void SpawnDagger()
        {
            daggerInfo = daggerSpawner.info;
            throwDirection = enemyPhysics.rotation.y == 0.0f ? 1 : -1;
            daggerSpawnPosition.x = throwDirection * Mathf.Abs(daggerSpawnPosition.x);
            daggerInfo.position = transform.position + daggerSpawnPosition;
            daggerInfo.objectRotation = enemyPhysics.rotation;
            daggerSpawner.OperateSpawn(daggerInfo, DaggerSpawner.ACTIVATE_TIME);
        }
        void SpawnBomb()
        {
            poisonBombInfo = poisonBombSpawner.info;
            throwDirection = enemyPhysics.rotation.y == 0.0f ? 1 : -1;
            poisonBombSpawnPosition.x = throwDirection * Mathf.Abs(poisonBombSpawnPosition.x);
            poisonBombInfo.position = transform.position + poisonBombSpawnPosition;
            poisonBombInfo.objectRotation = enemyPhysics.rotation;
            poisonBombSpawner.OperateSpawn(poisonBombInfo, PoisonBombSpawner.ACTIVATE_TIME);
        }

        #region Update-based fsm
        //protected override void Update()
        //{
        //    if (temporaryState != State.None)
        //    {
        //        ExecuteState(temporaryState);
        //        return;
        //    }
        //    if (enemyStatus.currentHp <= 0)
        //    {
        //        currentState = State.Die;
        //    }
        //    if (currentState == State.Die)
        //    {
        //        animator.Play(hurtAnimHash);
        //        return;
        //    }

        //    if (currentState != State.Chase && currentState != State.Attack
        //        && currentState != State.Hurt && currentState != State.Die)
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
        //                currentState = State.Chase;
        //                currentTime = 0;
        //            }
        //        }
        //    }

        //    switch (currentState)
        //    {
        //        case State.Idle:
        //            DoIdle();
        //            break;
        //        case State.Patrol:
        //            DoPatrol();
        //            break;
        //        case State.Chase:
        //            DoChase();
        //            break;
        //        case State.Attack:
        //            DoAttack();
        //            break;
        //        case State.Die:
        //            DoDie();
        //            break;
        //        case State.Hurt:
        //            DoHurt();
        //            break;
        //        default:
        //            break;
        //    }
        //}

        //protected override void DoIdle()
        //{
        //    enemyMovement.direction.x = 0;
        //    if (enemyMovement.velocity.y < 0)
        //    {
        //        animator.Play(fallAnimHash);
        //    }
        //    if (enemyMovement.velocity.y == 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("fall"))
        //    {
        //        animator.Play(idleAnimHash);
        //    }

        //    if (currentTime <= 0)
        //    {
        //        int desicion = Random.Range(0, 4);
        //        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        //        {
        //            desicion = 3;
        //        }
        //        currentTime = animTimeDictionary[idleAnimHash] * 4;
        //        switch (desicion)
        //        {
        //            case 0:
        //                animator.Play(idleAnimHash);
        //                transitionState = State.Patrol;
        //                break;
        //            case 1:
        //                animator.CrossFade("sit", 0.3f);
        //                transitionState = State.Idle;
        //                break;
        //            case 2:
        //                animator.CrossFade("smileSit", 0.3f);
        //                transitionState = State.Idle;
        //                break;
        //            case 3:
        //                animator.Play(idleAnimHash);
        //                transitionState = State.Idle;
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
        //        enemyMovement.direction.x = enemyPhysics.rotation.y == 0.0f ? 1 : -1;
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
        //        currentState = State.Idle;
        //    }

        //}
        //protected override void DoHurt()
        //{
        //    enemyMovement.direction.x = 0;
        //    bloodEffect.Play();
        //    currentTime = 0;
        //    animator.Play(hurtAnimHash, -1, 0);
        //}
        //protected override void DoDie()
        //{

        //}

        //protected override void DoAttack()
        //{
        //    if (currentTime <= 0.0f)
        //    {
        //        animator.Play(knifeThrowAnimHash);
        //        currentTime = animTimeDictionary[knifeThrowAnimHash];
        //    }
        //    currentTime -= Time.deltaTime;
        //    if (currentTime <= 0.0f)
        //    {
        //        currentState = State.Idle;
        //    }
        //}

        //protected override void DoChase()
        //{
        //    if (enemyMovement.velocity.y > 0)
        //    {
        //        animator.Play("jump");
        //    }
        //    else if (enemyMovement.velocity.y < 0)
        //    {
        //        animator.Play("fall");
        //    }
        //    else
        //    {
        //        enemyMovement.direction.x = 0;
        //        if (animator.GetCurrentAnimatorStateInfo(0).IsName("fall"))
        //        {
        //            animator.Play(idleAnimHash);
        //        }
        //    }
        //    float lookAtPlayerRotationY;

        //    if (GameManager.instance.playerPhysics.transform.position.x < enemyPhysics.position.x)
        //    {
        //        lookAtPlayerRotationY = 180;
        //    }
        //    else
        //    {
        //        lookAtPlayerRotationY = 0;
        //    }
        //    enemyPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);
        //    if (enemyMovement.velocity.y == 0 && controller.collisions.below)
        //    {
        //        enemyMovement.direction.x = enemyPhysics.rotation.y == 0 ? -1 : 1;
        //        enemyMovement.velocity.y = Random.Range(12.0f, 15.0f);
        //        if (canAttack)
        //        {
        //            canAttack = false;
        //            float randomTime = Random.Range(1.0f, 1.5f);
        //            WaitForSeconds waitTime = new WaitForSeconds(randomTime);
        //            StartCoroutine(SetStateAttack(waitTime));
        //        }
        //    }
        //}
        //IEnumerator SetStateAttack(WaitForSeconds waitTime)
        //{
        //    yield return waitTime;
        //    canAttack = true;
        //    enemyMovement.direction.x = 0;
        //    currentTime = 0;
        //    currentState = State.Attack;
        //}
        #endregion

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
