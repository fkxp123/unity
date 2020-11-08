using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchState : PlayerState
{
    public CrouchState(Player player) : base(player)
    {

    }
    public override void OperateEnter()
    {
        base.OperateEnter();
        playerMovement.StopMovement();
        if(player.stateMachine.OldState == player.crouchBowAttack)
        {
            return;
        }
        player.isPreAnimationFinished = false;
        playerMovement.animator.Play("preCrouch");
    }
    public override void OperateUpdate()
    {
        base.OperateUpdate();
        //좌/우이동키를 누를 경우에 튕겨져나가는듯한 움직임을 방지
        playerMovement.velocity.x = 0;
        if (player.isPreAnimationFinished)
        {
            playerMovement.animator.Play("crouch");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            player.stateMachine.SetState(player.crouchBowAttack);
        }
        if (playerInput.directionalInput.y != -1)
        {
            playerMovement.ResetMovement();
            player.CheckState(playerInput.directionalInput);
        }
    }
    public override void OperateExit()
    {
        base.OperateExit();
        if (player.stateMachine.OldState == player.crouchBowAttack)
        {
            return;
        }
        player.isPreAnimationFinished = false;
        playerMovement.animator.Play("breakCrouch");
    }
}