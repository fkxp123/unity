namespace MomodoraCopy
{ 
    public class AirBowAttackState : PlayerState
    {
        public AirBowAttackState(Player player) : base(player)
        {
            arrowSpawner = player.arrowSpawner;
            info = player.arrowSpawner.info;
        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            playerInput.StopCheckKey();
            arrowSpawner.SetSpawnerPosition(player.transform.position, arrowSpawner.airArrowSpawnerPosition);
            arrowSpawner.arrowRotateZ = arrowSpawner.SetPoolingObjectRotateZ(playerMovement.spriteRenderer.flipX);
            arrowSpawner.OperateSpawn(info, arrowSpawner.arrowRotateZ, ArrowSpawner.ACTIVATE_TIME);
            playerMovement.isPreAnimationFinished = false;
            playerMovement.animator.Play("airBowAttack");
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
                player.stateMachine.SetState(player.fall);
            }
        }
        public override void OperateExit()
        {
            playerInput.ResetCheckKey();
            base.OperateExit();
        }
    }

}