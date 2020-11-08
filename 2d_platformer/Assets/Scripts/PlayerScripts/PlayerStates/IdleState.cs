using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(Player player) : base(player)
    {

    }
    public override void OperateEnter()
    {
        base.OperateEnter();
    }
    public override void OperateUpdate()
    {
        if (player.isPreAnimationFinished)
        {
            playerMovement.animator.Play("idle");
        }
        base.OperateUpdate();
        player.CheckState(player.playerInput.directionalInput);
    }
    public override void OperateExit()
    {
        base.OperateExit();
    }
}