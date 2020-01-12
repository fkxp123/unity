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

    bool FindPlayer;
    bool AIBehavior;
    bool stopAllMove;

    void Awake()
    {
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        controller = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();
        directionalInput = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        CheckAIDelay();
        CalculateVelocity();
        if (!stopAllMove)
        {
            controller.Move(velocity * Time.deltaTime, directionalInput);
        }
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
    void CheckMoveTime()
    {
        Debug.Log(currentMoveTime);
        if (currentMoveTime < moveTime)
        {
            currentMoveTime += Time.deltaTime;
            //directionalInput = new Vector2(1, 0);
            animator.SetBool("isWalking", true);
            velocity.x += 0.25f;
        }
        else
        {
            Debug.Log("time -> delay");
            animator.SetBool("isWalking", false);
            currentAIDelay = 0;
            currentMoveTime = 0;
        }
    }
    void CheckAIDelay()
    {
        directionalInput = new Vector2(0, 0);
        stopAllMove = true;
        //velocity.x = 0;
        if (currentAIDelay < AIDelay)
        {
            currentAIDelay += Time.deltaTime;
        }
        else
        {
            Debug.Log("delay -> time");
            stopAllMove = false;
            //currentMoveTime = moveTime;//this is problem
            CheckMoveTime();
        }
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





