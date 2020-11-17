namespace MomodoraCopy
{
    public class BowAttackState : PlayerState
    {
        public BowAttackState(Player player) : base(player)
        {
            arrowSpawner = player.arrowSpawner;
            info = player.arrowSpawner.info;
        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            playerMovement.StopMovement();
            playerInput.StopCheckKey();
            arrowSpawner.SetSpawnerPosition(player.transform.position, arrowSpawner.standingArrowSpawnerPosition);
            arrowSpawner.arrowRotateZ = arrowSpawner.SetPoolingObjectRotateZ(playerMovement.spriteRenderer.flipX);
            arrowSpawner.OperateSpawn(info, arrowSpawner.arrowRotateZ, ArrowSpawner.ACTIVATE_TIME);
            playerMovement.isPreAnimationFinished = false;
            playerMovement.animator.Play("bowAttack");
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            if (playerMovement.isPreAnimationFinished)
            {
                playerMovement.ResetMovement();
                playerInput.ResetCheckKey();
                player.CheckState(playerInput.directionalInput);
            }
        }
        public override void OperateExit()
        {
            playerMovement.ResetMovement();
            playerInput.ResetCheckKey();
            base.OperateExit();
        }
    }
}