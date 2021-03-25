using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class ClimbLadderState : PlayerState
    {
        public ClimbLadderState(Player player) : base(player)
        {
        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            playerMovement.jumpCount = 0;
            player.animator.Play("climbLadder");
            player.animator.enabled = false;
            playerMovement.jumpPosY = playerMovement.transform.position.y;
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();

            if(playerInput.directionalInput.y == 1)
            {
                if (playerMovement.velocity.y == 0)
                {
                    player.animator.enabled = false;
                    return;
                }
                player.animator.enabled = true;
                player.animator.Play("climbLadder");
            }
            else if(playerInput.directionalInput.y == -1)
            {
                player.animator.enabled = true;
                player.animator.Play("climbLadder");
            }
            else
            {
                player.animator.enabled = false;
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
            if (!playerMovement.isLadder
                && playerInput.directionalInput.y == -1)
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
            player.isAnimationFinished = true;
            player.animator.enabled = true;
            playerMovement.isOnLadder = false;
        }
    }

}