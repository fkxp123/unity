using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class BasicEnemyFsm : MonoBehaviour
    {
        protected Animator animator;
        protected EnemyStatus enemyStatus;
        protected SpriteRenderer spriteRenderer;
        protected EnemyMovement enemyMovement;
        protected Controller2D controller;

        public enum State
        {
            None, Idle, Patrol, Chase, Attack, Interact, Hurt, Die
        }

        public State currentState;       
        [SerializeField]
        protected State transitionState;
        [SerializeField]
        protected State temporaryState;

        [HideInInspector]
        public State none;
        [HideInInspector]
        public State idle;
        [HideInInspector]
        public State patrol;
        [HideInInspector]
        public State chase;
        [HideInInspector]
        public State attack;
        [HideInInspector]
        public State interact;
        [HideInInspector]
        public State hurt;
        [HideInInspector]
        public State die;

        protected Dictionary<int, float> animTimeDictionary = new Dictionary<int, float>();

        protected float currentTime;
        protected Vector3 playerPosition;
        protected Transform enemyPhysics;

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {
            enemyPhysics = transform.parent;

            animator = GetComponent<Animator>();
            enemyStatus = GetComponent<EnemyStatus>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            enemyMovement = enemyPhysics.GetComponent<EnemyMovement>();
            controller = enemyPhysics.GetComponent<Controller2D>();

            currentState = State.Idle;
            transitionState = State.Idle;
            temporaryState = State.None;

            none = State.None;
            idle = State.Idle;
            patrol = State.Patrol;
            chase = State.Chase;
            attack = State.Attack;
            interact = State.Interact;
            hurt = State.Hurt;
            die = State.Die;

            SetAnimTimeDictionary();
            CachingAnimation();
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

        protected virtual void Update()
        {
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

        protected virtual void ExecuteState(State executeState)
        {
            switch (executeState)
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
    }
}
