using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : PlayerState
{
    public RunState(Player player) : base(player)
    {

    }
    public override void OperateEnter()
    {
        base.OperateEnter();
        player.isPreAnimationFinished = false;
        playerMovement.PlayPreRunAnim();
    }
    public override void OperateUpdate()
    {
        base.OperateUpdate();
        if (player.isPreAnimationFinished)
        {
            playerMovement.PlayRunAnim();
        }
        if (playerInput.isKeyDown)
        {
            player.CheckState(player.playerInput.directionalInput);
        }
        if(playerInput.directionalInput.x == 0)
        {
            player.CheckState(player.playerInput.directionalInput);
        }
        if (playerMovement.velocity.y < 0)
        {
            player.CheckState(player.playerInput.directionalInput);
        }
    }
    public override void OperateExit()
    {
        base.OperateExit();
        player.isPreAnimationFinished = false;
        playerMovement.PlayBreakRunAnim();
    }
}
