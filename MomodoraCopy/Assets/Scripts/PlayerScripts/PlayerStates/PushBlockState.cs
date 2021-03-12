using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class PushBlockState : PlayerState
    {
        public PushBlockState(Player player) : base(player)
        {
        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            player.animator.Play("pushBlock");
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();

            if (Input.GetKeyDown(KeyboardManager.instance.RollKey))
            {
                player.stateMachine.SetState(player.roll);
                return;
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.AttackKey))
            {
                player.stateMachine.SetState(player.attack);
                return;
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.BowAttackKey))
            {
                player.stateMachine.SetState(player.bowAttack);
                return;
            }
            if (playerInput.directionalInput.x == 0)
            {
                player.stateMachine.SetState(player.idle);
                return;
            }


        }
        public override void OperateExit()
        {
            base.OperateExit();
            player.isAnimationFinished = true;
        }
    }

}