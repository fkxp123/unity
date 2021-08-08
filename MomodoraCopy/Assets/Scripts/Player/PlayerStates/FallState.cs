using UnityEngine;

namespace MomodoraCopy
{
    public class FallState : PlayerState
    {
        public FallState(Player player) : base(player)
        {

        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            player.breakStepDustEffect.Stop();
            if (playerMovement.fallPosY == 0)
            {
                playerMovement.SaveFallPosY();
            }
            //player.animator.Play(player.preFallHash);
            if (player.stateMachine.PreviousState != player.land &&
               !player.animator.GetCurrentAnimatorStateInfo(0).IsName("preFall") &&
               !player.animator.GetCurrentAnimatorStateInfo(0).IsName("fall") &&
               player.stateMachine.PreviousState != player.airAttack)
            {
                player.isAnimationFinished = false;
                player.animator.Play(player.preFallHash);
                //player.animator.Play(player.preFallHash, -1, 0f);
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
            else if (Input.GetKeyDown(KeyboardManager.instance.JumpKey))
            {
                //if (playerMovement.jumpCount < playerMovement.maxJumpCount || playerMovement.isAlmostGround)
                if (playerMovement.jumpCount < playerMovement.maxJumpCount)
                {
                    player.stateMachine.SetState(player.jump);
                    return;
                }
            }
            else if (playerInput.IsBowCharging && Input.GetKeyUp(KeyboardManager.instance.BowAttackKey))
            {
                player.stateMachine.SetState(player.airBowAttack);
                return;
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.RollKey))
            {
                if (!playerMovement.isAlmostGround)
                {
                    return;
                }
                player.stateMachine.SetState(player.roll);
                return;
            }
            if (playerMovement.velocity.y == 0)
            {
                playerMovement.fallPosY = 0;
                player.stateMachine.SetState(player.land);
                return;
            }
            if (player.isAnimationFinished)
            {
                player.animator.Play(player.fallHash);
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
            player.isAnimationFinished = true;
        }
    }

}