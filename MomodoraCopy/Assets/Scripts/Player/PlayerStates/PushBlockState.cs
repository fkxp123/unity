using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class PushBlockState : PlayerState
    {
        float pushDirection;
        public PushBlockState(Player player) : base(player)
        {
        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            player.animator.Play("pushBlock");
            pushDirection = playerInput.directionalInput.x;
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            if (Input.GetKeyDown(KeyboardManager.instance.JumpKey))
            {
                if (playerMovement.jumpCount < playerMovement.maxJumpCount)
                {
                    player.stateMachine.SetState(player.jump);
                    return;
                }
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.RollKey))
            {
                player.stateMachine.SetState(player.roll);
                return;
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.AttackKey))
            {
                player.stateMachine.SetState(player.attack);
                return;
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.BowAttackKey))
            {
                player.stateMachine.SetState(player.bowAttack);
                return;
            }

            if (playerInput.directionalInput.x == 0)
            {
                player.stateMachine.SetState(player.idle);
                return;
            }
            else if (playerInput.directionalInput.x != pushDirection)
            {
                player.stateMachine.SetState(player.run);
                return;
            }

            if (playerMovement.velocity.y < 0 && Mathf.Abs(playerMovement.velocity.y) > 1f)
            {
                player.stateMachine.SetState(player.fall);
                return;
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
            player.isAnimationFinished = true;
        }
    }

}