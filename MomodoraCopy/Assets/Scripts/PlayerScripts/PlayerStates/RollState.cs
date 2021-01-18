using UnityEngine;

namespace MomodoraCopy
{
    public class RollState : PlayerState
    {
        public RollState(Player player) : base(player)
        {

        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            playerMovement.isAnimationFinished = false;
            playerMovement.animator.Play("roll");
            playerMovement.OperateRoll();
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            playerMovement.CheckCanFlip();
            if (!playerMovement.isAnimationFinished && playerMovement.animator.GetCurrentAnimatorStateInfo(0).IsName("roll"))
            {
                return;
            }
            if (playerMovement.velocity.y < 0)
            {
                player.stateMachine.SetState(player.fall);
                return;
            }
            player.stateMachine.SetState(player.idle);
        }
        public override void OperateExit()
        {
            base.OperateExit();
            playerMovement.stopCheckFlip = false;
            playerMovement.isAnimationFinished = true;
            playerMovement.moveType = PlayerMovement.MoveType.Normal;
        }
    }
}