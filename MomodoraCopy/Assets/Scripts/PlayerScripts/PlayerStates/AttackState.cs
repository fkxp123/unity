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
                playerMovement.isAnimationFinished = false;
                playerMovement.animator.Play("firstAttack");
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
            if (!playerMovement.animator.GetCurrentAnimatorStateInfo(0).IsName("firstAttack") &&
                !playerMovement.animator.GetCurrentAnimatorStateInfo(0).IsName("secondAttack") &&
                !playerMovement.animator.GetCurrentAnimatorStateInfo(0).IsName("thirdAttack"))
            {
                playerMovement.isAnimationFinished = true;
                player.stateMachine.SetState(player.idle);
                return;
            }
            if (playerMovement.isAnimationFinished && !comboFlag)
            {
                playerMovement.stopCheckFlip = false;
                player.stateMachine.SetState(player.idle);
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
            playerMovement.AttackCount = 0;
            playerMovement.moveType = PlayerMovement.MoveType.Normal;
        }
        void CheckCanComboAttack()
        {
            if (!playerMovement.isAnimationFinished && Input.GetKeyDown(KeyboardManager.instance.AttackKey) &&
                !comboFlag && playerMovement.AttackCount < playerMovement.maxAttackCount)
            {
                playerMovement.AttackCount += 1;
                comboFlag = true;
            }
        }
        void OperateComboAttack()
        {
            if (playerMovement.isAnimationFinished && comboFlag && playerMovement.AttackCount == 2)
            {
                comboFlag = false;
                playerMovement.isAnimationFinished = false;
                playerMovement.animator.Play("secondAttack");
                playerMovement.OperateSecondAttack();
            }
            if (playerMovement.isAnimationFinished && comboFlag && playerMovement.AttackCount == 3)
            {
                comboFlag = false;
                playerMovement.isAnimationFinished = false;
                playerMovement.animator.Play("thirdAttack");
                playerMovement.OperateThirdAttack();
            }
        }
    }
}