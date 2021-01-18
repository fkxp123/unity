using UnityEngine;

namespace MomodoraCopy
{
    public class LandState : PlayerState
    {
        public LandState(Player player) : base(player)
        {

        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            if (playerMovement.isLandHard)
            {
                playerMovement.moveType = PlayerMovement.MoveType.StopMove;
                playerMovement.isAnimationFinished = false;
                playerMovement.animator.Play("landingHard");
                return;
            }
            playerMovement.ResetPlayerVelocity();
            playerMovement.isAnimationFinished = false;
            playerMovement.animator.Play("landingSoft");
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            if (playerMovement.isAnimationFinished)
            {
                player.stateMachine.SetState(player.idle);
                return;
            }
            if(playerMovement.moveType == PlayerMovement.MoveType.StopMove)
            {
                return;
            }
            if (playerInput.isKeyDown)
            {
                player.stateMachine.SetState(player.idle);
                return;
            }
            if(playerInput.directionalInput.y == -1)
            {
                player.stateMachine.SetState(player.crouch);
                return;
            }
            if (playerInput.directionalInput.x != 0)
            {
                player.stateMachine.SetState(player.run);
                return;
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
            playerMovement.isAnimationFinished = true;
            playerMovement.moveType = PlayerMovement.MoveType.Normal;
        }
    }

}