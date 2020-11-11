using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBowAttackState : PlayerState
{
    public AirBowAttackState(Player player) : base(player)
    {

    }
    public override void OperateEnter()
    {
        base.OperateEnter();
        playerInput.StopCheckKey();
        if (player.stateMachine.OldState == player.airAttack)
        {
            return;
        }
        player.isPreAnimationFinished = false;
        playerMovement.animator.Play("airBowAttack");
    }
    public override void OperateUpdate()
    {
        base.OperateUpdate();
        if (playerMovement.isGround)
        {
            playerMovement.velocity.x = 0;
        }
        if (player.isPreAnimationFinished)
        {
            playerInput.ResetCheckKey();
            player.CheckState(playerInput.directionalInput);
        }
    }
    public override void OperateExit()
    {
        base.OperateExit();
    }
}
