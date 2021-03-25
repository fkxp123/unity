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
            player.isAnimationFinished = false;
            if(player.stateMachine.PreviousState != player.land ||
               !player.animator.GetCurrentAnimatorStateInfo(0).IsName("preFall") ||
               !player.animator.GetCurrentAnimatorStateInfo(0).IsName("fall"))
            {
                player.animator.Play("preFall", -1, 0f);
            }
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            playerMovement.SaveHighPosY();
            if (player.isAnimationFinished)
            {
                player.animator.Play("fall");
            }
            if (Input.GetKeyDown(KeyboardManager.instance.AttackKey))
            {
                player.stateMachine.SetState(player.airAttack);
                return;
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.JumpKey))
            {
                if (playerMovement.jumpCount < playerMovement.maxJumpCount)
                {
                    player.stateMachine.SetState(player.jump);
                    return;
                }
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.BowAttackKey))
            {
                player.stateMachine.SetState(player.airBowAttack);
                return;
            }
            if (playerMovement.velocity.y == 0)
            {
                playerMovement.fallPosY = 0;
                player.stateMachine.SetState(player.land);
                return;
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
            player.isAnimationFinished = true;
        }
    }

}