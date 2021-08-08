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
            player.animator.Play(player.preTalkingHash);
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            if(player.isAnimationFinished)
            {
                player.animator.Play(player.talkingHash);
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
            player.isAnimationFinished = false;
            player.animator.Play(player.postTalkingHash);
        }
    }

}