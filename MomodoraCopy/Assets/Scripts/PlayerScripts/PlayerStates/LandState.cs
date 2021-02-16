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
            foreach(ParticleSystem p in player.landEffect)
            {
                p.Play();
            }
            //player.landEffect.Play();
            if (playerMovement.isLandBlownUp)
            {
                playerMovement.moveType = PlayerMovement.MoveType.StopMove;
                playerMovement.stopCheckFlip = true;
                player.isAnimationFinished = false;
                player.animator.Play("landingBlownUp");
                return;
            }
            if (playerMovement.isLandHard)
            {
                playerMovement.moveType = PlayerMovement.MoveType.StopMove;
                playerMovement.stopCheckFlip = true;
                player.isAnimationFinished = false;
                player.animator.Play("landingHard");
                return;
            }
            playerMovement.ResetPlayerVelocity();
            player.isAnimationFinished = false;
            player.animator.Play("landingSoft");
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            if (player.isAnimationFinished)
            {
                playerMovement.stopCheckFlip = false;
                playerMovement.velocity.x = 0;
                playerMovement.isLandBlownUp = false;
                playerMovement.isLandHard = false;
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
            player.isAnimationFinished = true;
            playerMovement.moveType = PlayerMovement.MoveType.Normal;
        }
    }

}