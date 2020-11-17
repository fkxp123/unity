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
            playerMovement.isPreAnimationFinished = false;
            playerInput.StopCheckKey();
            playerMovement.animator.Play("roll");
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            playerMovement.OperateRoll();
            if (playerMovement.isPreAnimationFinished)
            {
                playerInput.ResetCheckKey();
                player.CheckState(playerInput.directionalInput);
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
            playerInput.ResetCheckKey();
            playerMovement.moveType = PlayerMovement.MoveType.Normal;
        }
    }
}