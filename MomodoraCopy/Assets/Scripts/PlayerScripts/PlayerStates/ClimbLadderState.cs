using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class ClimbLadderState : PlayerState
    {
        float climbTime;

        public ClimbLadderState(Player player) : base(player)
        {
        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            climbTime = 0.2f;
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            if(playerInput.directionalInput.y == 1)
            {
                player.animator.Play("climbLadder");
            }
            else if(playerInput.directionalInput.y == -1)
            {
                player.animator.Play("climbLadder");
            }
            else
            {
                player.animator.Play("climbLadder", -1, 0);
            }
            if (Input.GetKeyDown(KeyboardManager.instance.JumpKey))
            {
                player.stateMachine.SetState(player.jump);
                return;
            }
            if(playerMovement.isGround)
            {
                player.stateMachine.SetState(player.idle);
                return;
            }
            
            //isground && input arrowkey
            //playermovement.checkladder -> false
            //input jumpkey
        }
        public override void OperateExit()
        {
            base.OperateExit();
            playerMovement.isOnLadder = false;
        }
    }

}