using UnityEngine;

namespace MomodoraCopy
{
    public class JumpState : PlayerState
    {
        public JumpState(Player player) : base(player)
        {

        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            player.breakStepDustEffect.Stop();
            if (playerMovement.isGround)
            {
                playerMovement.SaveJumpPosY();
            }
            playerMovement.jumpCount += 1;
            playerMovement.isAnimationFinished = false;
            if (playerMovement.jumpCount == 1)
            {
                playerMovement.animator.Play("jump");
                playerMovement.OperateJumpKeyDown();
            }
            else if (playerMovement.jumpCount > 1)
            {
                player.doubleJumpEffect.transform.position = playerMovement.transform.position;
                player.doubleJumpEffect.Play();
                playerMovement.animator.Play("jump");
                playerMovement.OperateJumpKeyDown();
            }
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            playerMovement.SaveHighPosY();
            if (Input.GetKeyDown(KeyboardManager.instance.AttackKey))
            {
                player.stateMachine.SetState(player.airAttack);
                return;
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.BowAttackKey))
            {
                player.stateMachine.SetState(player.airBowAttack);
                return;
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.JumpKey))
            {
                if (playerMovement.jumpCount < playerMovement.maxJumpCount)
                {
                    OperateEnter();
                    return;
                }
            }
            else if (playerMovement.velocity.y < 0)
            {
                player.stateMachine.SetState(player.fall);
                return;
            }
            if (Input.GetKeyUp(KeyboardManager.instance.JumpKey))
            {
                playerMovement.OperateJumpKeyUp();
                return;
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
        }
    }

}