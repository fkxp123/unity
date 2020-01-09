using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    public float moveSpeed = 10;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;
    float timeToWallUnstick;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;
    public float rollPower = 1.2f;

    Controller2D controller;
    Animator animator;
    //public Camera cam;
    //CameraFollow cf;

    //float campos;
    //float targetpos;
    float oldVelocityY;
    float oldPositionX;
    float oldPositionY;
    int jumpCount;
    Vector2 currentDirection;
    Vector2 RollVelocity;
    //float smpos;

    public bool doubleJump = false;
    public bool stopAllMove = false;
    bool stopMoving;
    bool stopMoving_X;
    bool stopMoving_Y;
    float RollDelay = 2.0f;
    float currentRollDelay;
    bool canRoll = false;

    Vector2 directionalInput;
    bool wallSliding;
    int wallDirX;

    void Start()
    {
        //cf = GetComponent<CameraFollow>();
        controller = GetComponent<Controller2D>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }
    void Awake()//
    {
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        if (!stopAllMove)
        {
            CheckPositionY();
            CalculateVelocity();
            //HandleWallSliding();
            //on/off기능 추가?

            if (stopMoving_X)
            {
                StopMovingX();
            }
            if (stopMoving_Y)
            {
                StopMovingY();
            }

            controller.Move(velocity * Time.deltaTime, directionalInput);
            SetSpriteDirection();
            CheckAllAnim();
            CheckAllDelay();

            if (controller.collisions.above || controller.collisions.below)
            {
                velocity.y = 0;
            }
        }
    }
    void LateUpdate()
    {
        //CrouchAnim();
    }
    void ResetAllAnim()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isJumping", false);
        animator.SetBool("isFalling", false);
        animator.SetBool("doubleJump", false);
        animator.SetBool("isJumpingHigh", false);
    }
    void ResetAllMove()
    {
        stopAllMove = false;
    }
    void StopMoving()
    {
        if (stopMoving)
        {
            velocity.x = 0;
            velocity.y = 0;
        }
    }
    void StopMovingX()
    {
        velocity.x = 0;
    }
    void StopMovingY()
    {
        velocity.y = 0;
    }
    void ResetMoving()
    {
        stopMoving = false;
    }

    void SetSpriteDirection()
    {
        if (directionalInput.x == 1)
        {
            this.transform.localScale = new Vector2(1, 1);
        }
        else if (directionalInput.x == -1)
        {
            this.transform.localScale = new Vector2(-1, 1);
        }
    }
    void CheckAllAnim()
    {
        MoveAnim();
        JumpAnim();
        LandAnim();
        CrouchAnim();
    }
    void CheckAllDelay()
    {
        CheckRollDelay();
        CheckAttackDelay();
    }
    void CheckRollDelay()
    {
        Debug.Log("Rolldelay : " + currentRollDelay);
        currentRollDelay -= Time.deltaTime;
        if (currentRollDelay < 0)
        {
            canRoll = true;
        }
        else
            canRoll = false;
    }
    void CheckAttackDelay()
    {

    }
    void MoveAnim()//
    {
        if (directionalInput.x == 0)
            animator.SetBool("isWalking", false);
        else
            animator.SetBool("isWalking", true);
    }

    public void CheckPositionY()//공중에 있는도중 가장 높은위치값 저장
    {
        if (animator.GetBool("isFalling") == true)
        {
            if (oldPositionY < transform.position.y)
            {
                oldPositionY = transform.position.y;
            }
        }
        else
            oldPositionY = transform.position.y;
    }

    void LandAnim()
    {
        if (animator.GetBool("isFalling") == true)
        {
            if (oldPositionY - transform.position.y > 8)
            {
                
                if (controller.collisions.below) //landing
                {
                    animator.SetBool("isGround", true);
                    animator.SetBool("isJumpingHigh", true);
                    stopAllMove = true;
                    Invoke("ResetAllMove", .3f);//equal to HasExitTime
                }
            }
            else
            {
                if (controller.collisions.below) //landing
                {
                    animator.SetBool("isGround", true);
                    stopMoving = true;
                    StopMoving();
                    Invoke("ResetMoving", .2f);
                }
            }

        }
        else
            animator.SetBool("isJumpingHigh", false);

        if (controller.collisions.below == true) //checking landing
        {
            
            animator.SetBool("isGround", true);
            //animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
            //canDoubleJump = false;
            jumpCount = 0;
        }

    }
    public void CrouchAnim()
    {
        if (animator.GetBool("isCrouching") && !animator.GetBool("isRolling"))
        {
            stopMoving_X = true;
        }
        else
            stopMoving_X = false;

        if (directionalInput.y == -1 && animator.GetBool("isGround"))
        {
            oldPositionX = transform.position.x;
            transform.position = new Vector2(oldPositionX, transform.position.y);
            animator.SetBool("isCrouching", true);
        }
        else
        {
            animator.SetBool("isCrouching", false);
        }
            //float vel = 0.0f;
        //    if (directionalInput.y == -1 && animator.GetBool("isGround"))
        //{
        //    oldPositionX = transform.position.x;
        //    transform.position = new Vector2(oldPositionX, transform.position.y);
        //    animator.SetBool("isCrouching", true);
            //cam.GetComponent<CameraFollow>().enabled = false;
            //if (directionalInput.x == 1)
            //{
            //    targetpos = transform.position.x + 1;
            //    Debug.Log(cam.transform.position);
            //    campos = Mathf.SmoothDamp(cam.transform.position.x, targetpos, ref vel, 0.03f);

            //    cam.transform.position = new Vector3(campos, cam.transform.position.y, cam.transform.position.z);
            //    Debug.Log("!!!!!"+cam.transform.position);
            //}
            //else if (directionalInput.x == -1)
            //{
            //    targetpos = transform.position.x - 1;
            //    campos = Mathf.SmoothDamp(cam.transform.position.x, targetpos, ref vel, 0.03f);

            //    cam.transform.position = new Vector3(campos, cam.transform.position.y, cam.transform.position.z); 
            //}
        //}
        //else
        //{
        //    animator.SetBool("isCrouching", false);
            //campos = Mathf.SmoothDamp(campos, transform.position.x, ref vel, 0.03f);//return to original camera setting
            //cam.transform.position = new Vector3(campos, cam.transform.position.y, cam.transform.position.z);
            //if(Mathf.Abs(campos - transform.position.x) < 0.01)
            //{
            //    cam.GetComponent<CameraFollow>().enabled = true;
            //}
        //}
    }
    void JumpAnim()//
    {
        if (!animator.GetBool("isRolling"))
        {
            if (controller.collisions.below == false) //checking ground
            {
                animator.SetBool("isGround", false);
            }
            if (controller.collisions.below == false && oldVelocityY > velocity.y) //checking falling
            {
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", true);
            }
            if (controller.collisions.below == false && doubleJump) //checking doublejump
            {
                animator.SetBool("doubleJump", true);
            }
            if (controller.collisions.below == false && oldVelocityY > velocity.y) //checking double falling
            {
                animator.SetBool("doubleJump", false);
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", true);
                doubleJump = false;
            }
            oldVelocityY = velocity.y;
        }
        
    }

    IEnumerator RollCoroutine()
    {
        if (canRoll)
        {
            currentRollDelay = RollDelay;
            stopMoving_X = false;
            //animator.SetBool("isFalling", false);
            if (animator.GetBool("isGround"))
            {
                animator.SetBool("isRolling", true);

                if (directionalInput.x == 0 || directionalInput.y == -1)
                {
                    if (transform.localScale.x == 1)
                    {
                        velocity = new Vector3(velocity.x + 30, 0);
                        controller.Move(velocity * Time.deltaTime, new Vector2(1, 0));
                    }
                    if (transform.localScale.x == -1)
                    {
                        velocity = new Vector3(velocity.x - 30, 0);
                        controller.Move(velocity * Time.deltaTime, new Vector2(-1, 0));
                    }
                }
                else
                {
                    if (transform.localScale.x == 1)
                    {
                        velocity = new Vector3(velocity.x + 10, 0);
                        controller.Move(velocity * Time.deltaTime, new Vector2(1, 0));
                    }
                    if (transform.localScale.x == -1)
                    {
                        velocity = new Vector3(velocity.x - 10, 0);
                        controller.Move(velocity * Time.deltaTime, new Vector2(-1, 0));
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
            animator.SetBool("isRolling", false);
        }
        
    }
    public void Roll()
    {
        StartCoroutine("RollCoroutine");
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (!animator.GetBool("isRolling"))
        {
            oldPositionY = transform.position.y;
            CheckPositionY();
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

            if (jumpCount == 0)
            {
                if (directionalInput.y != -1 || !controller.isThroughPlatform)//
                {
                    velocity.y = maxJumpVelocity;
                    jumpCount += 1;
                    animator.SetBool("isJumping", true);
                }
            }
            else if (jumpCount == 1)
            {
                velocity.y = maxJumpVelocity;
                doubleJump = true;
                jumpCount += 1;
                animator.SetBool("doubleJump", true);
            }
            else if (jumpCount == 1 && controller.isThroughPlatform)//platform은 통과x throughPlatform은 통과
            {
                velocity.y = maxJumpVelocity;
                doubleJump = true;
                jumpCount += 1;
                animator.SetBool("doubleJump", true);
            }
        }
        
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
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

    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
    }
}