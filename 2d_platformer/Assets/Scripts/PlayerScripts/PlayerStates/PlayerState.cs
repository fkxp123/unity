using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : IState
{
    protected Player player;
    protected PlayerMovement playerMovement;
    protected PlayerInput playerInput;

    public PlayerState(Player player)
    {
        this.player = player;
        this.playerMovement = player.playerMovement;
        this.playerInput = player.playerInput;
    }
    public virtual void OperateEnter()
    {
#if STATE_DEBUG_MOD
        Debug.Log(this + " 시작");
#endif
    }
    public virtual void OperateUpdate()
    {
#if STATE_DEBUG_MOD
        Debug.Log(this + " 업데이트");
#endif
    }
    public virtual void OperateExit()
    {
#if STATE_DEBUG_MOD
        Debug.Log(this + " 종료");
#endif
    }
}

