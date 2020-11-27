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
            OperateAirAttack();
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            if (playerMovement.isGround)
            {
                playerMovement.ResetPlayerVelocity();
            }
            if (playerMovement.isPreAnimationFinished)
            {
                playerInput.ResetCheckKey();
                player.CheckState(playerInput.directionalInput);
            }
        }
        public override void OperateExit()
        {
            playerInput.ResetCheckKey();
            base.OperateExit();
        }
        void OperateAirAttack()
        {
            playerInput.StopCheckKey();
            playerMovement.isPreAnimationFinished = false;
            playerMovement.animator.Play("airAttack");
            playerMovement.OperateAirAttack();
        }
    }
}