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
            playerMovement.moveType = PlayerMovement.MoveType.StopMove;
            if (player.stateMachine.PreviousState == player.crouchBowAttack)
            {
                return;
            }
            playerMovement.isAnimationFinished = false;
            playerMovement.animator.Play("preCrouch");
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            if (playerMovement.isAnimationFinished)
            {
                playerMovement.animator.Play("crouch");
            }
            if (Input.GetKeyDown(KeyboardManager.instance.BowAttackKey))
            {
                player.stateMachine.SetState(player.crouchBowAttack);
                return;
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.AttackKey))
            {
                player.stateMachine.SetState(player.attack);
                return;
            }
            if (playerInput.directionalInput.y != -1)
            {
                player.stateMachine.SetState(player.idle);
                return;
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
            playerMovement.isAnimationFinished = false;
            playerMovement.animator.Play("breakCrouch");
            playerMovement.moveType = PlayerMovement.MoveType.Normal;
        }
    }
}