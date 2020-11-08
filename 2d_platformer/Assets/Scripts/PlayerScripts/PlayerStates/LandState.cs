using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandState : PlayerState
{
    public LandState(Player player) : base(player)
    {

    }
    bool isLandHard;
    public override void OperateEnter()
    {
        base.OperateEnter();
        //좌/우이동키를 누를 경우에 튕겨져나가는듯한 움직임을 방지
        playerMovement.velocity.x = 0;
        if (playerMovement.highPosY - playerMovement.transform.position.y > 8)
        {
            player.SetStateStun();
            isLandHard = true;
            playerMovement.animator.Play("landingHard");
        }
        else
        {
            player.isPreAnimationFinished = false;
            playerMovement.animator.Play("landingSoft");
        }
    }
    public override void OperateUpdate()
    {
        base.OperateUpdate();
        if (isLandHard)
        {
            if (!player.stopCheckState)
            {
                player.CheckState(player.playerInput.directionalInput);
            }
        }
        else
        {
            if (player.isPreAnimationFinished)
            {
                player.CheckState(player.playerInput.directionalInput);
            }
            else if (playerInput.isKeyDown)
            {
                player.CheckState(player.playerInput.directionalInput);
            }
            else if(playerInput.directionalInput.x != 0)
            {
                player.CheckState(player.playerInput.directionalInput);
            }
        }
    }
    public override void OperateExit()
    {
        base.OperateExit();
    }
}
