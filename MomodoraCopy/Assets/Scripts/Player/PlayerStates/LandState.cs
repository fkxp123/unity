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
                //playerMovement.moveType = PlayerMovement.MoveType.StopMove;
                playerMovement.canInput = false;
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
            if(playerMovement.highPosY - playerMovement.transform.position.y > 0.1f)
            {
                player.animator.Play("landingSoft");
            }
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            if (player.isAnimationFinished)
            {
                playerMovement.canInput = true;
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
            if (playerMovement.isLandBlownUp)
            {
                return;
            }
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
            else if (playerInput.IsBowCharging && Input.GetKeyUp(KeyboardManager.instance.BowAttackKey))
            {
                player.stateMachine.SetState(player.bowAttack);
                return;
            }
            if (playerInput.directionalInput.y == -1)
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