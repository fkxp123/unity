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
            playerMovement.SetCrouchBoxCollider2D();
            playerMovement.moveType = PlayerMovement.MoveType.StopMove;
            if (player.stateMachine.PreviousState == player.crouchBowAttack)
            {
                return;
            }
            player.isAnimationFinished = false;
            player.animator.Play("preCrouch");
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            if (player.isAnimationFinished)
            {
                player.animator.Play("crouch");
            }
            if (playerInput.IsBowCharging && Input.GetKeyUp(KeyboardManager.instance.BowAttackKey))
            {
                player.stateMachine.SetState(player.crouchBowAttack);
                return;
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.AttackKey))
            {
                player.stateMachine.SetState(player.attack);
                return;
            }
            if(playerMovement.velocity.y < 0)
            {
                player.stateMachine.SetState(player.fall);
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
            playerMovement.SetNormalBoxCollider2D();
            player.isAnimationFinished = false;
            player.animator.Play("breakCrouch");
            playerMovement.moveType = PlayerMovement.MoveType.Normal;
        }
    }
}