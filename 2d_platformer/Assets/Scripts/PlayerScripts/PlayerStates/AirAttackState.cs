using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirAttackState : PlayerState
{
    public AirAttackState(Player player) : base(player)
    {

    }
    public override void OperateEnter()
    {
        base.OperateEnter();
        OperateAirAttack();
    }
    public override void OperateUpdate()
    {
        base.OperateUpdate();
        if (player.isPreAnimationFinished)
        {
            player.CheckState(playerInput.directionalInput);
        }
        if (playerMovement.isGround)
        {
            playerMovement.velocity.x = 0;
        }
    }
    public override void OperateExit()
    {
        base.OperateExit();
    }
    void OperateAirAttack()
    {
        player.isPreAnimationFinished = false;
        playerMovement.animator.Play("airAttack");
        playerMovement.OperateAirAttack();
    }
}
