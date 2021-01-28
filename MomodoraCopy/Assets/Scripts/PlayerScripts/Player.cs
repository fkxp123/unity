using UnityEngine;
using UnityEngine.SceneManagement;

namespace MomodoraCopy
{
    public class Player : MonoBehaviour
    {
        static Player instance;

        #region Variables
        public PlayerMovement playerMovement;
        public PlayerInput playerInput;
        public PlayerStateMachine stateMachine;
        BoxCollider2D boxCollider2D;

        public GameObject arrowSpawnerObject;
        public ArrowSpawner arrowSpawner;

        public ParticleSystem[] landEffect;
        public ParticleSystem doubleJumpEffect;
        public ParticleSystem stepDustEffect;
        public ParticleSystem breakStepDustEffect;

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
        void Awake()
        {
            if (instance == null)
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            //SceneManager.sceneLoaded += OnSceneLoaded;

            playerMovement = GetComponent<PlayerMovement>();
            playerInput = GetComponent<PlayerInput>();
            boxCollider2D = GetComponent<BoxCollider2D>();

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
            #endregion

            GameManager.instance.Load();
            DontDestroyOnLoad(transform.parent.gameObject);
            stateMachine = new PlayerStateMachine(idle);
        }

        //void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        //{
        //    Debug.Log("hi");
        //    GameManager.instance.Load();
        //}

        void Update()
        {
            stateMachine.DoOperateUpdate();
        }

        public void SetNormalBoxCollider2D()
        {
            boxCollider2D.offset = new Vector2(0.0f, 0.22f);
            boxCollider2D.size = new Vector2(1.13f, 2.32f);
        }
        public void SetCrouchBoxCollider2D()
        {
            boxCollider2D.offset = new Vector2(0.0f, -0.19f);
            boxCollider2D.size = new Vector2(1.13f, 1.5f);
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
                    if (!playerMovement.animator.GetCurrentAnimatorStateInfo(0).IsName("bowAttack"))
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
                    if (!playerMovement.animator.GetCurrentAnimatorStateInfo(0).IsName("airBowAttack"))
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
    }
}