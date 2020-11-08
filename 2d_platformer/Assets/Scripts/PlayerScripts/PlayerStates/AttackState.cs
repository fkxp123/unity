using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : PlayerState
{
    // reset animation speed 1
    // treat frame in animation clip 

    bool comboFlag;
    public AttackState(Player player) : base(player)
    {

    }
    public override void OperateEnter()
    {
        base.OperateEnter();
        playerMovement.attackCount += 1;
        if (playerMovement.attackCount == 1)
        {
            player.isPreAnimationFinished = false;
            playerMovement.animator.Play("firstAttack");
            playerMovement.OperateFirstAttack();
        }
    }
    public override void OperateUpdate()
    {
        base.OperateUpdate();
        if (player.isPreAnimationFinished && !comboFlag)
        {
            playerMovement.stopCheckFlip = false;
            player.CheckState(playerInput.directionalInput);
        }
        CheckCanComboAttack();
        CheckCanFlip();
        OperateComboAttack();
    }
    public override void OperateExit()
    {
        base.OperateExit();
        playerMovement.attackCount = 0;
        player.isPreAnimationFinished = true;
        playerMovement.moveType = PlayerMovement.MoveType.Normal;
    }
    void CheckCanComboAttack()
    {
        if (!player.isPreAnimationFinished && Input.GetKeyDown(KeyCode.S) &&
            !comboFlag && playerMovement.attackCount < playerMovement.maxAttackCount)
        {//need some hangtime?-canInputAttackKey(){if(isPreAnimationFinished)}
            playerMovement.attackCount += 1;
            comboFlag = true;
        }
    }
    void CheckCanFlip()
    {
        if (!player.isPreAnimationFinished)
        {
            playerMovement.stopCheckFlip = true;
            return;
        }
        playerMovement.stopCheckFlip = false;
    }
    void OperateComboAttack()
    {
        if (player.isPreAnimationFinished && comboFlag && playerMovement.attackCount == 2)
        {
            comboFlag = false;
            player.isPreAnimationFinished = false;
            playerMovement.animator.Play("secondAttack");
            playerMovement.OperateSecondAttack();
        }
        if (player.isPreAnimationFinished && comboFlag && playerMovement.attackCount == 3)
        {
            comboFlag = false;
            player.isPreAnimationFinished = false;
            playerMovement.animator.Play("thirdAttack");
            playerMovement.OperateThirdAttack();
        }
    }
}
