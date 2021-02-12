using UnityEngine;

namespace MomodoraCopy
{
    public class IdleState : PlayerState
    {
        public IdleState(Player player) : base(player)
        {

        }
        public override void OperateEnter()
        {
            base.OperateEnter();
        }
        public override void OperateUpdate()
        {
            if (player.isAnimationFinished)
            {
                player.animator.Play("idle");
            }
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
            else if (playerInput.directionalInput.x != 0)
            {
                player.stateMachine.SetState(player.run);
                return;
            }
            else if (playerInput.directionalInput.y == -1)
            {
                player.stateMachine.SetState(player.crouch);
                return;
            }
            else if (playerMovement.velocity.y < 0)
            {
                player.stateMachine.SetState(player.fall);
                return;
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
        }
    }
}