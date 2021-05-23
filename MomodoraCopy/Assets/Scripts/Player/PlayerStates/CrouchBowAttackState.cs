using UnityEngine;

namespace MomodoraCopy
{
    public class CrouchBowAttackState : PlayerState
    {
        float crouchBowPositionY = -0.1f;
        public CrouchBowAttackState(Player player) : base(player)
        {
            //arrowSpawner = player.arrowSpawner;
            //info = player.arrowSpawner.info;
        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            playerMovement.SetCrouchBoxCollider2D();

            arrowSpawner = player.arrowSpawner;
            info = player.arrowSpawner.info;

            info.position = new Vector3(player.transform.position.x, player.transform.position.y + crouchBowPositionY, Random.Range(0.0f, 1.0f));
            info.objectRotation = player.transform.rotation;
            arrowSpawner.OperateSpawn(info, ArrowSpawner.ACTIVATE_TIME);
            player.isAnimationFinished = false;
            player.animator.Play("crouchBowAttack");
            playerMovement.moveType = PlayerMovement.MoveType.StopMove;
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            playerMovement.CheckCanFlip();
            if (!player.isAnimationFinished)
            {
                return;
            }
            if (Input.GetKey(KeyboardManager.instance.DownKey))
            {
                player.stateMachine.SetState(player.crouch);
                return;
            }
            player.stateMachine.SetState(player.idle);
        }
        public override void OperateExit()
        {
            base.OperateExit();
            playerMovement.SetNormalBoxCollider2D();
            playerMovement.moveType = PlayerMovement.MoveType.Normal;
        }
    }

}