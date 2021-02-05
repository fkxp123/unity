namespace MomodoraCopy
{
    public class AirAttackState : PlayerState
    {
        public AirAttackState(Player player) : base(player)
        {

        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            playerMovement.isAnimationFinished = false;
            playerMovement.animator.Play("airAttack");
            playerMovement.OperateAirAttack();
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            if (playerMovement.isGround)
            {
                playerMovement.ResetPlayerVelocity();
            }
            if (playerMovement.isAnimationFinished)
            {
                if(playerMovement.velocity.y == 0)
                {
                    player.stateMachine.SetState(player.idle);
                    return;
                }
                player.stateMachine.SetState(player.fall);
                return;
            }
            playerMovement.CheckAirAttackArea();
            playerMovement.CheckCanFlip();
        }
        public override void OperateExit()
        {
            base.OperateExit();
        }
    }
}