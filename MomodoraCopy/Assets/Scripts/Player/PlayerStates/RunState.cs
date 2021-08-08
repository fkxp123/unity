using UnityEngine;

namespace MomodoraCopy
{
    public class RunState : PlayerState
    {
        public RunState(Player player) : base(player)
        {

        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            if (!player.animator.GetCurrentAnimatorStateInfo(0).IsName("run") &&
                !player.animator.GetCurrentAnimatorStateInfo(0).IsName("preRun") &&
                player.stateMachine.PreviousState != player.attack)
            {
                player.isAnimationFinished = false;
                player.animator.Play(player.preRunHash, -1, 0);
            }
            player.stepDustEffect.Play();
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            if (Input.GetKeyDown(KeyboardManager.instance.JumpKey))
            {
                if (playerMovement.jumpCount < playerMovement.maxJumpCount)
                {
                    player.stateMachine.SetState(player.jump);
                    return;
                }
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.RollKey))
            {
                player.stateMachine.SetState(player.roll);
                return;
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.AttackKey))
            {
                player.stateMachine.SetState(player.attack);
                return;
            }
            else if (playerInput.IsBowCharging && Input.GetKeyUp(KeyboardManager.instance.BowAttackKey))
            {
                player.stateMachine.SetState(player.bowAttack);
                return;
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.UseItemKey))
            {
                player.stateMachine.SetState(player.useItem);
                return;
            }
            else if (playerInput.directionalInput.x == 0)
            {
                player.stateMachine.SetState(player.idle);
                return;
            }
            //else if (playerMovement.velocity.y < 0 && !playerMovement.isGround)
            else if (playerMovement.velocity.y < 0 && Mathf.Abs(playerMovement.velocity.y) > 1f)
            {
                player.stateMachine.SetState(player.fall);
                return;
            }
            if (player.isAnimationFinished)
            {
                player.animator.Play(player.runHash);
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
            if (playerInput.isKeyDown)
            {
                player.isAnimationFinished = true;
                return;
            }
            player.isAnimationFinished = false;
            player.breakStepDustEffect.Play();
            player.animator.Play(player.breakRunHash);
            player.stepDustEffect.Stop();
        }
    }

}