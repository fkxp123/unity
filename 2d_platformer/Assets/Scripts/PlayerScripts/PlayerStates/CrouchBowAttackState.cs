using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchBowAttackState : PlayerState
{
    public CrouchBowAttackState(Player player) : base(player)
    {

    }
    public override void OperateEnter()
    {
        base.OperateEnter();
        playerMovement.StopMovement();
        playerInput.StopCheckKey();
        player.isPreAnimationFinished = false;
        playerMovement.animator.Play("crouchBowAttack");
    }
    public override void OperateUpdate()
    {
        base.OperateUpdate();
        playerMovement.velocity.x = 0;
        if (!player.isPreAnimationFinished)
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
        base.OperateExit();
    }
}
