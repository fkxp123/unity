using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class PrayState : PlayerState
    {
        public PrayState(Player player) : base(player)
        {

        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            playerMovement.moveType = PlayerMovement.MoveType.StopMove;
            playerMovement.stopCheckFlip = true;
            playerMovement.ResetPlayerVelocity();
            player.animator.Play(player.preStandPrayHash);
            player.isAnimationFinished = false;
            player.ShowPrayEffects();
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            if (player.isAnimationFinished)
            {
                player.animator.Play(player.standPrayHash);
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
            player.animator.Play(player.postStandPrayHash);
            player.isAnimationFinished = false;
            playerMovement.stopCheckFlip = false;
            playerMovement.moveType = PlayerMovement.MoveType.Normal;
            player.HidePrayEffects();
        }
    }

}