using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxJumpHeight = 4;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    float gravity;
    float moveSpeed = 10.0f;
    float velocityXSmoothing;

    Vector3 velocity;
    Vector2 directionalInput;
    Animator animator;
    Controller2D controller;

    float currentAIDelay;
    float AIDelay = 2.0f;
    float currentMoveTime;
    float moveTime = 2.0f;

    public Transform pos;
    public Vector2 boxSize;
    public GameObject ExclamationMark;
    public GameObject PlayerPos;

    bool FindPlayer;
    bool AIBehavior;
    bool stopAllMove;
    bool stopAi;
    bool AiIdle;
    bool TimeToBehavior;
    bool stopMoving_X;

    int random = 0;

    void Awake()
    {
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        controller = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();
        directionalInput = new Vector2(0, 0);
    }

    void Update()
    {
        CheckPlayerIsNear();
        if (!FindPlayer)
        {
            CheckAiDelay();
        }
        CalculateVelocity();
        if (stopMoving_X)
        {
            velocity.x = 0;
        }
        controller.Move(velocity * Time.deltaTime, directionalInput);
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
    }
    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }
    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
    }
    
    void CheckAiDelay()
    {
        ExclamationMark.SetActive(false);
        directionalInput = new Vector2(0, 0);
        stopMoving_X = true;
        //velocity.x = 0;
        if (currentAIDelay < AIDelay)
        {
            currentAIDelay += Time.deltaTime;
        }
        else
        {
            //stopAi = true;
            Debug.Log("delay -> move");

            if (random == 0)
            {
                random = Random.Range(1, 3);
                Debug.Log(random);
            }
            else
            {
                switch (random)
                {
                    case 1:
                        SetAiMove();
                        break;
                    case 2:
                        SetAiDirection();
                        break;
                    default:
                        break;
                }
            }
        }
    }
    void SetAiMove()
    {
        stopMoving_X = false;
        Debug.Log("setaimove");
        if (currentMoveTime < moveTime)
        {
            currentMoveTime += Time.deltaTime;
            //directionalInput = new Vector2(1, 0);
            animator.SetBool("isWalking", true);
            if(transform.localScale.x == 1)
            {
                velocity.x += 0.25f;
            }
            else
                velocity.x -= 0.25f;
            random = 1;
        }
        else
        {
            Debug.Log("move -> delay");
            animator.SetBool("isWalking", false);
            currentMoveTime = 0;
            currentAIDelay = 0;
            random = 0;
        }
    }
    void SetAiDirection()
    {
        Debug.Log("setaidirection");
        float r = Random.Range(0, 2);
        if(r == 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }
        else
            transform.localScale = new Vector2(1, 1);
        currentAIDelay = 0;
        random = 0;
    }
    void CheckPlayerIsNear()
    {
        bool flag = false;
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            Debug.Log(collider.tag);
            if (collider.tag == "player")
            {
                flag = true;
                //FindPlayer = true;
                //ExclamationMark.SetActive(true);
                //FollowPlayer();
            }
            //else
            //    FindPlayer = false;
        }
        if (flag)
        {
            FindPlayer = true;
            ExclamationMark.SetActive(true);
            FollowPlayer();
        }
        else
            FindPlayer = false;
    }
    void FollowPlayer()
    {
        if(PlayerPos.transform.position.x - transform.position.x > 0)
        {
            transform.localScale = new Vector2(1, 1);
        }
        else
            transform.localScale = new Vector2(-1, 1);
        currentMoveTime = 0;
        SetAiMove();
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(pos.position, boxSize);
    }
    //void CheckAttackTime()
    //{
    //    if (currentAttackTime < 0 && currentRollTime < 0)
    //    {
    //        //stopMoving_X = false;
    //        stopAllInput = false;
    //    }
    //    else
    //    {
    //        currentAttackTime -= Time.deltaTime;
    //    }
    //    Debug.Log("Attacktime : " + currentAttackTime);
    //}
}





