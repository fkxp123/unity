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
            playerMovement.isAnimationFinished = false;
            playerMovement.animator.Play("preRun", -1, 0);
            player.stepDustEffect.Play();
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            if (playerMovement.isAnimationFinished)
            {
                playerMovement.animator.Play("run");
            }
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
            else if (Input.GetKeyDown(KeyboardManager.instance.BowAttackKey))
            {
                player.stateMachine.SetState(player.bowAttack);
                return;
            }
            else if (playerInput.directionalInput.x == 0)
            {
                player.stateMachine.SetState(player.idle);
                return;
            }
            else if (playerMovement.velocity.y < 0)
            {
                player.stateMachine.SetState(player.fall);
                return;
            }
        }
        public override void OperateExit()
        {
            base.OperateExit();
            if (playerInput.isKeyDown)
            {
                playerMovement.isAnimationFinished = true;
                return;
            }
            playerMovement.isAnimationFinished = false;
            player.breakStepDustEffect.Play();
            playerMovement.animator.Play("breakRun");
            player.stepDustEffect.Stop();
        }
    }

}