using UnityEngine;

namespace MomodoraCopy
{
    public class CrouchState : PlayerState
    {
        public CrouchState(Player player) : base(player)
        {

        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            playerMovement.StopMovement();
            if (player.stateMachine.OldState == player.crouchBowAttack)
            {
                return;
            }
            playerMovement.isPreAnimationFinished = false;
            playerMovement.animator.Play("preCrouch");
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            if (playerMovement.isPreAnimationFinished)
            {
                playerMovement.animator.Play("crouch");
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                player.stateMachine.SetState(player.crouchBowAttack);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                player.stateMachine.SetState(player.attack);
            }
            if (playerInput.directionalInput.y != -1)
            {
                playerMovement.ResetMovement();
                player.CheckState(playerInput.directionalInput);
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
            playerMovement.ResetMovement();
            playerMovement.isPreAnimationFinished = false;
            playerMovement.animator.Play("breakCrouch");
        }
    }
}