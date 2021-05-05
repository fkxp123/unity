using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MomodoraCopy
{
    public class EdeaFsm : BasicBossFsm
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
        int moveAnimHash;
        int attackAnimHash;
        int tailAttackAnimHash;
        int preBreathAnimHash;
        int breathAnimHash;
        int postBreathAnimHash;
        int vomitAnimHash;
        int hurtAnimHash;
        int deathAnimHash;
        WaitForSeconds idleTime;
        WaitForSeconds moveTime;
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

        public ParticleSystem breathEffect;
        public ParticleSystem breathCollisions;

        protected override void Start()
        {
            base.Start();
            waitTime = new WaitForSeconds(coroutineCycle);
            StartCoroutine(Fsm());
        }
        protected override void CachingAnimation()
        {
            idleAnimHash = Animator.StringToHash("idle");
            moveAnimHash = Animator.StringToHash("move");
            attackAnimHash = Animator.StringToHash("attack");
            tailAttackAnimHash = Animator.StringToHash("tailAttack");
            preBreathAnimHash = Animator.StringToHash("preBreath");
            breathAnimHash = Animator.StringToHash("breath");
            postBreathAnimHash = Animator.StringToHash("postBreath");
            vomitAnimHash = Animator.StringToHash("vomit");
            hurtAnimHash = Animator.StringToHash("hurt");
            deathAnimHash = Animator.StringToHash("death");

            idleTime = new WaitForSeconds(animTimeDictionary[idleAnimHash] * 4);
            moveTime = new WaitForSeconds(animTimeDictionary[idleAnimHash]);
            attackTime = new WaitForSeconds(animTimeDictionary[attackAnimHash]);
            tailAttackTime = new WaitForSeconds(animTimeDictionary[tailAttackAnimHash]);
            preBreathTime = new WaitForSeconds(animTimeDictionary[preBreathAnimHash]);
            breathTime = new WaitForSeconds(animTimeDictionary[breathAnimHash] * 12);
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
                        bossPhysics.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                    }
                    else
                    {
                        bossPhysics.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    }
                    currentState = State.Idle;
                }
            }
        }

        IEnumerator Fsm()
        {
            yield return null;
            while (true)
            {
                if (bossStatus.currentHp <= 0)
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
            bossMovement.direction.x = 0;
            //int moveDesicion = random.Next(0, 5);
            int moveDesicion = 1;
            float randomTime = random.Next(2, 3) + (float)random.NextDouble();
            animator.Play(idleAnimHash);

            switch (moveDesicion)
            {
                case 0:
                    while (randomTime > 0)
                    {
                        if (currentState != State.Idle)
                        {
                            randomTime = 0;
                        }
                        yield return waitTime;
                        randomTime -= coroutineCycle;
                    }
                    if (currentState == State.Idle)
                    {
                        int attackDesicion = random.Next(0, 3);
                        switch (attackDesicion)
                        {
                            case 0:
                                currentState = State.Idle;
                                break;
                            case 1:
                                currentState = State.AttackPattern1;
                                break;
                            case 2:
                                currentState = State.AttackPattern2;
                                break;
                            default:
                                currentState = State.Idle;
                                break;
                        }
                    }
                    break;
                case 1:
                    currentState = State.MovePattern1;
                    break;
                case 2:
                    currentState = State.MovePattern2;
                    break;
                case 3:
                    currentState = State.MovePattern3;
                    break;
                case 4:
                    currentState = State.MovePattern4;
                    break;
                default:
                    break;
            }
        }
        IEnumerator MovePattern1()
        {
            float lookAtPlayerRotationY;

            animator.Play(moveAnimHash);
            bossMovement.direction.x = GameManager.instance.playerPhysics.transform.position.x < bossPhysics.position.x ? -1 : 1;
            float randomTime = random.Next(2, 4) + (float)random.NextDouble();
            while (randomTime > 0)
            {
                lookAtPlayerRotationY = GameManager.instance.playerPhysics.transform.position.x < bossPhysics.position.x ? 180 : 0;
                bossPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);
                if (currentState != State.MovePattern1)
                {
                    yield break;
                }
                randomTime -= coroutineCycle;
                yield return waitTime;
            }
            randomTime = random.Next(2, 4) + (float)random.NextDouble();
            bossMovement.direction.x = GameManager.instance.playerPhysics.transform.position.x < bossPhysics.position.x ? 1 : -1;
            while (randomTime > 0)
            {
                lookAtPlayerRotationY = GameManager.instance.playerPhysics.transform.position.x < bossPhysics.position.x ? 180 : 0;
                bossPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);
                if (currentState != State.MovePattern1)
                {
                    yield break;
                }
                randomTime -= coroutineCycle;
                yield return waitTime;
            }
            if (currentState == State.MovePattern1)
            {
                currentState = State.AttackPattern4;
                //int attackDesicion = random.Next(0, 4);
                //switch (attackDesicion)
                //{
                //    case 0:
                //        currentState = State.AttackPattern1;
                //        break;
                //    case 1:
                //        currentState = State.AttackPattern2;
                //        break;
                //    case 2:
                //        currentState = State.AttackPattern3;
                //        break;
                //    case 3:
                //        currentState = State.AttackPattern4;
                //        break;
                //    default:
                //        currentState = State.Idle;
                //        break;
                //}
            }
        }

        IEnumerator AttackPattern1()
        {
            bossMovement.direction.x = 0;
            float lookAtPlayerRotationY;
            lookAtPlayerRotationY = GameManager.instance.playerPhysics.transform.position.x < bossPhysics.position.x ? 180 : 0;
            bossPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);
            float moveToPlayerTime = 1.0f;
            bossMovement.direction.x = bossPhysics.rotation.y == 0 ? 1 : -1;
            while (moveToPlayerTime > 0)
            {
                if (currentState != State.AttackPattern1)
                {
                    yield break;
                }
                moveToPlayerTime -= coroutineCycle;
                yield return waitTime;
            }
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
                if (currentState != State.AttackPattern1)
                {
                    yield break;
                }
                attackTime -= coroutineCycle;
                yield return waitTime;
            }
            if (currentState == State.AttackPattern1)
            {
                currentState = State.Idle;
            }
        }

        IEnumerator AttackPattern2()
        {
            bossMovement.direction.x = 0;
            float lookAtPlayerRotationY;
            lookAtPlayerRotationY = GameManager.instance.playerPhysics.transform.position.x < bossPhysics.position.x ? 180 : 0;
            bossPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);
            bossMovement.direction.x = bossPhysics.rotation.y == 0 ? 1 : -1;
            animator.Play(tailAttackAnimHash);
            float attackTime = animTimeDictionary[tailAttackAnimHash];
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
                if (currentState != State.AttackPattern2)
                {
                    yield break;
                }
                attackTime -= coroutineCycle;
                yield return waitTime;
            }
            if (currentState == State.AttackPattern2)
            {
                currentState = State.Idle;
            }
        }

        IEnumerator AttackPattern3()
        {
            bossMovement.direction.x = 0;
            float lookAtPlayerRotationY;
            lookAtPlayerRotationY = GameManager.instance.playerPhysics.transform.position.x < bossPhysics.position.x ? 180 : 0;
            bossPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);
            bossMovement.direction.x = bossPhysics.rotation.y == 0 ? 1 : -1;
            animator.Play(vomitAnimHash);
            float attackTime = animTimeDictionary[vomitAnimHash];
            while (attackTime > 0)
            {

                if (currentState != State.AttackPattern3)
                {
                    yield break;
                }
                attackTime -= coroutineCycle;
                yield return waitTime;
            }
            if (currentState == State.AttackPattern3)
            {
                currentState = State.Idle;
            }
        }

        IEnumerator AttackPattern4()
        {
            float lookAtPlayerRotationY;
            lookAtPlayerRotationY = GameManager.instance.playerPhysics.transform.position.x < bossPhysics.position.x ? 180 : 0;
            bossPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);

            bossMovement.direction.x = bossPhysics.rotation.y == 0 ? 1 : -1;
            animator.Play(preBreathAnimHash);
            yield return preBreathTime;

            animator.Play(breathAnimHash);
            breathEffect.transform.position = new Vector3(bossPhysics.transform.position.x + 5.3f * (bossPhysics.rotation.y == 0 ? 1 : -1),
                bossPhysics.transform.position.y + 0.41f, bossPhysics.transform.position.z);
            breathCollisions.transform.position = new Vector3(bossPhysics.transform.position.x + 5.3f * (bossPhysics.rotation.y == 0 ? 1 : -1),
                bossPhysics.transform.position.y + 0.41f, bossPhysics.transform.position.z);
            breathEffect.transform.rotation = Quaternion.Euler(breathEffect.transform.rotation.x, 
                (bossPhysics.rotation.y == 0 ? 90 : -90), breathEffect.transform.rotation.z);
            breathCollisions.transform.rotation = Quaternion.Euler(breathEffect.transform.rotation.x,
                (bossPhysics.rotation.y == 0 ? 90 : -90), breathEffect.transform.rotation.z);
            breathEffect.Play();
            breathCollisions.Play();
            bossMovement.direction.x = 0;
            yield return breathTime;
            breathEffect.Stop();
            breathCollisions.Stop();

            animator.Play(postBreathAnimHash);
            yield return postBreathTime;

            if (currentState == State.AttackPattern4)
            {
                currentState = State.Idle;
            }
        }

        //IEnumerator Chase()
        //{
        //    float lookAtPlayerRotationY;
        //    lookAtPlayerRotationY = GameManager.instance.playerPhysics.transform.position.x < bossPhysics.position.x ? 180 : 0;
        //    bossPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);
        //    bossMovement.direction.x = bossPhysics.rotation.y == 0 ? 1 : -1;
        //    float randomTime = 1 + (float)random.NextDouble();
        //    animator.Play(moveAnimHash);
        //    while (randomTime > 0)
        //    {
        //        //Collider2D[] colliders = Physics2D.OverlapBoxAll(attackBox.transform.position, attackBoxArea, 0);
        //        //foreach (Collider2D collider in colliders)
        //        //{
        //        //    if (collider.tag == "Player")
        //        //    {
        //        //        currentState = State.Attack;
        //        //        randomTime = 0;
        //        //    }
        //        //}
        //        //if (currentState != State.Chase)
        //        //{
        //        //    randomTime = 0;
        //        //}
        //        if(Mathf.Abs(bossPhysics.position.x - GameManager.instance.playerPhysics.transform.position.x) < 10f)
        //        {
        //            bossMovement.direction.x = 0;
        //            animator.Play(idleAnimHash);
        //            randomTime = 0;
        //        }
        //        randomTime -= coroutineCycle;
        //        yield return waitTime;
        //    }
        //    if (currentState == State.Chase)
        //    {
        //        //currentState = State.Idle;
        //        currentState = State.Attack;
        //    }
        //}
        //IEnumerator Attack()
        //{
        //    int desicion = random.Next(0, 3);
        //    if(desicion == 0)
        //    {
        //        bossMovement.direction.x = 0;
        //        float lookAtPlayerRotationY;
        //        lookAtPlayerRotationY = GameManager.instance.playerPhysics.transform.position.x < bossPhysics.position.x ? 180 : 0;
        //        bossPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);
        //        float moveToPlayerTime = 1.0f;
        //        bossMovement.direction.x = bossPhysics.rotation.y == 0 ? 1 : -1;
        //        animator.Play(moveAnimHash);
        //        while(moveToPlayerTime > 0)
        //        {
        //            moveToPlayerTime -= coroutineCycle;
        //            yield return waitTime;
        //        }
        //        animator.Play(attackAnimHash);
        //        float attackTime = animTimeDictionary[attackAnimHash];
        //        while (attackTime > 0)
        //        {
        //            if (canAttack)
        //            {
        //                Collider2D[] colliders = Physics2D.OverlapBoxAll(attackBox.transform.position, attackBoxArea, 0);
        //                foreach (Collider2D collider in colliders)
        //                {
        //                    if (collider.tag == "Player")
        //                    {
        //                        collider.transform.GetChild(1).GetComponent<PlayerStatus>().
        //                            TakeDamage(attackDamage, DamageType.Melee, transform.rotation);
        //                    }
        //                    canAttack = false;
        //                }
        //            }
        //            if (currentState != State.Attack)
        //            {
        //                attackTime = 0;
        //            }
        //            attackTime -= coroutineCycle;
        //            yield return waitTime;
        //        }
        //        if (currentState == State.Attack)
        //        {
        //            currentState = State.Chase;
        //        }
        //    }
        //    else if(desicion == 1)
        //    {
        //        bossMovement.direction.x = 0;
        //        float lookAtPlayerRotationY;
        //        lookAtPlayerRotationY = GameManager.instance.playerPhysics.transform.position.x < bossPhysics.position.x ? 180 : 0;
        //        bossPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);
        //        animator.Play(tailAttackAnimHash);
        //        float attackTime = animTimeDictionary[tailAttackAnimHash];
        //        while (attackTime > 0)
        //        {
        //            if (canAttack)
        //            {
        //                Collider2D[] colliders = Physics2D.OverlapBoxAll(attackBox.transform.position, attackBoxArea, 0);
        //                foreach (Collider2D collider in colliders)
        //                {
        //                    if (collider.tag == "Player")
        //                    {
        //                        collider.transform.GetChild(1).GetComponent<PlayerStatus>().
        //                            TakeDamage(attackDamage, DamageType.Melee, transform.rotation);
        //                    }
        //                    canAttack = false;
        //                }
        //            }
        //            if (currentState != State.Attack)
        //            {
        //                attackTime = 0;
        //            }
        //            attackTime -= coroutineCycle;
        //            yield return waitTime;
        //        }
        //        if (currentState == State.Attack)
        //        {
        //            currentState = State.Chase;
        //        }
        //    }
        //    else
        //    {
        //        bossMovement.direction.x = 0;
        //        float lookAtPlayerRotationY;
        //        lookAtPlayerRotationY = GameManager.instance.playerPhysics.transform.position.x < bossPhysics.position.x ? 180 : 0;
        //        bossPhysics.rotation = Quaternion.Euler(0, lookAtPlayerRotationY, 0);
        //        animator.Play(vomitAnimHash);
        //        float attackTime = animTimeDictionary[vomitAnimHash];
        //        while (attackTime > 0)
        //        {
        //            if (canAttack)
        //            {
        //                Collider2D[] colliders = Physics2D.OverlapBoxAll(attackBox.transform.position, attackBoxArea, 0);
        //                foreach (Collider2D collider in colliders)
        //                {
        //                    if (collider.tag == "Player")
        //                    {
        //                        collider.transform.GetChild(1).GetComponent<PlayerStatus>().
        //                            TakeDamage(attackDamage, DamageType.Melee, transform.rotation);
        //                    }
        //                    canAttack = false;
        //                }
        //            }
        //            if (currentState != State.Attack)
        //            {
        //                attackTime = 0;
        //            }
        //            attackTime -= coroutineCycle;
        //            yield return waitTime;
        //        }
        //        if (currentState == State.Attack)
        //        {
        //            currentState = State.Chase;
        //        }
        //    }
        //    float randomTime = random.Next(2, 4) + (float)random.NextDouble();


        //}
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
            bossMovement.direction.x = 0;
            float hurtTime = animTimeDictionary[hurtAnimHash];
            animator.Play(hurtAnimHash, -1, 0);
            currentState = State.Idle;
            while (hurtTime > 0)
            {
                hurtTime -= coroutineCycle;
                yield return waitTime;
                if (currentState == State.Hurt)
                {
                    bossMovement.direction.x = 0;
                    hurtTime = animTimeDictionary[hurtAnimHash];
                    animator.Play(hurtAnimHash, -1, 0);
                    currentState = State.Idle;
                }
            }
        }
        IEnumerator Die()
        {
            bossMovement.direction.x = 0;
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