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
            playerMovement.SaveJumpPosY();
            playerMovement.jumpCount += 1;
            playerMovement.isPreAnimationFinished = false;
            if (playerMovement.jumpCount == 1)
            {
                playerMovement.animator.Play("jump");
                playerMovement.OperateJumpKeyDown();
            }
            else if (playerMovement.jumpCount > 1)
            {
                playerMovement.animator.Play("doubleJump");
                playerMovement.OperateJumpKeyDown();
            }
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            playerMovement.SaveHighPosY();
            if (playerMovement.velocity.y < 0)
            {
                player.CheckState(player.playerInput.directionalInput);
            }
            if (playerInput.isKeyDown)
            {
                player.CheckState(player.playerInput.directionalInput);
            }
            if (Input.GetKeyUp(KeyboardManager.instance.JumpKey))
            {
                playerMovement.OperateJumpKeyUp();
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
        }
    }

}