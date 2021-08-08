using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using InventorySystem;
using UnityEngine.Experimental.Rendering.Universal;

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
        public SpriteRenderer spriteRenderer;

        public Shader shaderGUItext;
        public Shader shaderSpritesDefault;

        Transform playerPhysics;

        public bool isAnimationFinished = true;
        public bool AttackFlag = false; //Animation Event

        public bool isCutScene;

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
        public IState climbLadder;
        public IState pushBlock;
        public IState blownUp;
        public IState talking;
        public IState useItem;
        public IState pray;
        #endregion

        #region AnimHash
        [HideInInspector]public int idleHash;
        [HideInInspector]public int preRunHash;
        [HideInInspector]public int runHash;
        [HideInInspector]public int breakRunHash;
        [HideInInspector]public int landingBlownUpHash;
        [HideInInspector]public int landingSoftHash;
        [HideInInspector]public int landingHardHash;
        [HideInInspector]public int preFallHash;
        [HideInInspector]public int fallHash;
        [HideInInspector]public int climbLadderHash;
        [HideInInspector]public int blownUpHash;
        [HideInInspector]public int hurtHash;
        [HideInInspector]public int doubleJumpHash;
        [HideInInspector]public int jumpHash;
        [HideInInspector]public int pushBlockHash;
        [HideInInspector]public int bowAttackHash;
        [HideInInspector]public int airBowAttackHash;
        [HideInInspector]public int crouchBowAttackHash;
        [HideInInspector]public int airAttackHash;
        [HideInInspector]public int firstAttackHash;
        [HideInInspector]public int secondAttackHash;
        [HideInInspector]public int thirdAttackHash;
        [HideInInspector]public int preCrouchHash;
        [HideInInspector]public int crouchHash;
        [HideInInspector]public int breakCrouchHash;
        [HideInInspector]public int rollHash;
        [HideInInspector]public int preTalkingHash;
        [HideInInspector]public int talkingHash;
        [HideInInspector]public int postTalkingHash;
        [HideInInspector]public int useItemHash;
        [HideInInspector]public int preStandPrayHash;
        [HideInInspector]public int standPrayHash;
        [HideInInspector]public int postStandPrayHash;
        #endregion

        public IUsable currentItem;
        public ParticleSystem useItemEffect;

        bool isFirst = true;

        public bool isPause;

        public GameObject prayEffects;
        public Light2D prayLight;
        public ParticleSystem cycloneEffect;
        public ParticleSystem floatingEffect1;
        public ParticleSystem floatingEffect2;
        ParticleSystem.MainModule cycloneEffectMain;
        ParticleSystem.MainModule floatingEffectMain1;
        ParticleSystem.MainModule floatingEffectMain2;
        Coroutine prayLightRoutine;

        #endregion
        //void OnEnable()
        //{
        //    SceneManager.sceneLoaded += OnSceneLoaded;
        //}
        //void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        //{
        //    enabled = false;
        //    StartCoroutine(ResetEnabled());
        //}
        //void OnDisable()
        //{
        //    SceneManager.sceneLoaded -= OnSceneLoaded;
        //}
        void Start()
        {
            if (isFirst)
            {
                isFirst = false;
            }
            //enabled = false;
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            shaderGUItext = Shader.Find("GUI/Text Shader");
            shaderSpritesDefault = Shader.Find("Sprites/Default");

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
            climbLadder = new ClimbLadderState(this);
            pushBlock = new PushBlockState(this);
            blownUp = new BlownUpState(this);
            talking = new TalkingState(this);
            useItem = new UseItemState(this);
            pray = new PrayState(this);
            #endregion

            stateMachine = new PlayerStateMachine(idle);

            #region Set AnimHash
            idleHash = Animator.StringToHash("idle");
            preRunHash = Animator.StringToHash("preRun");
            runHash = Animator.StringToHash("run");
            breakRunHash = Animator.StringToHash("breakRun");
            landingBlownUpHash = Animator.StringToHash("landingBlownUp");
            landingSoftHash = Animator.StringToHash("landingSoft");
            landingHardHash = Animator.StringToHash("landingHard");
            preFallHash = Animator.StringToHash("preFall");
            fallHash = Animator.StringToHash("fall");
            climbLadderHash = Animator.StringToHash("climbLadder");
            blownUpHash = Animator.StringToHash("blownUp");
            hurtHash = Animator.StringToHash("hurt");
            doubleJumpHash = Animator.StringToHash("doubleJump");
            jumpHash = Animator.StringToHash("jump");
            pushBlockHash = Animator.StringToHash("pushBlock");
            bowAttackHash = Animator.StringToHash("bowAttack");
            airBowAttackHash = Animator.StringToHash("airBowAttack");
            crouchBowAttackHash = Animator.StringToHash("crouchBowAttack");
            airAttackHash = Animator.StringToHash("airAttack");
            firstAttackHash = Animator.StringToHash("firstAttack");
            secondAttackHash = Animator.StringToHash("secondAttack");
            thirdAttackHash = Animator.StringToHash("thirdAttack");
            preCrouchHash = Animator.StringToHash("preCrouch");
            crouchHash = Animator.StringToHash("crouch");
            breakCrouchHash = Animator.StringToHash("breakCrouch");
            rollHash = Animator.StringToHash("roll");
            preTalkingHash = Animator.StringToHash("preTalking");
            talkingHash = Animator.StringToHash("talking");
            postTalkingHash = Animator.StringToHash("postTalking");
            useItemHash = Animator.StringToHash("useItem");
            preStandPrayHash = Animator.StringToHash("preStandPray");
            standPrayHash = Animator.StringToHash("standPray");
            postStandPrayHash = Animator.StringToHash("postStandPray");
            #endregion

            EventManager.instance.AddListener(EventType.GamePause, OnGamePause);
            EventManager.instance.AddListener(EventType.GameResume, OnGameResume);

            cycloneEffectMain = cycloneEffect.main;
            floatingEffectMain1 = floatingEffect1.main;
            floatingEffectMain2 = floatingEffect2.main;
        }

        void OnGamePause()
        {
            isPause = true;
        }
        void OnGameResume()
        {
            isPause = false;
        }

        void Update()
        {
            if (isPause)
            {
                return;
            }
            if (isCutScene)
            {
                return;
            }
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
        public void EnablePlayerInput()
        {
            GameManager.instance.stopPlayerInput = false;
        }
        public void DisablePlayerInput()
        {
            GameManager.instance.stopPlayerInput = true;
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

        public void UseItem()
        {
            if (currentItem != null)
            {
                useItemEffect.Play();
                currentItem.UseItem();
            }
        }

        public void ShowPrayEffects()
        {
            prayEffects.transform.position = transform.position;
            this.RestartCoroutine(FadeInPrayLight(), ref prayLightRoutine);
            cycloneEffect.Play();
            floatingEffect1.Play();
            floatingEffect2.Play();

            cycloneEffectMain.maxParticles = 100;
            floatingEffectMain1.maxParticles = 5;
            floatingEffectMain1.maxParticles = 5;

        }

        public void HidePrayEffects()
        {
            this.RestartCoroutine(FadeOutPrayLight(), ref prayLightRoutine);
            cycloneEffect.Stop();
            floatingEffect1.Stop();
            floatingEffect2.Stop();

            cycloneEffectMain.maxParticles = 0;
            floatingEffectMain1.maxParticles = 0;
            floatingEffectMain1.maxParticles = 0;
        }

        IEnumerator FadeInPrayLight()
        {
            prayLight.gameObject.SetActive(true);
            while(prayLight.intensity < 0.5f)
            {
                prayLight.intensity += Time.deltaTime;
                yield return null;
            }
        }
        IEnumerator FadeOutPrayLight()
        {
            while (prayLight.intensity > 0)
            {
                prayLight.intensity -= Time.deltaTime;
                yield return null;
            }
            prayLight.gameObject.SetActive(false);
        }
    }
}