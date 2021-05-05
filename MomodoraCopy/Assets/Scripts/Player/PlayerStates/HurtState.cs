using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class HurtState : PlayerState
    {
        float waitTime;
        float currentTime;

        float hitEffectTime;
        WaitForSeconds hitEffectWaitTime;

        public HurtState(Player player) : base(player)
        {
            waitTime = player.playerStatus.hurtTime;
        }
        public override void OperateEnter()
        {
            base.OperateEnter();
            currentTime = waitTime;
            player.animator.Play("hurt");
            player.spriteRenderer.material.shader = player.shaderGUItext;
            player.spriteRenderer.color = Color.white;
        }
        public override void OperateUpdate()
        {
            base.OperateUpdate();
            currentTime -= Time.deltaTime;
            if(currentTime <= 0)
            {
                player.stateMachine.SetState(player.idle);
                return;
            }
        }
        public override void OperateExit()
        {
            player.spriteRenderer.material.shader = player.shaderSpritesDefault;
            player.spriteRenderer.color = Color.white;
            base.OperateExit();
            player.animator.Play("idle");
        }
    }
}
