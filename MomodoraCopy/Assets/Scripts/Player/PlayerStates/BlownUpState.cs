using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class BlownUpState : PlayerState
    {
        float waitTime;
        float currentTime;

        public BlownUpState(Player player) : base(player)
        {

        }
        public override void OperateEnter()
        {
            waitTime = player.playerStatus.hurtTime;
            currentTime = waitTime;
            base.OperateEnter();
            player.animator.Play("blownUp");
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            currentTime -= Time.deltaTime;
            if(playerMovement.velocity.y == 0)
            {
                player.stateMachine.SetState(player.land);
                return;
            }
            if (currentTime <= 0)
            {
                //player.stateMachine.SetState(player.idle);
                return;
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
            playerMovement.isForced = false;
        }
    }
}
