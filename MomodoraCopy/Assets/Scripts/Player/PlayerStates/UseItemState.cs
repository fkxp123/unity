using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem;

namespace MomodoraCopy
{
    public class UseItemState : PlayerState
    {
        public UseItemState(Player player) : base(player)
        {

        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            player.animator.Play(player.useItemHash);
            player.isAnimationFinished = false;
            player.currentItem = GameManager.instance.slotItems[GameManager.instance.CurrentItemSlotCount] as IUsable;
            playerMovement.moveType = PlayerMovement.MoveType.StopMove;
            playerMovement.stopCheckFlip = true;
            playerMovement.ResetPlayerVelocity();
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            if (player.isAnimationFinished)
            {
                playerMovement.stopCheckFlip = false;
                player.stateMachine.SetState(player.idle);
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();

            if (player.useItemEffect.isPlaying)
            {
                player.useItemEffect.Stop();
            }
            playerMovement.moveType = PlayerMovement.MoveType.Normal;
        }
    }

}