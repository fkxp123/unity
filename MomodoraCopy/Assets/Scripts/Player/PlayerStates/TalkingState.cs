using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class TalkingState : PlayerState
    {
        public TalkingState(Player player) : base(player)
        {

        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            player.isAnimationFinished = false;
            player.animator.Play(Animator.StringToHash("preTalking"));
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            if(player.isAnimationFinished)
            {
                player.animator.Play(Animator.StringToHash("talking"));
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
            player.isAnimationFinished = false;
            player.animator.Play(Animator.StringToHash("postTalking"));
        }
    }

}