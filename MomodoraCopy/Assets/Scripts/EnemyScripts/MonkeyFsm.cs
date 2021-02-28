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

        protected override void Start()
        {
            base.Start();
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

        protected override void Update()
        {
            if (temporaryState != State.None)
            {
                ExecuteState(temporaryState);
                return;
            }
            if (enemyStatus.currentHp <= 0)
            {
                currentState = State.Die;
            }
            if (currentState == State.Die)
            {
                animator.Play("hurt");
                return;
            }

            if (currentState != State.Chase && currentState != State.Attack && currentState != State.Hurt)
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
                case State.Hurt:
                    DoHurt();
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

            if (currentTime <= 0)
            {
                int desicion = Random.Range(0, 2);
                currentTime = idleTime * 4;
                switch (desicion)
                {
                    case 0:
                        animator.Play("idle");
                        transitionState = State.Patrol;
                        break;
                    case 1:
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
            animator.Play(hurt1AnimHash, -1, 0);
        }

        protected override void DoAttack()
        {
            enemyMovement.direction.x = 0;
            if (currentTime <= 0.0f)
            {
                animator.Play(attackAnimHash);
                currentTime = attackTime;
            }
            currentTime -= Time.deltaTime;
            if (canAttack)
            {
                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackBox.transform.position, attackBoxArea, 0);
                foreach (Collider2D collider in collider2Ds)
                {
                    if (collider.tag == "Player")
                    {
                        collider.transform.GetChild(1).GetComponent<PlayerStatus>().TakeDamage(attackDamage, DamageType.Melee, transform.rotation);
                    }
                    canAttack = false;
                }
            }
            if (currentTime <= 0.0f)
            {
                currentState = State.Idle;
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

        protected override void DoChase()
        {
            float lookAtPlayerRotationY;
            if(currentTime <= 0)
            {
                lookAtPlayerRotationY = GameManager.instance.playerPhysics.transform.position.x < enemyPhysics.position.x ? 180 : 0;
                enemyPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);
                enemyMovement.direction.x = enemyPhysics.rotation.y == 0 ? 1 : -1;
                currentTime = Random.Range(1.0f, 1.5f);
                animator.Play("patrol");
                //enemyMovement.velocity.x = impPhysics.rotation.y == 0 ? -5 : 5;
            }
            currentTime -= Time.deltaTime;
            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackBox.transform.position, attackBoxArea, 0);
            foreach (Collider2D collider in collider2Ds)
            {
                if (collider.tag == "Player")
                {
                    currentState = State.Attack;
                    currentTime = 0;
                    return;
                }
            }
            if (currentTime <= 0)
            {
                currentState = State.Idle;
            }
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
