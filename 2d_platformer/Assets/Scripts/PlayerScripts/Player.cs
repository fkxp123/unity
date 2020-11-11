using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Player : MonoBehaviour
{
    #region Variables
    public PlayerMovement playerMovement;
    public PlayerInput playerInput;
    public PlayerStateMachine stateMachine;

    public bool stopAllInput;
    public bool stopCheckState;
    public bool isPreAnimationFinished = true;

    #region IState
    public IState idle;
    public IState crouch;
    public IState run;
    public IState jump;
    public IState fall;
    public IState roll;
    public IState attack;
    public IState bowAttack;
    public IState land;
    public IState airAttack;
    public IState crouchBowAttack;
    public IState airBowAttack;
    #endregion

    #endregion

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();

        #region State Instances
        idle = new IdleState(this);
        crouch = new CrouchState(this);
        run = new RunState(this);
        jump = new JumpState(this);
        fall = new FallState(this);
        roll = new RollState(this);
        attack = new AttackState(this);
        bowAttack = new BowAttackState(this);
        land = new LandState(this);
        airAttack = new AirAttackState(this);
        crouchBowAttack = new CrouchBowAttackState(this);
        airBowAttack = new AirBowAttackState(this);
        #endregion

        stateMachine = new PlayerStateMachine(idle);
    }

    void Update()
    {
        stateMachine.DoOperateUpdate();
    }

    public void CheckState(Vector2 input)
    {
        //check player's state in ground
        if (playerMovement.isGround)
        {
            playerMovement.jumpCount = 0;
            if (stateMachine.CurState == fall || stateMachine.CurState == jump)
            {
                stateMachine.SetState(land);
                return;
            }
            //-------------------------------------
            //CheckState by PlayerInput.CheckKeyDown
            else if (playerInput.isKeyDownJump)
            {
                playerInput.isKeyDownJump = false;
                if (playerMovement.jumpCount < playerMovement.maxJumpCount)
                {
                    stateMachine.SetState(jump);
                    return;
                }
            }
            else if (playerInput.isKeyDownRoll)
            {
                playerInput.isKeyDownRoll = false;
                stateMachine.SetState(roll);
                return;
            }
            else if (playerInput.isKeyDownAttack)
            {
                playerInput.isKeyDownAttack = false;
                stateMachine.SetState(attack);
                return;
            }
            else if (playerInput.isKeyDownBowAttack)
            {
                playerInput.isKeyDownBowAttack = false;
                stateMachine.SetState(bowAttack);
                return;
            }
            //-------------------------------------
            //CheckState by PlayerInput.CheckArrowKeyDown
            else if (input.x != 0)
            {
                stateMachine.SetState(run);
                return;
            }
            else if (input.y == -1)
            {
                stateMachine.SetState(crouch);
                return;
            }
            else if (input.x == 0)
            {
                stateMachine.SetState(idle);
                return;
            }
            //-------------------------------------
        }
        else //check player's state in air
        {
            if (playerInput.isKeyDownJump)
            {
                playerInput.isKeyDownJump = false;
                if (playerMovement.jumpCount < playerMovement.maxJumpCount)
                {
                    stateMachine.SetState(jump);
                    return;
                }
            }
            else if (playerInput.isKeyDownAttack)
            {
                playerInput.isKeyDownAttack = false;
                stateMachine.SetState(airAttack);
                return;
            }
            else if (playerInput.isKeyDownBowAttack)
            {
                playerInput.isKeyDownBowAttack = false;
                stateMachine.SetState(airBowAttack);
            }
            else
            {
                stateMachine.SetState(fall);
                return;
            }
        }
    }    

    public void StopCheckState()
    {
        stopCheckState = true;
    }
    public void ResetCheckState()
    {
        stopCheckState = false;
    }
    

    public void SetStateStun()
    {
        playerInput.StopCheckKey();
        playerMovement.StopMovement();
        StopCheckState();
    }
    public void SetStateNormal()
    {
        playerInput.ResetCheckKey();
        playerMovement.ResetMovement();
        ResetCheckState();
    }
    public void SetPreAnimationFinished()
    {
        isPreAnimationFinished = true;
    }
}