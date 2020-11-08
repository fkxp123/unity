using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAttackState : PlayerState
{
    public BowAttackState(Player player) : base(player)
    {

    }
    public override void OperateEnter()
    {
        base.OperateEnter();
        playerMovement.StopMovement();
        playerInput.StopCheckKey();
        if(player.stateMachine.OldState == player.attack)
        {
            return;
        }
        player.isPreAnimationFinished = false;
        playerMovement.animator.Play("bowAttack");
    }
    public override void OperateUpdate()
    {
        base.OperateUpdate();
        playerMovement.velocity.x = 0;
        if (player.isPreAnimationFinished)
        {
            playerMovement.ResetMovement();
            playerInput.ResetCheckKey();
            player.CheckState(playerInput.directionalInput);
        }
    }
    public override void OperateExit()
    {
        base.OperateExit();
    }
}