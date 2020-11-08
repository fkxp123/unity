using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollState : PlayerState
{
    public RollState(Player player) : base(player)
    {

    }
    public override void OperateEnter()
    {
        base.OperateEnter();
        player.isPreAnimationFinished = true;
        playerInput.StopCheckKey();
        player.StopCheckState();
        playerMovement.animator.Play("roll");
    }
    public override void OperateUpdate()
    {
        base.OperateUpdate();
        playerMovement.OperateRoll();
        if (!player.stopCheckState)
        {
            player.CheckState(playerInput.directionalInput);
        }
    }
    public override void OperateExit()
    {
        base.OperateExit();
        playerInput.ResetCheckKey();
        playerMovement.moveType = PlayerMovement.MoveType.Normal;
    }
}