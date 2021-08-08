using UnityEngine;

namespace MomodoraCopy
{
    public class PlayerState : IState
    {
        protected Player player;
        protected PlayerMovement playerMovement;
        protected PlayerInput playerInput;
        protected ArrowSpawner arrowSpawner;
        protected PoolingObjectInfo info;

        public PlayerState(Player player)
        {
            this.player = player;
            this.playerMovement = player.playerMovement;
            this.playerInput = player.playerInput;
        }
        public virtual void OperateEnter()
        {
#if STATE_DEBUG_MOD
            Debug.Log(this + " 시작");
#endif
        }
        public virtual void OperateUpdate()
        {
#if STATE_DEBUG_MOD
            Debug.Log(this + " 업데이트");
#endif
            if (!player.isAnimationFinished)
            {
                AnimatorStateInfo animationState = player.animator.GetCurrentAnimatorStateInfo(0);
                AnimatorClipInfo[] myAnimatorClip = player.animator.GetCurrentAnimatorClipInfo(0);
                float myTime = myAnimatorClip[0].clip.length * animationState.normalizedTime;
                if(myTime >= myAnimatorClip[0].clip.length)
                {
                    player.isAnimationFinished = true;
                }
            }
        }
        public virtual void OperateExit()
        {
#if STATE_DEBUG_MOD
            Debug.Log(this + " 종료");
#endif
        }
    }
}