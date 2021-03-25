using UnityEngine;

namespace MomodoraCopy
{ 
    public class AirBowAttackState : PlayerState
    {
        float airBowPositionY = 0.75f;
        public AirBowAttackState(Player player) : base(player)
        {
            arrowSpawner = player.arrowSpawner;
            info = player.arrowSpawner.info;
        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            if (!player.animator.GetCurrentAnimatorStateInfo(0).IsName("airBowAttack"))
            {
                player.isAnimationFinished = false;
                player.animator.Play("airBowAttack", -1, 0f);
            }
            info.position = new Vector3(player.transform.position.x, player.transform.position.y + airBowPositionY, Random.Range(0.0f, 1.0f));
            info.objectRotation = player.transform.rotation;
            arrowSpawner.OperateSpawn(info, ArrowSpawner.ACTIVATE_TIME);
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            playerMovement.CheckCanFlip();
            if (playerMovement.velocity.y == 0)
            {
                playerMovement.moveType = PlayerMovement.MoveType.StopMove;
                if (playerMovement.isLandHard)
                {
                    player.stateMachine.SetState(player.land);
                    return;
                }
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.JumpKey))
            {
                if (playerMovement.jumpCount < playerMovement.maxJumpCount)
                {
                    player.stateMachine.SetState(player.jump);
                    return;
                }
            }

            if (!player.isAnimationFinished)
            {
                return;
            }
            player.stateMachine.SetState(player.fall);
        }
        public override void OperateExit()
        {
            base.OperateExit();
            player.isAnimationFinished = true;
            playerMovement.stopCheckFlip = false;
            playerMovement.moveType = PlayerMovement.MoveType.Normal;
        }
    }

}