namespace MomodoraCopy
{
    public class RunState : PlayerState
    {
        public RunState(Player player) : base(player)
        {

        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            playerMovement.isPreAnimationFinished = false;
            playerMovement.PlayPreRunAnim();
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            if (playerMovement.isPreAnimationFinished)
            {
                playerMovement.PlayRunAnim();
            }
            if (playerInput.isKeyDown)
            {
                player.CheckState(player.playerInput.directionalInput);
            }
            if (playerInput.directionalInput.x == 0)
            {
                player.CheckState(player.playerInput.directionalInput);
            }
            if (playerMovement.velocity.y < 0)
            {
                player.CheckState(player.playerInput.directionalInput);
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
            playerMovement.isPreAnimationFinished = false;
            playerMovement.PlayBreakRunAnim();
        }
    }

}