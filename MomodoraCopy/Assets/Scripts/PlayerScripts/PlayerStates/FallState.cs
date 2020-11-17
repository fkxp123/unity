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
            playerMovement.SaveFallPosY();
            playerMovement.isPreAnimationFinished = false;
            playerMovement.animator.Play("preFall");
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            playerMovement.SaveHighPosY();
            if (playerMovement.isPreAnimationFinished)
            {
                playerMovement.animator.Play("fall");
            }
            if (playerInput.isKeyDown)
            {
                player.CheckState(playerInput.directionalInput);
            }
            if (playerMovement.velocity.y == 0)
            {
                player.CheckState(playerInput.directionalInput);
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
        }
    }

}