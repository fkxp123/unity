using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class HurtState : PlayerState
    {
        float waitTime;
        float currentTime;

        public HurtState(Player player) : base(player)
        {
            waitTime = player.playerStatus.hurtTime;
        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            currentTime = waitTime;
            player.animator.Play("hurt");
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            currentTime -= Time.deltaTime;
            if(currentTime <= 0)
            {
                player.stateMachine.SetState(player.idle);
                return;
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
            player.animator.Play("idle");
        }
    }
}
