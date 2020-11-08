using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class PlayerMovement : MonoBehaviour
{
    #region Variables
    public bool isGround;
    float gravity;

    public bool stopMovement;
    public bool stopCheckFlip;

    //bool stopMoving_X;
    #region Components
    PlayerInput playerInput;
    Controller2D controller;
    public Animator animator;
    BoxCollider2D boxCollider;
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
        Roll
    }
    MoveType _moveType;
    public MoveType moveType
    {
        get
        {
            return _moveType;
        }
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
        get{ return arrowInput; }
        set
        {
            if(value.x == 1)
            {
                arrowInput.x = 1;
                if(!stopCheckFlip)
                {
                    spriteRenderer.flipX = false;
                }
            }
            else if(value.x == -1)
            {
                arrowInput.x = -1;
                if (!stopCheckFlip)
                {
                    spriteRenderer.flipX = true;
                }
            }
            else if(value.y == 1)
            {
                arrowInput.y = 1;
            }
            else if(value.y == -1)
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
    #endregion

    #region AttackState Variables
    public Transform attackPos;
    public Vector2 meleeBoxSize;
    public int attackCount;
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
    int _jumpCount;
    public float highPosY;
    [SerializeField]
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

    #endregion
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<Controller2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        enemy = Enemy.instance;
        playerStatus = PlayerStatus.instance;
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        moveType = MoveType.Normal;
        MoveTypeDictionary.Add(MoveType.Normal, moveSpeed);
        MoveTypeDictionary.Add(MoveType.FirstAttack, FirstAttackSpeed);
        MoveTypeDictionary.Add(MoveType.SecondAttack, SecondAttackSpeed);
        MoveTypeDictionary.Add(MoveType.ThirdAttack, ThirdAttackSpeed);
        MoveTypeDictionary.Add(MoveType.Roll, rollSpeed);
    }
    void Update()
    {
        CheckGround();
        //CheckAllDelay();
        SetDirectionalInput(playerInput.directionalInput);
        if (!stopMovement)
        {
            CalculateVelocity(moveType);
            controller.Move(velocity * Time.deltaTime, directionalInput);
        }
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
    }

    public void StopMovement()
    {
        stopMovement = true;
    }
    public void ResetMovement()
    {
        stopMovement = false;
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

    public void CheckGround()
    {
        if (controller.collisions.below)
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }
    }

    //void CheckAllDelay()
    //{
    //    //CheckRollDelay();
    //    //CheckComboAttackDelay();
    //}

    public void StopMovingX()
    {
        //stopMoving_X = true;
    }

    #region RunState Functions
    public void PlayPreRunAnim()
    {
        animator.Play("preRun");
    }
    public void PlayRunAnim()
    {
        animator.Play("run");
    }
    public void PlayBreakRunAnim()
    {
        animator.Play("breakRun");
    }
    #endregion


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
        highPosY = transform.position.y;
    }
    #endregion

    #region JumpState Functions
    public void SaveJumpPosY()//점프한 y값 저장
    {
        //점프키를 눌렀을때만 호출
        highPosY = transform.position.y;
    }
    public void SaveHighPosY()//공중에 있는도중 가장 높은 y값 저장
    {
        //update문에서 계속 최고점의 y값으로 설정함
        if (transform.position.y > highPosY)
        {
            highPosY = transform.position.y;
        }
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

        if(directionalInput.y != -1 || !controller.isThroughPlatform)//
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
    public void AbleAttackBox()
    {
        AttackFlag = true;
    }
    public void DisableAttackBox()
    {
        AttackFlag = false;
    }
    public void AttackBox()//Animation event
    {
        bool flag = false;
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackPos.position, meleeBoxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.tag == "enemy")
            {
                flag = true;
            }
        }
        if (flag)
        {
            enemy.HitbyPlayer(5);
        }
    }
    IEnumerator AttackCoroutine()
    {
        if (AttackFlag)
        {
            AttackBox();
        }
        if (animator.GetBool("isGround"))
        {
            animator.Play("attack");
        }
        else if (animator.GetBool("isFalling") || animator.GetBool("isJumping"))
        {
            //stopMoving_X = false;
            //stopAllMove = false;
            animator.Play("airAttack");
        }
        yield return new WaitForSeconds(.5f);
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
    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }
    public float SetDirection()
    {
        if (spriteRenderer.flipX)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }
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
    void CalculateVelocity(MoveType moveType)
    {
        this.moveType = moveType;
        float targetVelocityX;

        if (moveType == MoveType.Normal)
        {
            targetVelocityX = directionalInput.x * MoveTypeDictionary[moveType];
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        }
        else if (moveType == MoveType.Roll)
        {
            targetVelocityX = SetDirection() * MoveTypeDictionary[moveType];
            velocity.x = Mathf.SmoothDamp(targetVelocityX, 0, ref velocityXSmoothing, accelerationTimeRoll);
        }
        else //attack moveType
        {
            //targetVelocityX = SetDirection() * MoveTypeDictionary[moveType];
            //velocity.x = Mathf.SmoothDamp(targetVelocityX, 0, ref velocityXSmoothing, accelerationTimeAttack);
            velocity.x = SetDirection() * MoveTypeDictionary[moveType];
        }

        velocity.y += gravity * Time.deltaTime;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackPos.position, meleeBoxSize);
    }
}
