﻿using UnityEngine;

namespace MomodoraCopy
{
    public class BowAttackState : PlayerState
    {
        float bowPositionY = 0.65f;
        bool isBowAttackKeyDown;
        public BowAttackState(Player player) : base(player)
        {
            //arrowSpawner = player.arrowSpawner;
            //info = player.arrowSpawner.info;
        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            player.isAnimationFinished = false;
            player.animator.Play("bowAttack", -1, 0f);

            arrowSpawner = player.arrowSpawner;
            info = player.arrowSpawner.info;

            playerMovement.moveType = PlayerMovement.MoveType.StopMove;
            playerMovement.stopCheckFlip = true;
            OperateBowAttack();
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            playerMovement.CheckCanFlip();
            playerInput.directionalInput.x = 0;
            if (!player.isAnimationFinished)
            {
                return;
            }
            player.stateMachine.SetState(player.idle);
        }
        public override void OperateExit()
        {
            base.OperateExit();
            player.isAnimationFinished = true;
            playerMovement.stopCheckFlip = false;
            playerMovement.moveType = PlayerMovement.MoveType.Normal;
        }
        void OperateBowAttack()
        {
            if (playerInput.MaxBowCharged)
            {
                info.position = new Vector3(player.transform.position.x, player.transform.position.y + bowPositionY, Random.Range(0.0f, 1.0f));
                info.objectRotation = player.transform.rotation;
                arrowSpawner.OperateDynamicSpawn(info, ArrowSpawner.ACTIVATE_TIME);
                info.objectRotation = Quaternion.Euler(player.transform.rotation.x, player.transform.rotation.y, 
                    player.transform.rotation.y == 0 ? 15f : 195f);
                arrowSpawner.OperateDynamicSpawn(info, ArrowSpawner.ACTIVATE_TIME);
                info.objectRotation = Quaternion.Euler(player.transform.rotation.x, player.transform.rotation.y, 
                    player.transform.rotation.y == 0 ? -15f : 165f);
                arrowSpawner.OperateDynamicSpawn(info, ArrowSpawner.ACTIVATE_TIME);
                return;
            }
            info.position = new Vector3(player.transform.position.x, player.transform.position.y + bowPositionY, Random.Range(0.0f, 1.0f));
            info.objectRotation = player.transform.rotation;
            arrowSpawner.OperateDynamicSpawn(info, ArrowSpawner.ACTIVATE_TIME);
        }

        void CheckSomething()
        {
            if (player.animator.GetCurrentAnimatorStateInfo(0).IsName("bowAttack"))
            {
                player.stateMachine.SetState(player.idle);
                return;
            }
        }
    }
}