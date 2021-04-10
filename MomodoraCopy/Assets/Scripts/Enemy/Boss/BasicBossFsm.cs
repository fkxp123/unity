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

    }
}
