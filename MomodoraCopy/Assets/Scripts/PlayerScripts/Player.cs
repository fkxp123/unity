using UnityEngine;
using UnityEngine.SceneManagement;

namespace MomodoraCopy
{
    public class Player : MonoBehaviour
    {
        #region Variables
        public PlayerMovement playerMovement;
        public PlayerInput playerInput;
        public PlayerStatus playerStatus;
        public PlayerStateMachine stateMachine;

        public GameObject arrowSpawnerObject;
        public ArrowSpawner arrowSpawner;

        public ParticleSystem[] landEffect;
        public ParticleSystem doubleJumpEffect;
        public ParticleSystem stepDustEffect;
        public ParticleSystem breakStepDustEffect;

        public Animator animator;
        Transform playerPhysics;

        public bool isAnimationFinished = true;
        public bool AttackFlag = false; //Animation Event

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
        public IState hurt;
        #endregion

        #endregion

        void Start()
        {
            animator = GetComponent<Animator>();

            playerPhysics = transform.parent;
            playerMovement = playerPhysics.GetComponent<PlayerMovement>();
            playerInput = playerPhysics.GetComponent<PlayerInput>();
            playerStatus = GetComponent<PlayerStatus>();

            arrowSpawner = arrowSpawnerObject.GetComponent<ArrowSpawner>();

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
            hurt = new HurtState(this);
            #endregion

            stateMachine = new PlayerStateMachine(idle);
        }

        void Update()
        {
            stateMachine.DoOperateUpdate();
        }

        public void CheckStates(Vector2 input)
        {
            //check player's state in ground
            if (playerMovement.isGround)
            {
                if (stateMachine.CurrentState == fall || stateMachine.CurrentState == jump)
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
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("bowAttack"))
                    {
                        playerInput.isKeyDownBowAttack = false;
                        stateMachine.SetState(bowAttack);
                        return;
                    }
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
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("airBowAttack"))
                    {
                        playerInput.isKeyDownBowAttack = false;
                        stateMachine.SetState(airBowAttack);
                        return;
                    }
                }
                else
                {
                    stateMachine.SetState(fall);
                    return;
                }
            }
        }

        public void SetPreAnimationFinished()
        {
            isAnimationFinished = true;
        }
        public void AbleAttack()
        {
            AttackFlag = true;
        }
        public void DisableAttack()
        {
            AttackFlag = false;
        }

    }
}