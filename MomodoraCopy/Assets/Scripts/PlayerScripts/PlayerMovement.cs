using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    [RequireComponent(typeof(Controller2D))]
    public class PlayerMovement : MonoBehaviour
    {
        #region Variables
        public bool isGround;
        public bool isFall;
        float gravity;

        public bool stopCheckFlip;
        public bool isAnimationFinished = true;

        public GameObject attackBox;

        //bool stopMoving_X;
        #region Components
        BoxCollider2D boxCollider;
        Controller2D controller;
        PlayerInput playerInput;

        public Animator animator;
        public SpriteRenderer spriteRenderer;
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
        public Vector2 directionalInput
        {
            get { return arrowInput; }
            set
            {
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
                else
                {
                    arrowInput.x = 0;
                    arrowInput.y = 0;
                }
            }
        }
        #endregion

        #region RollState Variables
        float currentRollDelay;
        //bool canRoll = false;
        #endregion

        #region BowAttackState Variables
        public Transform arrowPos;
        public GameObject arrow;
        public bool isLandHard;
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
        bool AttackFlag = false; //Animation Event
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

        Vector2 checkCrushedArea;

        public Vector3 currentVelocity;
        Vector3 previousVelocity;


        #endregion
        void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            controller = GetComponent<Controller2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            boxCollider = GetComponent<BoxCollider2D>();
            playerStatus = GetComponent<PlayerStatus>();

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

            Bounds bounds = boxCollider.bounds;

            bounds.Expand(new Vector2(-0.5f, -1.5f));
            //bounds.Expand(0.015f * -3);
            checkCrushedArea = bounds.size;
        }
        void CheckCrushed()//CheckCrushedByArea & CheckCrushedByTime
        {
            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(transform.position + Vector3.up * 0.22f , checkCrushedArea, 0);
            foreach (Collider2D collider in collider2Ds)
            {
                if (collider.transform.CompareTag("Platform"))
                {
                    playerStatus.CrushedDeath();
                }
            }
        }

        void CalculateCurrentVelocity()
        {
            currentVelocity = (transform.position - previousVelocity) / Time.deltaTime;
            previousVelocity = transform.position;
        }

        void Update()
        {
            CheckCrushed();
            SetDirectionalInput(playerInput.directionalInput);
            SetPlayerMovement();
            CheckVerticalCollision();
            CheckisGround();
            CalculateCurrentVelocity();
        }
        void SetDirectionalInput(Vector2 directionalInput)
        {
            this.directionalInput = directionalInput;
        }
        void SetPlayerMovement()
        {
            CalculateVelocity(moveType);
            controller.Move(velocity * Time.deltaTime, directionalInput);
        }
        void CheckVerticalCollision()
        {
            if (controller.collisions.above || controller.collisions.below)
            {
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

            velocity.y += gravity * Time.deltaTime;

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
            if (!isAnimationFinished)
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
                spriteRenderer.flipX = false;
            }
            else if (directionalInput.x == -1)
            {
                spriteRenderer.flipX = true;
            }
        }

        public void SetPreAnimationFinished()
        {
            isAnimationFinished = true;
        }

        #region LandState Functions
        public void OperateLand()
        {
            velocity.x = 0;
            if (highPosY - transform.position.y > 8)
            {
                animator.Play("landingHard");
            }
            else
            {
                animator.Play("landingSoft");
            }
        }
        #endregion

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

            if (directionalInput.y != -1 || !controller.isThroughPlatform)//
            {
                velocity.y = maxJumpVelocity;
            }
            //else if (jumpCount == 1 && controller.isThroughPlatform)//platform은 통과x throughPlatform은 통과
            //{
            //    velocity.y = maxJumpVelocity;
            //    jumpCount += 1;
            //    animator.Play("doubleJump");
            //}
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
            if (AttackFlag)
            {
                switch (AttackCount)
                {
                    case 1:
                        collider2Ds = Physics2D.OverlapBoxAll(firstAttackPos.position, firstAttackArea, 0);
                        foreach (Collider2D collider in collider2Ds)
                        {
                            if (collider.tag == "Enemy")
                            {
                                collider.GetComponent<EnemyStatus>().TakeDamage(playerStatus.meleeAtk, DamageType.Melee, transform.rotation);
                            }
                            else if (collider.tag == "CheckPoint")
                            {
                                collider.GetComponent<CheckPoint>().SetBellAngle(transform.rotation.y == 0 ? 1 : -1);
                            }
                            AttackFlag = false;
                        }
                        break;
                    case 2:
                        collider2Ds = Physics2D.OverlapBoxAll(secondAttackPos.position, secondAttackArea, 0);
                        foreach (Collider2D collider in collider2Ds)
                        {
                            if (collider.tag == "Enemy")
                            {
                                collider.GetComponent<EnemyStatus>().TakeDamage(playerStatus.meleeAtk, DamageType.Melee, transform.rotation);
                            }
                            else if (collider.tag == "CheckPoint")
                            {
                                collider.GetComponent<CheckPoint>().SetBellAngle(transform.rotation.y == 0 ? 1 : -1);
                            }
                            AttackFlag = false;
                        }
                        break;
                    case 3:
                        collider2Ds = Physics2D.OverlapBoxAll(thirdAttackPos.position, thirdAttackArea, 0);
                        foreach (Collider2D collider in collider2Ds)
                        {
                            if (collider.tag == "Enemy")
                            {
                                collider.GetComponent<EnemyStatus>().TakeDamage(playerStatus.meleeAtk, DamageType.Melee, transform.rotation);
                            }
                            else if (collider.tag == "CheckPoint")
                            {
                                collider.GetComponent<CheckPoint>().SetBellAngle(transform.rotation.y == 0 ? 1 : -1);
                            }
                            AttackFlag = false;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        public void CheckAirAttackArea()
        {
            if (AttackFlag)
            {
                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(airAttackPos.position, airAttackArea, 0);
                foreach (Collider2D collider in collider2Ds)
                {
                    if (collider.tag == "Enemy")
                    {
                        collider.GetComponent<EnemyStatus>().TakeDamage(playerStatus.meleeAtk, DamageType.Melee, transform.rotation);
                    }
                    else if(collider.tag == "CheckPoint")
                    {
                        collider.GetComponent<CheckPoint>().SetBellAngle(transform.rotation.y == 0 ? 1 : -1);
                    }
                    AttackFlag = false;
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
        public void AbleAttack()
        {
            AttackFlag = true;
        }
        public void DisableAttack()
        {
            AttackFlag = false;
        }

        #endregion

        #region BowAttackStateFunctions
        public void BowAttack()
        {
            StartCoroutine("BowAttackCoroutine");
        }
        IEnumerator BowAttackCoroutine()
        {
            velocity.x = 0;
            //stopMoving_X = true;
            if (animator.GetBool("isCrouching"))
            {
                arrowPos.transform.Translate(0, -0.7f, 0);
                animator.Play("crouchBowAttack");
                Instantiate(arrow, arrowPos.position, transform.rotation);
                yield return new WaitForSeconds(0.3f);
                //stopMoving_X = false;
                arrowPos.transform.Translate(0, +0.7f, 0);
            }
            else
            {
                if (animator.GetBool("isGround"))
                {
                    directionalInput = new Vector2(0, 0);
                    animator.Play("bowAttack");
                }
                else if (animator.GetBool("isFalling") || animator.GetBool("doubleJump"))
                {
                    animator.Play("airBowAttack");
                }
                Instantiate(arrow, arrowPos.position, transform.rotation);
                yield return new WaitForSeconds(0.3f);
                //stopMoving_X = false;
            }
        }
        #endregion
        //playerinput에서 directionalinput값을 가져옴
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
            boxCollider.size = new Vector2(1.13f, 2.32f);
        }
        public void SetCrouchBoxCollider2D()
        {
            boxCollider.offset = new Vector2(0.0f, -0.19f);
            boxCollider.size = new Vector2(1.13f, 1.5f);
        }

        void OnDrawGizmos()
        {
            //Gizmos.color = Color.red;
            //Gizmos.DrawWireCube(firstAttackPos.position, firstAttackArea);
            //Gizmos.color = Color.green;
            //Gizmos.DrawWireCube(secondAttackPos.position, secondAttackArea);
            //Gizmos.color = Color.blue;
            //Gizmos.DrawWireCube(thirdAttackPos.position, thirdAttackArea);
            //Gizmos.color = Color.yellow;
            //Gizmos.DrawWireCube(airAttackPos.position, airAttackArea);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 0.22f, checkCrushedArea);
        }
    }

}