using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class BasicBossFsm : MonoBehaviour
    {
        protected Animator animator;
        protected BossStatus bossStatus;
        protected SpriteRenderer spriteRenderer;
        protected BossMovement bossMovement;
        protected Controller2D controller;

        protected float coroutineCycle = 0.1f;
        protected WaitForSeconds waitTime;

        public enum State
        {
            None, Idle,
            MovePattern1, MovePattern2, MovePattern3, MovePattern4,
            AttackPattern1, AttackPattern2, AttackPattern3, AttackPattern4,
            Hurt, Die
        }

        public State currentState;
        [SerializeField]
        protected State transitionState;
        [SerializeField]
        protected State temporaryState;

        protected Dictionary<int, float> animTimeDictionary = new Dictionary<int, float>();

        protected float currentTime;
        protected Vector3 playerPosition;
        protected Transform bossPhysics;

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {
            bossPhysics = transform.parent;

            animator = GetComponent<Animator>();
            bossStatus = GetComponent<BossStatus>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            bossMovement = bossPhysics.GetComponent<BossMovement>();
            controller = bossPhysics.GetComponent<Controller2D>();

            currentState = State.Idle;
            transitionState = State.Idle;
            //temporaryState = State.None;

            SetAnimTimeDictionary();
            CachingAnimation();

            waitTime = new WaitForSeconds(coroutineCycle);

            StartCoroutine(Fsm());
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

        protected virtual IEnumerator Fsm()
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

        protected virtual IEnumerator Idle()
        {
            yield return null;
        }
        protected virtual IEnumerator MovePattern1()
        {
            yield return null;
        }
        protected virtual IEnumerator MovePattern2()
        {
            yield return null;
        }
        protected virtual IEnumerator MovePattern3()
        {
            yield return null;
        }
        protected virtual IEnumerator MovePattern4()
        {
            yield return null;
        }
        protected virtual IEnumerator AttackPattern1()
        {
            yield return null;
        }
        protected virtual IEnumerator AttackPattern2()
        {
            yield return null;
        }
        protected virtual IEnumerator AttackPattern3()
        {
            yield return null;
        }
        protected virtual IEnumerator AttackPattern4()
        {
            yield return null;
        }
        protected virtual IEnumerator Hurt()
        {
            yield return null;
        }
        protected virtual IEnumerator Die()
        {
            yield return null;
        }
    }
}
