using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MomodoraCopy
{
    [RequireComponent(typeof(Controller2D))]
    public class PlayerMovement : MonoBehaviour
    {
        static PlayerMovement instance;

        #region Variables
        public bool isGround;
        public bool isFall;
        float gravity;

        public bool stopCheckFlip;

        public GameObject attackBox;

        //bool stopMoving_X;
        #region Components
        BoxCollider2D boxCollider;
        Controller2D controller;
        PlayerInput playerInput;

        //public SpriteRenderer spriteRenderer;
        PlayerStatus playerStatus;
        Enemy enemy;
        #endregion

        #region CalculateVelocity Variables
        public Vector3 velocity;
        float velocityXSmoothing;
        float accelerationTimeAirborne = .2f;
        float accelerationTimeGrounded = .1f;
        float accelerationTimeRoll = .025f;
        public float accelerationTimeAttack = .5f;
        float moveSpeed = 10;
        float FirstAttackSpeed = 2;
        float SecondAttackSpeed = 2;
        float ThirdAttackSpeed = 4;
        float rollSpeed = 18;
        float climbSpeed = 10;

        public enum MoveType
        {
            Normal,
            FirstAttack,
            SecondAttack,
            ThirdAttack,
            Roll,
            StopMove
        }
        MoveType _moveType;
        public MoveType moveType
        {
            get{ return _moveType; }
            set
            {
                _moveType = value;
                if (value == MoveType.Normal)
                {
                    stopCheckFlip = false;
                }
            }
        }
        Dictionary<MoveType, float> MoveTypeDictionary = new Dictionary<MoveType, float>();
        #endregion

        #region Input Variables
        Vector2 arrowInput;
        public bool canInput;
        public Vector2 directionalInput
        {
            get { return arrowInput; }
            set
            {
                arrowInput.x = 0;
                arrowInput.y = 0;
                if (value.x == 1)
                {
                    arrowInput.x = 1;
                    if (!stopCheckFlip)
                    {
                        transform.rotation = Quaternion.Euler(0, 0, 0);
                        //spriteRenderer.flipX = false;
                    }
                }
                else if (value.x == -1)
                {
                    arrowInput.x = -1;
                    if (!stopCheckFlip)
                    {
                        transform.rotation = Quaternion.Euler(0, 180, 0);
                        //spriteRenderer.flipX = true;
                    }
                }
                else if (value.y == 1)
                {
                    arrowInput.y = 1;
                }
                else if (value.y == -1)
                {
                    arrowInput.y = -1;
                }
            }
        }
        #endregion

        #region RollState Variables
        float currentRollDelay;
        //bool canRoll = false;
        #endregion

        #region BowAttackState Variables
        public bool isLandHard;
        public bool isLandBlownUp;
        #endregion

        #region AttackState Variables
        public Transform firstAttackPos;
        public Transform secondAttackPos;
        public Transform thirdAttackPos;
        public Transform airAttackPos;
        
        public Vector2 firstAttackArea;
        public Vector2 secondAttackArea;
        public Vector2 thirdAttackArea;
        public Vector2 airAttackArea;

        [SerializeField]
        int attackCount;
        public int AttackCount
        {
            get{ return attackCount; }
            set
            {
                Mathf.Clamp(value, 0, maxAttackCount);
                attackCount = value;
            }
        }
        public int maxAttackCount = 3;
        public float comboAttackDelay = 0.5f;
        public float currentAttackDelay;

        #endregion

        #region JumpState Variables
        float maxJumpHeight = 4;
        float minJumpHeight = 1;
        float timeToJumpApex = .4f;
        float maxJumpVelocity;
        float minJumpVelocity;
        public float highPosY;
        public float jumpPosY;
        public float fallPosY;
        int _jumpCount;
        public int jumpCount
        {
            get { return _jumpCount; }
            set
            {
                _jumpCount = Mathf.Clamp(value, 0, maxJumpCount);
            }
        }
        public int maxJumpCount = 2;

        #endregion

        #region HandleWallState Variables
        [HideInInspector]
        public Vector2 wallJumpClimb;
        [HideInInspector]
        public Vector2 wallJumpOff;
        [HideInInspector]
        public Vector2 wallLeap;
        float wallSlideSpeedMax = 3;
        float wallStickTime = .25f;
        float timeToWallUnstick;
        bool wallSliding;
        int wallDirX;
        #endregion

        Vector2 crushedArea;

        public Vector3 currentVelocity;
        Vector3 previousVelocity;

        public float maxForcedAngle = 180f;
        float forcedAngle;
        float ForcedAngle
        {
            get { return forcedAngle; }
            set
            {
                forcedAngle = Mathf.Clamp(value, 0.0f, maxForcedAngle);
            }
        }
        float forceDirectionX;
        public float dragCoefficient = 0.4f;
        public bool isForced;
        public bool isStun;

        Transform playerSprite;
        Player player;
        Vector3 forceAcceleration;

        public bool isOnLadder;
        public LayerMask ladderLayer;
        public RaycastHit2D ladderHit;
        public bool isLadder;

        Vector3 checkPushBlockArea;

        void Awake()
        {
            if (instance == null)
            {
                DontDestroyOnLoad(transform.parent.gameObject);
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(transform.parent.gameObject);
            }
        }

        #endregion
        void Start()
        {
            GameManager.instance.Load();
            DontDestroyOnLoad(transform.parent.gameObject);
            
            playerInput = GetComponent<PlayerInput>();
            controller = GetComponent<Controller2D>();
            boxCollider = GetComponent<BoxCollider2D>();

            playerSprite = transform.GetChild(1);
            player = playerSprite.GetComponent<Player>();
            playerStatus = playerSprite.GetComponent<PlayerStatus>();
            //spriteRenderer = playerSprite.GetComponent<SpriteRenderer>();

            gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

            moveType = MoveType.Normal;
            MoveTypeDictionary.Add(MoveType.Normal, moveSpeed);
            MoveTypeDictionary.Add(MoveType.FirstAttack, FirstAttackSpeed);
            MoveTypeDictionary.Add(MoveType.SecondAttack, SecondAttackSpeed);
            MoveTypeDictionary.Add(MoveType.ThirdAttack, ThirdAttackSpeed);
            MoveTypeDictionary.Add(MoveType.Roll, rollSpeed);
            MoveTypeDictionary.Add(MoveType.StopMove, 0);

            canInput = true;

            SetNormalBoxCollider2D();
            SetCrushedArea();
            SetCheckPushBlockArea();
        }
        void SetCrushedArea()
        {
            Bounds bounds = boxCollider.bounds;
            //bounds.Expand(new Vector2(boxCollider.size.x * -0.99f, boxCollider.size.y * -0.99f));
            //bounds.Expand(new Vector2(boxCollider.size.x * -0.5f, boxCollider.size.y * -0.66f));
            bounds.Expand(0.015f * -4);
            crushedArea = bounds.size;
        }
        void SetCheckPushBlockArea()
        {
            Bounds bounds = boxCollider.bounds;
            bounds.Expand(new Vector2(boxCollider.size.x * -0.5f, boxCollider.size.y * -0.75f));
            checkPushBlockArea = bounds.size;
        }
        
        void CheckCrushed()
        {
            Collider2D[] collider2Ds = 
                Physics2D.OverlapBoxAll(boxCollider.bounds.center, crushedArea, 0);
            foreach (Collider2D collider in collider2Ds)
            {
                if (collider.transform.tag == "Platform" || 
                   (collider.transform.tag == "Trap" && collider.gameObject.layer == LayerMask.NameToLayer("Platform")) ||
                   (collider.transform.tag == "PushBlock" && collider.gameObject.layer == LayerMask.NameToLayer("Platform")))
                {
                    playerStatus.CrushedDeath();
                }
            }
            //separate pushblock and trap??
            //Collider2D[] collider2D =
            //    Physics2D.OverlapBoxAll(boxCollider.bounds.center, crushedArea, 0);
            //foreach (Collider2D collider in collider2D)
            //{
            //    if (collider.transform.tag == "Platform" ||
            //       (collider.transform.tag == "Trap" && collider.gameObject.layer == LayerMask.NameToLayer("Platform")) ||
            //       (collider.transform.tag == "PushBlock" && collider.gameObject.layer == LayerMask.NameToLayer("Platform")))
            //    {
            //        playerStatus.CrushedDeath();
            //    }
            //}
        }
        void CheckSpike()
        {
            Collider2D[] collider2Ds =
                Physics2D.OverlapBoxAll(boxCollider.bounds.center, boxCollider.bounds.size, 0);
            foreach (Collider2D collider in collider2Ds)
            {
                if (collider.transform.tag == "Spike")
                {
                    playerStatus.CrushedDeath();
                }
            }
        }
        void CheckPushBlock()
        {
            Collider2D[] collider2Ds =
                Physics2D.OverlapBoxAll(transform.rotation.y == 0 ?
                boxCollider.bounds.center + Vector3.right * 0.25f + Vector3.up * 0.5f:
                boxCollider.bounds.center + Vector3.left * 0.25f + Vector3.up * 0.5f, checkPushBlockArea, 0);
            foreach (Collider2D collider in collider2Ds)
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer("PushBlock"))
                {
                    if(collider.GetComponent<PushBlockController>().velocity.y == 0)
                    {
                        if ((transform.position.x < collider.transform.position.x && playerInput.directionalInput.x > 0) ||
                            (transform.position.x > collider.transform.position.x && playerInput.directionalInput.x < 0))
                        {
                            if(player.stateMachine.CurrentState != player.idle && 
                                player.stateMachine.CurrentState != player.run &&
                                player.stateMachine.CurrentState != player.pushBlock)
                            {
                                return;
                            }
                            player.stateMachine.SetState(player.pushBlock);
                            if(player.stateMachine.CurrentState != player.pushBlock)
                            {
                                return;
                            }
                            if(playerInput.directionalInput.x == 0)
                            {
                                return;
                            }
                            collider.GetComponent<PushBlockController>().direction.x = transform.rotation.y == 0 ? 1 : -1;
                        }
                    }
                }
            }
        }
        void CheckLadder()
        {
            Vector2 ladderRayOrigin = playerInput.directionalInput.y == 1 ?
                (Vector2)boxCollider.bounds.center : new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y);
            float ladderRayLength = 1f;
            float directionY = playerInput.directionalInput.y == -1 ? -1 : 1;
            ladderHit = Physics2D.Raycast(ladderRayOrigin, (isGround ? Vector2.up * directionY : Vector2.up), ladderRayLength, ladderLayer);
            //Debug.DrawRay(ladderRayOrigin, Vector2.up  * ladderRayLength, Color.green);

            if (ladderHit && player.stateMachine.CurrentState != player.climbLadder
                    && player.stateMachine.CurrentState != player.jump
                    && (playerInput.directionalInput.y == 1 || playerInput.directionalInput.y == -1))
            {
                isOnLadder = true;
                player.stateMachine.SetState(player.climbLadder);
                Vector3Int cell = ladderHit.collider.GetComponent<GridLayout>().WorldToCell(transform.position);
                float cellPosX = cell.x + 0.5f;
                
                transform.position = new Vector3(cellPosX, transform.position.y, transform.position.z);
            }
            isLadder = Physics2D.OverlapCircle(transform.position, 0.2f, ladderLayer);
        }

        void CalculateCurrentVelocity()
        {
            currentVelocity = (transform.position - previousVelocity) / Time.deltaTime;
            previousVelocity = transform.position;
        }

        void Update()
        {
            //CheckCrushed();
            //CheckSpike();
            //CheckPushBlock();
            //CheckLadder();
            SetDirectionalInput(playerInput.directionalInput);
            SetPlayerMovement();
            CheckVerticalCollision();
            CheckisGround();
            CalculateCurrentVelocity();
        }
        void FixedUpdate()
        {
            CheckCrushed();
            CheckSpike();
            CheckPushBlock();
            CheckLadder();
        }
        void SetDirectionalInput(Vector2 directionalInput)
        {
            if (isStun)
            {
                this.directionalInput = Vector2.zero;
                return;
            }
            if (!canInput)
            {
                this.directionalInput = Vector2.zero;
                return;
            }
            this.directionalInput = directionalInput;
        }

        void SetPlayerMovement()
        {
            CalculateVelocity(moveType);
            controller.Move(velocity * Time.deltaTime, directionalInput);
        }

        float coefficient;
        float blownUpPercent;

        void CheckVerticalCollision()
        {
            if (isForced)
            {
                ForcedAngle += maxForcedAngle * Time.deltaTime;
                playerSprite.transform.rotation = forceDirectionX == -1 ?
                    Quaternion.Euler(transform.rotation.x, 0, ForcedAngle) :
                    Quaternion.Euler(transform.rotation.x, 180, ForcedAngle);
            }
            if (controller.collisions.above || controller.collisions.below)
            {
                if (isForced)
                {
                    playerSprite.transform.rotation =
                        Quaternion.Euler(0, forceDirectionX == -1 ? 0 : 180, 0);
                }
                velocity.y = 0;
            }
        }
        float SetDirection()
        {
            if (transform.rotation.y == 0)
            {
                return 1;
            }
            return -1;
        }

        void CalculateVelocity(MoveType moveType)
        {
            this.moveType = moveType;
            float targetVelocityX;
            float targetVelocityY;

            if (isOnLadder)
            {
                if (player.stateMachine.CurrentState == player.climbLadder)
                {
                    if (playerInput.directionalInput.y == 1 && !ladderHit)
                    {
                        velocity.y = 0;
                    }
                    else if(player.stateMachine.CurrentState != player.jump)
                    {
                        targetVelocityY = directionalInput.y * climbSpeed;
                        velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityXSmoothing, 0);
                    }
                }
                velocity.x = 0;
                return;
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
            }
            velocity.y = Mathf.Clamp(velocity.y, -50, 50);
            if (isForced)
            {
                float drag;
                drag = -1 * dragCoefficient * forcedVelocity.x * Time.deltaTime;
                if (forceDirectionX == 1)
                {
                    if (velocity.x + drag < 25)
                    {
                        velocity.x = 25;
                        return;
                    }
                }
                else
                {
                    if (velocity.x + drag > -25)
                    {
                        velocity.x = -25;
                        return;
                    }
                }
                velocity.x += drag;
                return;
            }

            if (moveType == MoveType.Normal)
            {
                targetVelocityX = directionalInput.x * MoveTypeDictionary[moveType];
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
                return;
            }
            else if (moveType == MoveType.Roll)
            {
                targetVelocityX = SetDirection() * MoveTypeDictionary[moveType];
                velocity.x = Mathf.SmoothDamp(targetVelocityX, 0, ref velocityXSmoothing, accelerationTimeRoll);
                return;
            }

            velocity.x = SetDirection() * MoveTypeDictionary[moveType];
        }
        Vector3 forcedVelocity;
        public void TakeExplosion(Vector3 explosionPower)
        {
            Debug.Log(explosionPower);
            forcedVelocity = explosionPower;
            velocity = explosionPower;
            coefficient = explosionPower.x;
            forceDirectionX = Mathf.Sign(explosionPower.x);
            player.stateMachine.SetState(player.blownUp);
            isForced = true;
            isLandBlownUp = true;
        }

        void CheckisGround()
        {
            if (controller.collisions.below)
            {
                jumpCount = 0;
                jumpPosY = transform.position.y;
                isGround = true;
                return;
            }
            isGround = false;
        }

        public void CheckCanFlip()
        {
            if (!player.isAnimationFinished)
            {
                stopCheckFlip = true;
                return;
            }
            stopCheckFlip = false;
        }

        public void ResetPlayerVelocity()
        {
            velocity.x = 0;
            velocity.y = 0;
        }
        public void SetSpriteDirection()
        {
            if (directionalInput.x == 1)
            {
                //spriteRenderer.flipX = false;
            }
            else if (directionalInput.x == -1)
            {
                //spriteRenderer.flipX = true;
            }
        }

        #region FallState Functions
        public void SaveFallPosY()
        {
            fallPosY = transform.position.y;
            highPosY = transform.position.y;
        }
        #endregion

        #region JumpState Functions
        public void SaveJumpPosY()
        {
            jumpPosY = transform.position.y;
            highPosY = transform.position.y;
        }
        public void SaveHighPosY()
        {
            if (transform.position.y > highPosY)
            {
                highPosY = transform.position.y;
            }
            if(highPosY - transform.position.y > 8)
            {
                isLandHard = true;
                return;
            }
            isLandHard = false;
        }
        public void OperateJumpKeyDown()
        {
            #region WallSliding
            if (wallSliding)
            {
                if (wallDirX == directionalInput.x)
                {
                    velocity.x = -wallDirX * wallJumpClimb.x;
                    velocity.y = wallJumpClimb.y;
                }
                else if (directionalInput.x == 0)
                {
                    velocity.x = -wallDirX * wallJumpOff.x;
                    velocity.y = wallJumpOff.y;
                }
                else
                {
                    velocity.x = -wallDirX * wallLeap.x;
                    velocity.y = wallLeap.y;
                }
            }
            #endregion

            if (directionalInput.y != -1 || !controller.isThroughPlatform)
            {
                velocity.y = maxJumpVelocity;
            }
        }
        public void OperateJumpKeyUp()
        {
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
        }
        #endregion

        #region RollState Functions
        public void OperateRoll()
        {
            moveType = MoveType.Roll;
        }
        //void CheckRollDelay()
        //{
        //    if (currentRollDelay < 0)
        //    {
        //        canRoll = true;
        //    }
        //    else
        //    {
        //        canRoll = false;
        //        currentRollDelay -= Time.deltaTime;
        //    }
        //}
        //IEnumerator RollCoroutine()
        //{
        //    if (canRoll)
        //    {
        //        currentRollDelay = RollDelay;
        //        playerStatus.noHitMode = true;
        //        yield return new WaitForSeconds(.5f);
        //        isRoll = false;
        //        playerStatus.noHitMode = false;
        //    }
        //}
        #endregion

        #region AttackState Functions
        public void CheckAttackArea()
        {
            Collider2D[] collider2Ds;
            if (player.AttackFlag)
            {
                switch (AttackCount)
                {
                    case 1:
                        collider2Ds = Physics2D.OverlapBoxAll(firstAttackPos.position, firstAttackArea, 0);
                        foreach (Collider2D collider in collider2Ds)
                        {
                            if (collider.tag == "Enemy")
                            {
                                collider.transform.GetChild(0).GetComponent<EnemyStatus>().TakeDamage(playerStatus.meleeAtk, DamageType.Melee, transform.rotation);
                                StartCoroutine(GameManager.instance.mainCameraObject.transform.GetChild(2).GetComponent<CinemachineShake>().Shake(3f, .1f));
                            }
                            else if (collider.tag == "BossHitBox")
                            {
                                collider.transform.parent.GetComponent<BossStatus>().TakeDamage(playerStatus.meleeAtk, DamageType.Melee, transform.rotation);
                                //StartCoroutine(GameManager.instance.mainCameraObject.transform.GetChild(2).GetComponent<CinemachineShake>().Shake(4f, .1f));
                            }
                            else if (collider.tag == "CheckPoint")
                            {
                                collider.GetComponent<CheckPoint>().SetBellAngle(transform.rotation.y == 0 ? 1 : -1);
                            }
                            else if(collider.tag == "Gems")
                            {
                                collider.GetComponent<Rigidbody2D>().AddForce(new Vector3(0.1f * transform.rotation.y == 0 ? 1 : -1, 1f, 0), ForceMode2D.Impulse);
                            }
                            else if(collider.tag == "Chest")
                            {
                                collider.GetComponent<Chest>().OpenChest();
                            }
                            player.AttackFlag = false;
                        }
                        break;
                    case 2:
                        collider2Ds = Physics2D.OverlapBoxAll(secondAttackPos.position, secondAttackArea, 0);
                        foreach (Collider2D collider in collider2Ds)
                        {
                            if (collider.tag == "Enemy")
                            {
                                collider.transform.GetChild(0).GetComponent<EnemyStatus>().TakeDamage(playerStatus.meleeAtk, DamageType.Melee, transform.rotation);
                                StartCoroutine(GameManager.instance.mainCameraObject.transform.GetChild(2).GetComponent<CinemachineShake>().Shake(4f, .1f));
                            }
                            else if (collider.tag == "BossHitBox")
                            {
                                collider.transform.parent.GetComponent<BossStatus>().TakeDamage(playerStatus.meleeAtk, DamageType.Melee, transform.rotation);
                                //StartCoroutine(GameManager.instance.mainCameraObject.transform.GetChild(2).GetComponent<CinemachineShake>().Shake(4f, .1f));
                            }
                            else if (collider.tag == "CheckPoint")
                            {
                                collider.GetComponent<CheckPoint>().SetBellAngle(transform.rotation.y == 0 ? 1 : -1);
                            }
                            else if (collider.tag == "Gems")
                            {
                                collider.GetComponent<Rigidbody2D>().AddForce(new Vector3(0.1f * transform.rotation.y == 0 ? 1 : -1, 1f, 0), ForceMode2D.Impulse);
                            }
                            else if (collider.tag == "Chest")
                            {
                                collider.GetComponent<Chest>().OpenChest();
                            }
                            player.AttackFlag = false;
                        }
                        break;
                    case 3:
                        collider2Ds = Physics2D.OverlapBoxAll(thirdAttackPos.position, thirdAttackArea, 0);
                        foreach (Collider2D collider in collider2Ds)
                        {
                            if (collider.tag == "Enemy")
                            {
                                collider.transform.GetChild(0).GetComponent<EnemyStatus>().TakeDamage(playerStatus.meleeAtk, DamageType.Melee, transform.rotation);
                                StartCoroutine(GameManager.instance.mainCameraObject.transform.GetChild(2).GetComponent<CinemachineShake>().Shake(5f, .1f));
                            }
                            else if (collider.tag == "BossHitBox")
                            {
                                collider.transform.parent.GetComponent<BossStatus>().TakeDamage(playerStatus.meleeAtk, DamageType.Melee, transform.rotation);
                                //StartCoroutine(GameManager.instance.mainCameraObject.transform.GetChild(2).GetComponent<CinemachineShake>().Shake(4f, .1f));
                            }
                            else if (collider.tag == "CheckPoint")
                            {
                                collider.GetComponent<CheckPoint>().SetBellAngle(transform.rotation.y == 0 ? 1 : -1);
                            }
                            else if (collider.tag == "Gems")
                            {
                                collider.GetComponent<Rigidbody2D>().AddForce(new Vector3(0.1f * transform.rotation.y == 0 ? 1 : -1, 1f, 0), ForceMode2D.Impulse);
                            }
                            else if (collider.tag == "Chest")
                            {
                                collider.GetComponent<Chest>().OpenChest();
                            }
                            player.AttackFlag = false;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        public void CheckAirAttackArea()
        {
            if (player.AttackFlag)
            {
                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(airAttackPos.position, airAttackArea, 0);
                foreach (Collider2D collider in collider2Ds)
                {
                    if (collider.tag == "Enemy")
                    {
                        collider.transform.GetChild(0).GetComponent<EnemyStatus>().TakeDamage(playerStatus.meleeAtk, DamageType.Melee, transform.rotation);
                        StartCoroutine(GameManager.instance.mainCameraObject.transform.GetChild(2).GetComponent<CinemachineShake>().Shake(4f, .1f));
                    }
                    else if (collider.tag == "BossHitBox")
                    {
                        collider.transform.parent.GetComponent<BossStatus>().TakeDamage(playerStatus.meleeAtk, DamageType.Melee, transform.rotation);
                        //StartCoroutine(GameManager.instance.mainCameraObject.transform.GetChild(2).GetComponent<CinemachineShake>().Shake(4f, .1f));
                    }
                    else if(collider.tag == "CheckPoint")
                    {
                        collider.GetComponent<CheckPoint>().SetBellAngle(transform.rotation.y == 0 ? 1 : -1);
                    }
                    else if (collider.tag == "Gems")
                    {
                        collider.GetComponent<Rigidbody2D>().AddForce(new Vector3(0.1f * transform.rotation.y == 0 ? 1 : -1, 1f, 0), ForceMode2D.Impulse);
                    }
                    else if (collider.tag == "Chest")
                    {
                        collider.GetComponent<Chest>().OpenChest();
                    }
                    player.AttackFlag = false;
                }   
            }
        }
        public void OperateFirstAttack()
        {
            moveType = MoveType.FirstAttack;
        }
        public void OperateSecondAttack()
        {
            moveType = MoveType.SecondAttack;
        }
        public void OperateThirdAttack()
        {
            moveType = MoveType.ThirdAttack;
        }
        public void OperateAirAttack()
        {

        }

        public void CheckComboAttackDelay()
        {
            if (currentAttackDelay < 0)
            {
                attackCount = 0;
                currentAttackDelay = comboAttackDelay;
            }
            else
            {
                currentAttackDelay -= Time.deltaTime;
            }
        }

        #endregion


        void HandleWallSliding()
        {
            wallDirX = (controller.collisions.left) ? -1 : 1;
            wallSliding = false;
            if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
            {
                wallSliding = true;

                if (velocity.y < -wallSlideSpeedMax)
                {
                    velocity.y = -wallSlideSpeedMax;
                }

                if (timeToWallUnstick > 0)
                {
                    velocityXSmoothing = 0;
                    velocity.x = 0;

                    if (directionalInput.x != wallDirX && directionalInput.x != 0)
                    {
                        timeToWallUnstick -= Time.deltaTime;
                    }
                    else
                    {
                        timeToWallUnstick = wallStickTime;
                    }
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }

            }
        }

        public void SetNormalBoxCollider2D()
        {
            boxCollider.offset = new Vector2(0.0f, 0.22f);
            boxCollider.size = new Vector2(1.0f, 2.32f);
            SetCrushedArea();
        }
        public void SetCrouchBoxCollider2D()
        {
            boxCollider.offset = new Vector2(0.0f, -0.19f);
            boxCollider.size = new Vector2(1.0f, 1.5f);
            SetCrushedArea();
        }

        private void OnDrawGizmos()
        {
            //Gizmos.color = Color.green;
            //Gizmos.DrawWireCube(transform.rotation.y == 0 ?
            //    boxCollider.bounds.center + Vector3.right * 0.25f + Vector3.up * 0.5f :
            //    boxCollider.bounds.center + Vector3.left * 0.25f + Vector3.up * 0.5f, checkPushBlockArea);

            //Gizmos.color = Color.red;
            //Gizmos.DrawWireCube(boxCollider.bounds.center, crushedArea);
        }
    }

}