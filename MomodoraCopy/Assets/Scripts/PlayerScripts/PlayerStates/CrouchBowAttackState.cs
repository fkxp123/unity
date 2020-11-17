using UnityEngine;

namespace MomodoraCopy
{
    public class CrouchBowAttackState : PlayerState
    {
        public CrouchBowAttackState(Player player) : base(player)
        {
            arrowSpawner = player.arrowSpawner;
            info = player.arrowSpawner.info;
        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            playerMovement.StopMovement();
            playerInput.StopCheckKey();
            arrowSpawner.SetSpawnerPosition(player.transform.position, arrowSpawner.crouchArrowSpawnerPosition);
            arrowSpawner.arrowRotateZ = arrowSpawner.SetPoolingObjectRotateZ(playerMovement.spriteRenderer.flipX);
            arrowSpawner.OperateSpawn(info, arrowSpawner.arrowRotateZ, ArrowSpawner.ACTIVATE_TIME);
            playerMovement.isPreAnimationFinished = false;
            playerMovement.animator.Play("crouchBowAttack");
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            if (!playerMovement.isPreAnimationFinished)
            {
                return;
            }
            playerMovement.ResetMovement();
            playerInput.ResetCheckKey();
            if (Input.GetKey(KeyCode.DownArrow))
            {
                player.stateMachine.SetState(player.crouch);
                return;
            }
            player.CheckState(playerInput.directionalInput);
        }
        public override void OperateExit()
        {
            playerMovement.ResetMovement();
            playerInput.ResetCheckKey();
            base.OperateExit();
        }
    }

}