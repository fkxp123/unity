using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MomodoraCopy
{
    public class ImpFsm : BasicEnemyFsm
    {
        bool findFriend;

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
        
        Vector3 daggerSpawnPosition;
        int throwDirection = 1;

        public GameObject daggerSpawnerObject;
        DaggerSpawner daggerSpawner;
        PoolingObjectInfo info;

        public ParticleSystem bloodEffect;
        public ParticleSystem hitEffect;

        protected override void Start()
        {
            base.Start();

            daggerSpawner = daggerSpawnerObject.GetComponent<DaggerSpawner>();
            daggerSpawnPosition = new Vector3(0.8f, -0.3f, Random.Range(0.0f, 1.0f));
        }
        int idleAnimHash;
        int knifeThrowAnimHash;
        int hurtAnimHash;
        int jumpAnimHash;
        int fallAnimHash;
        int patrolAnimHash;
        float idleTime;
        float knifeThrowTime;
        float hurtTime;

        protected override void CachingAnimation()
        {
            idleAnimHash = Animator.StringToHash("idle");
            idleTime = animTimeDictionary[idleAnimHash];

            knifeThrowAnimHash = Animator.StringToHash("knifeThrow");
            knifeThrowTime = animTimeDictionary[knifeThrowAnimHash];

            hurtAnimHash = Animator.StringToHash("hurt");
            hurtTime = animTimeDictionary[hurtAnimHash];

            jumpAnimHash = Animator.StringToHash("jump");
            fallAnimHash = Animator.StringToHash("fall");
            patrolAnimHash = Animator.StringToHash("patrol");
        }

        protected override void Update()
        {
            if (temporaryState != State.None)
            {
                ExecuteState(temporaryState);
                return;
            }
            if(enemyStatus.currentHp <= 0)
            {
                currentState = State.Die;
            }
            if(currentState == State.Die)
            {
                animator.Play(hurtAnimHash);
                return;
            }

            if (currentState != State.Chase && currentState != State.Attack 
                && currentState != State.Hurt && currentState != State.Die)
            {
                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(findPlayerBox.transform.position, findPlayerBoxArea, 0);
                foreach (Collider2D collider in collider2Ds)
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
                        currentTime = 0;
                    }
                }
            }

            switch (currentState)
            {
                case State.Idle:
                    DoIdle();
                    break;
                case State.Patrol:
                    DoPatrol();
                    break;
                case State.Chase:
                    DoChase();
                    break;
                case State.Attack:
                    DoAttack();
                    break;
                case State.Die:
                    DoDie();
                    break;
                case State.Hurt:
                    DoHurt();
                    break;
                default:
                    break;
            }
        }
        
        //execute-debug only
        protected override void DoIdle()
        {
            enemyMovement.direction.x = 0;
            if(enemyMovement.velocity.y < 0)
            {
                animator.Play(fallAnimHash);
            }
            if (enemyMovement.velocity.y == 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("fall"))
            {
                animator.Play(idleAnimHash);
            }

            if (currentTime <= 0)
            {
                int desicion = Random.Range(0, 4);
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
                {
                    desicion = 3;
                }
                currentTime = idleTime * 4;
                switch (desicion)
                {
                    case 0:
                        animator.Play(idleAnimHash);
                        transitionState = State.Patrol;
                        break;
                    case 1:
                        animator.CrossFade("sit",0.3f);
                        transitionState = State.Idle;
                        break;
                    case 2:
                        animator.CrossFade("smileSit", 0.3f);
                        transitionState = State.Idle;
                        break;
                    case 3:
                        animator.Play(idleAnimHash);
                        transitionState = State.Idle;
                        break;
                    default:
                        break;
                }
            }
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                currentState = transitionState;
            }
        }

        protected override void DoPatrol()
        {
            if (currentTime <= 0.0f)
            {
                animator.Play("patrol");
                int directionDesicion = Random.Range(0, 2);
                currentTime = Random.Range(1.0f, 2.0f);

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
            }
            isNearToWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, platform);
            isCliff = Physics2D.OverlapCircle(cliffCheck.position, cliffCheckRadius, platform);
            if (isNearToWall)
            {
                enemyPhysics.rotation = Quaternion.Euler(0.0f, enemyPhysics.rotation.y == 0.0f ? 180.0f : 0.0f, 0.0f);
                enemyMovement.direction.x = enemyPhysics.rotation.y == 0.0f ? 1 : -1;
                currentTime = Random.Range(1.0f, 2.0f);
            }
            if (!isCliff)
            {
                enemyPhysics.rotation = Quaternion.Euler(0.0f, enemyPhysics.rotation.y == 0.0f ? 180.0f : 0.0f, 0.0f);
                enemyMovement.direction.x = enemyPhysics.rotation.y == 0.0f ? 1 : -1;
                currentTime = Random.Range(1.0f, 2.0f);
            }
            currentTime -= Time.deltaTime;

            if (currentTime <= 0.0f)
            {
                currentState = State.Idle;
            }

        }
        protected override void DoHurt()
        {
            enemyMovement.direction.x = 0;
            bloodEffect.Play();
            currentTime = 0;
            animator.Play(hurtAnimHash, -1, 0);
        }
        protected override void DoDie()
        {
            
        }

        protected override void DoAttack()
        {
            if (currentTime <= 0.0f)
            {
                animator.Play(knifeThrowAnimHash);
                currentTime = knifeThrowTime;
            }
            currentTime -= Time.deltaTime;
            if (currentTime <= 0.0f)
            {
                currentState = State.Idle;
            }
        }

        protected override void DoChase()
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
            float lookAtPlayerRotationY;
            
            if(GameManager.instance.playerPhysics.transform.position.x < enemyPhysics.position.x)
            {
                lookAtPlayerRotationY = 180;
            }
            else
            {
                lookAtPlayerRotationY = 0;
            }
            enemyPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);
            if (enemyMovement.velocity.y == 0 && controller.collisions.below)
            {
                enemyMovement.direction.x = enemyPhysics.rotation.y == 0 ? -1 : 1;
                enemyMovement.velocity.y = Random.Range(12.0f, 15.0f);
                if (canAttack)
                {
                    canAttack = false;
                    float randomTime = Random.Range(1.0f, 1.5f);
                    WaitForSeconds waitTime = new WaitForSeconds(randomTime);
                    StartCoroutine(SetStateAttack(waitTime));
                }
            }
        }
        IEnumerator SetStateAttack(WaitForSeconds waitTime)
        {
            yield return waitTime;
            canAttack = true;
            enemyMovement.direction.x = 0;
            currentTime = 0;
            currentState = State.Attack;
        }

        //void CalculateTime(float activeTime, State transitionState)
        //{
        //    if(currentTime <= 0.0f)
        //    {
        //        currentTime = activeTime;
        //    }
        //    currentTime -= Time.deltaTime;
        //    if(currentTime <= 0.0f)
        //    {
        //        currentState = transitionState;
        //    }
        //}
        void SpawnDagger()
        {
            info = daggerSpawner.info;
            throwDirection = enemyPhysics.rotation.y == 0.0f ? 1 : -1;
            daggerSpawnPosition.x = throwDirection * Mathf.Abs(daggerSpawnPosition.x);
            info.position = transform.position + daggerSpawnPosition;
            info.objectRotation = enemyPhysics.rotation;
            daggerSpawner.OperateSpawn(info, DaggerSpawner.ACTIVATE_TIME);
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
