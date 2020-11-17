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
            if (playerMovement.highPosY - playerMovement.transform.position.y > 8)
            {
                playerInput.StopCheckKey();
                playerMovement.StopMovement();
                playerMovement.animator.Play("landingHard");
                return;
            }
            playerMovement.ResetPlayerVelocity();
            playerMovement.isPreAnimationFinished = false;
            playerMovement.animator.Play("landingSoft");
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();

            if (playerMovement.stopMovement)
            {
                return;
            }
            if (playerMovement.isPreAnimationFinished)
            {
                player.CheckState(player.playerInput.directionalInput);
            }
            else if (playerInput.isKeyDown && !playerMovement.stopMovement)
            {
                player.CheckState(player.playerInput.directionalInput);
            }
            else if (playerInput.directionalInput.x != 0 && !playerMovement.stopMovement)
            {
                player.CheckState(player.playerInput.directionalInput);
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
        }
    }

}