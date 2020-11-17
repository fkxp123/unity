using UnityEngine;

namespace MomodoraCopy
{
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
                playerMovement.isPreAnimationFinished = false;
                playerMovement.animator.Play("firstAttack");
                playerMovement.OperateFirstAttack();
            }
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            if (playerMovement.isPreAnimationFinished && !comboFlag)
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
            playerMovement.isPreAnimationFinished = true;
            playerMovement.moveType = PlayerMovement.MoveType.Normal;
        }
        void CheckCanComboAttack()
        {
            if (!playerMovement.isPreAnimationFinished && Input.GetKeyDown(KeyCode.S) &&
                !comboFlag && playerMovement.attackCount < playerMovement.maxAttackCount)
            {//need some hangtime?-canInputAttackKey(){if(isPreAnimationFinished)}
                playerMovement.attackCount += 1;
                comboFlag = true;
            }
        }
        void CheckCanFlip()
        {
            if (!playerMovement.isPreAnimationFinished)
            {
                playerMovement.stopCheckFlip = true;
                return;
            }
            playerMovement.stopCheckFlip = false;
        }
        void OperateComboAttack()
        {
            if (playerMovement.isPreAnimationFinished && comboFlag && playerMovement.attackCount == 2)
            {
                comboFlag = false;
                playerMovement.isPreAnimationFinished = false;
                playerMovement.animator.Play("secondAttack");
                playerMovement.OperateSecondAttack();
            }
            if (playerMovement.isPreAnimationFinished && comboFlag && playerMovement.attackCount == 3)
            {
                comboFlag = false;
                playerMovement.isPreAnimationFinished = false;
                playerMovement.animator.Play("thirdAttack");
                playerMovement.OperateThirdAttack();
            }
        }
    }
}