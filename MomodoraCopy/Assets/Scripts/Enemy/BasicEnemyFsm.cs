using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public enum EnemyState
    {
        None, Idle, Patrol, Chase, Attack, Interact, Hurt, Die
    }

    public class BasicEnemyFsm : MonoBehaviour
    {
        protected Animator animator;
        protected EnemyStatus enemyStatus;
        protected SpriteRenderer spriteRenderer;
        protected EnemyMovement enemyMovement;
        protected Controller2D controller;


        public EnemyState currentState;       
        [SerializeField]
        protected EnemyState transitionState;
        [SerializeField]
        protected EnemyState temporaryState;

        [HideInInspector]
        public EnemyState none;
        [HideInInspector]
        public EnemyState idle;
        [HideInInspector]
        public EnemyState patrol;
        [HideInInspector]
        public EnemyState chase;
        [HideInInspector]
        public EnemyState attack;
        [HideInInspector]
        public EnemyState interact;
        [HideInInspector]
        public EnemyState hurt;
        [HideInInspector]
        public EnemyState die;

        protected Dictionary<int, float> animTimeDictionary = new Dictionary<int, float>();

        protected float currentTime;
        protected Vector3 playerPosition;
        protected Transform enemyPhysics;

        protected Coroutine fsmRoutine;

        protected virtual void Awake()
        {
            enemyPhysics = transform.parent;

            animator = GetComponent<Animator>();
            enemyStatus = GetComponent<EnemyStatus>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            enemyMovement = enemyPhysics.GetComponent<EnemyMovement>();
            controller = enemyPhysics.GetComponent<Controller2D>();

            currentState = EnemyState.Idle;
            transitionState = EnemyState.Idle;
            temporaryState = EnemyState.None;

            none = EnemyState.None;
            idle = EnemyState.Idle;
            patrol = EnemyState.Patrol;
            chase = EnemyState.Chase;
            attack = EnemyState.Attack;
            interact = EnemyState.Interact;
            hurt = EnemyState.Hurt;
            die = EnemyState.Die;

            SetAnimTimeDictionary();
            CachingAnimation();
        }

        protected virtual void OnEnable()
        {
            currentState = EnemyState.Idle;
            fsmRoutine = StartCoroutine(Fsm());
        }

        protected virtual void OnDisable()
        {
            enemyMovement.direction.x = 0;
            StopCoroutine(fsmRoutine);
        }

        protected virtual void Start()
        {
        }

        protected virtual void CachingAnimation()
        {

        }

        protected void SetAnimTimeDictionary()
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                animTimeDictionary.Add(Animator.StringToHash(clip.name), clip.length);
            }
        }

        protected IEnumerator Fsm()
        {
            yield return null;
            while (true)
            {
                if (enemyStatus.currentHp <= 0)
                {
                    currentState = EnemyState.Die;
                }
                if (temporaryState != EnemyState.None)
                {
                    yield return StartCoroutine(temporaryState.ToString());
                }
                else
                {
                    yield return StartCoroutine(currentState.ToString());
                }
            }
        }


        protected virtual void Update()
        {
            switch (currentState)
            {
                case EnemyState.Idle:
                    DoIdle();
                    break;
                case EnemyState.Patrol:
                    DoPatrol();
                    break;
                case EnemyState.Chase:
                    DoChase();
                    break;
                case EnemyState.Attack:
                    DoAttack();
                    break;
                case EnemyState.Hurt:
                    DoHurt();
                    break;
                case EnemyState.Die:
                    DoDie();
                    break;
                default:
                    break;
            }
        }

        protected virtual void ExecuteState(EnemyState executeState)
        {
            switch (executeState)
            {
                case EnemyState.Idle:
                    DoIdle();
                    break;
                case EnemyState.Patrol:
                    DoPatrol();
                    break;
                case EnemyState.Chase:
                    DoChase();
                    break;
                case EnemyState.Attack:
                    DoAttack();
                    break;
                case EnemyState.Hurt:
                    DoHurt();
                    break;
                case EnemyState.Die:
                    DoDie();
                    break;
                default:
                    break;
            }
        }
        protected virtual void DoIdle()
        {

        }
        protected virtual void DoPatrol()
        {

        }
        protected virtual void DoChase()
        {

        }
        protected virtual void DoAttack()
        {

        }
        protected virtual void DoHurt()
        {

        }
        protected virtual void DoDie()
        {

        }

        //protected virtual IEnumerator Idle()
        //{
        //    yield return null;
        //}
        //protected virtual IEnumerator Patrol()
        //{
        //    yield return null;
        //}
        //protected virtual IEnumerator Chase()
        //{
        //    yield return null;
        //}
        //protected virtual IEnumerator Attack()
        //{
        //    yield return null;
        //}
        //protected virtual IEnumerator Interact()
        //{
        //    yield return null;
        //}
        //protected virtual IEnumerator Hurt()
        //{
        //    yield return null;
        //}
        //protected virtual IEnumerator Die()
        //{
        //    yield return null;
        //}
    }
}
