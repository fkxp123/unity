using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class ImpFsm : BasicEnemyFsm
    {
        Animator animator;
        SpriteRenderer spriteRenderer;
        EnemyMovement enemyMovement;
        Controller2D controller;
        EnemyStatus enemyStatus;

        float currentTime;
        float idleTime;
        float knifeThrowTime;
        float hurtTime;

        bool findFriend;

        public Transform wallCheck;
        public float wallCheckRadius;
        public Transform cliffCheck;
        public float cliffCheckRadius;
        public LayerMask platform;
        public bool isNearToWall;
        public bool isCliff;

        public GameObject findPlayerBox;
        public Vector3 findPlayerBoxSize;

        bool canAttack = true;
        
        Vector3 playerPosition;
        Vector3 daggerSpawnPosition;
        int throwDirection = 1;

        [SerializeField]
        GameObject daggerSpawnerObject;
        DaggerSpawner daggerSpawner;
        PoolingObjectInfo info;

        [SerializeField]
        ParticleSystem bloodEffect;
        [SerializeField]
        ParticleSystem hitEffect;

        protected override void Start()
        {
            base.Start();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            enemyMovement = GetComponent<EnemyMovement>();
            controller = GetComponent<Controller2D>();
            enemyStatus = GetComponent<EnemyStatus>();

            daggerSpawner = daggerSpawnerObject.GetComponent<DaggerSpawner>();
            daggerSpawnPosition = new Vector3(0.8f, -0.3f, Random.Range(0.0f, 1.0f));
            CachingAnimationTime();


            var main = hitEffect.main;
            main.startRotation3D = true;
            //StartCoroutine("CheckSomething");
            //StartCoroutine("Test");
        }

        void CachingAnimationTime()
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                switch (clip.name)
                {
                    case "idle":
                        idleTime = clip.length;
                        break;
                    case "knifeThrow":
                        knifeThrowTime = clip.length;
                        break;
                    case "hurt":
                        hurtTime = clip.length;
                        break;
                    default:
                        break;
                }
            }
        }

        //void Check()
        //{
        //    if (executeState != State.None)
        //    {
        //        ExecuteState(executeState);
        //        return;
        //    }
        //    if (currentState != State.Chase && currentState != State.Attack)
        //    {
        //        bool flag = false;
        //        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(findPlayerBox.transform.position, findPlayerBoxSize, 0);
        //        foreach (Collider2D collider in collider2Ds)
        //        {
        //            if (collider.tag == "player")
        //            {
        //                flag = true;
        //                playerPosition = collider.transform.position;
        //                if (playerPosition.x < transform.position.x)
        //                {
        //                    transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        //                }
        //                else
        //                {
        //                    transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        //                }
        //                throwDirection = transform.rotation.y == 0.0f ? 1 : -1;
        //            }
        //        }
        //        if (flag)
        //        {
        //            //findPlayer = true;
        //            currentState = State.Chase;
        //            currentTime = 0;
        //            return;
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
        //        case State.Hurt:
        //            DoHurt();
        //            break;
        //        case State.Die:
        //            DoDie();
        //            break;
        //        default:
        //            break;
        //    }
        //}

        //IEnumerator CheckSomething()
        //{
        //    Check();
        //    yield return new WaitForSeconds(0.1f);
        //    StartCoroutine("CheckSomething");
        //}

        protected override void Update()
        {
            if (executeState != State.None)
            {
                ExecuteState(executeState);
                return;
            }
            if (currentState == State.Hurt)
            {
                //DoHurt();
                //if (!hitEffect.isPlaying)
                //{
                //    hitEffect.Play();
                //}
                //var main = hitEffect.main;
                //main.startRotationY = transform.rotation.y == 0 ? 135 : 0;
                bloodEffect.Play();
                currentTime = 0;
                animator.Play("hurt");
                return;
            }
            else
            {
                //bloodEffect.Stop();
                hitEffect.Stop();
            }
            if (currentState != State.Chase && currentState != State.Attack)
            {
                bool flag = false;
                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(findPlayerBox.transform.position, findPlayerBoxSize, 0);
                foreach (Collider2D collider in collider2Ds)
                {
                    if (collider.tag == "Player")
                    {
                        flag = true;
                        playerPosition = collider.transform.position;
                        if (playerPosition.x < transform.position.x)
                        {
                            transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                        }
                        else
                        {
                            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                        }
                    }
                }
                if (flag)
                {
                    currentState = State.Chase;
                    currentTime = 0;
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
                animator.Play("fall");
            }
            if (enemyMovement.velocity.y == 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("fall"))
            {
                animator.Play("idle");
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
                        animator.Play("idle");
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
                        animator.Play("idle");
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
                        transform.rotation = Quaternion.Euler(0.0f, transform.rotation.y == 0.0f ? 180.0f : 0.0f, 0.0f);
                        enemyMovement.direction.x = transform.rotation.y == 0.0f ? 1 : -1;
                        break;
                    case 1:
                        transform.rotation = Quaternion.Euler(0.0f, transform.rotation.y == 0.0f ? 180.0f : 0.0f, 0.0f);
                        enemyMovement.direction.x = transform.rotation.y == 0.0f ? 1 : -1;
                        break;
                    default:
                        break;
                }
            }
            isNearToWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, platform);
            isCliff = Physics2D.OverlapCircle(cliffCheck.position, cliffCheckRadius, platform);
            if (isNearToWall)
            {
                transform.rotation = Quaternion.Euler(0.0f, transform.rotation.y == 0.0f ? 180.0f : 0.0f, 0.0f);
                enemyMovement.direction.x = transform.rotation.y == 0.0f ? 1 : -1;
                currentTime = Random.Range(1.0f, 2.0f);
            }
            if (!isCliff)
            {
                transform.rotation = Quaternion.Euler(0.0f, transform.rotation.y == 0.0f ? 180.0f : 0.0f, 0.0f);
                enemyMovement.direction.x = transform.rotation.y == 0.0f ? 1 : -1;
                currentTime = Random.Range(1.0f, 2.0f);
            }
            currentTime -= Time.deltaTime;

            if (currentTime <= 0.0f)
            {
                currentState = State.Idle;
            }

        }
        protected override void DoAttack()
        {
            if (currentTime <= 0.0f)
            {
                animator.Play("knifeThrow");
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
                    animator.Play("idle");
                }
            }
            if (enemyMovement.velocity.y == 0)
            {
                enemyMovement.direction.x = transform.rotation.y == 0 ? -1 : 1;
                enemyMovement.velocity.y = Random.Range(12.0f, 15.0f);
                if (canAttack)
                {
                    canAttack = false;
                    float waitTime = Random.Range(0.5f, 1.5f);
                    Invoke("SetStateAttack", waitTime);
                }
            }
        }
        void SetStateAttack()
        {
            canAttack = true;
            enemyMovement.direction.x = 0;
            currentTime = 0;
            currentState = State.Attack;
        }

        void CalculateTime(float activeTime, State transitionState)
        {
            if(currentTime <= 0.0f)
            {
                currentTime = activeTime;
            }
            currentTime -= Time.deltaTime;
            if(currentTime <= 0.0f)
            {
                currentState = transitionState;
            }
        }
        void SpawnDagger()
        {
            info = daggerSpawner.info;
            throwDirection = transform.rotation.y == 0.0f ? 1 : -1;
            daggerSpawnPosition.x = throwDirection * Mathf.Abs(daggerSpawnPosition.x);
            info.position = transform.position + daggerSpawnPosition;
            info.objectRotation = transform.rotation;
            daggerSpawner.OperateSpawn(info, DaggerSpawner.ACTIVATE_TIME);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(cliffCheck.position, cliffCheckRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(findPlayerBox.transform.position, findPlayerBoxSize);
        }
    }
}
