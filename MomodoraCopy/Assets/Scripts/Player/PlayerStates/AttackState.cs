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
            playerMovement.AttackCount += 1;
            if (playerMovement.AttackCount == 1)
            {
                player.isAnimationFinished = false;
                player.animator.Play("firstAttack");
                playerMovement.OperateFirstAttack();
            }
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            playerMovement.CheckAttackArea();
            CheckCanComboAttack();
            playerMovement.CheckCanFlip();
            OperateComboAttack();
            if (!player.animator.GetCurrentAnimatorStateInfo(0).IsName("firstAttack") &&
                !player.animator.GetCurrentAnimatorStateInfo(0).IsName("secondAttack") &&
                !player.animator.GetCurrentAnimatorStateInfo(0).IsName("thirdAttack"))
            {
                player.isAnimationFinished = true;
                player.stateMachine.SetState(player.idle);
                return;
            }
            if (player.isAnimationFinished && !comboFlag)
            {
                playerMovement.stopCheckFlip = false;
                player.stateMachine.SetState(player.idle);
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
            playerMovement.AttackCount = 0;
            comboFlag = false;
            playerMovement.moveType = PlayerMovement.MoveType.Normal;
        }
        void CheckCanComboAttack()
        {
            if (!player.isAnimationFinished && Input.GetKeyDown(KeyboardManager.instance.AttackKey) &&
                !comboFlag && playerMovement.AttackCount < playerMovement.maxAttackCount)
            {
                playerMovement.AttackCount += 1;
                comboFlag = true;
            }
        }
        void OperateComboAttack()
        {
            if (player.isAnimationFinished && comboFlag && playerMovement.AttackCount == 2)
            {
                comboFlag = false;
                player.isAnimationFinished = false;
                player.animator.Play("secondAttack");
                playerMovement.OperateSecondAttack();
            }
            if (player.isAnimationFinished && comboFlag && playerMovement.AttackCount == 3)
            {
                comboFlag = false;
                player.isAnimationFinished = false;
                player.animator.Play("thirdAttack");
                playerMovement.OperateThirdAttack();
            }
        }
    }
}