﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Enemy instance;
    public int Hp = 100;
    int CurrentHp;
    public int atk = 5;

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
    float currentAttackTime;
    float attackTime = 2.0f;
    float currentAttackDelay;

    public Transform FindPlayerBoxPos;
    public Vector2 FindPlayerBoxSize;
    public Transform AtkPlayerBoxPos;
    public Vector2 AtkPlayerBoxSize;
    public GameObject ExclamationMark;
    public GameObject PlayerPos;

    bool FindPlayer;
    bool AIBehavior;
    bool stopAllMove;
    bool stopAi;
    bool AiIdle;
    bool TimeToBehavior;
    bool stopMoving_X;
    bool canAttack;
    public bool isAttack;

    int random = 0;
    int dmg = 5;

    void Awake()
    {
        instance = this;
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        controller = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();
        directionalInput = new Vector2(0, 0);
    }

    void Update()
    {
        CanAttackPlayer();
        CheckAttackDelay();
        if (!isAttack)
        {
            CheckPlayerIsNear();
        }
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
        bool flag1 = false;
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(FindPlayerBoxPos.position, FindPlayerBoxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            Debug.Log(collider.tag);
            if (collider.tag == "player")
            {
                flag1 = true;
            }
        }
        if (flag1)
        {
            FindPlayer = true;
            ExclamationMark.SetActive(true);
            FollowPlayer();
        }
        else
            FindPlayer = false;
    }
    void CanAttackPlayer()
    {
        bool flag2 = false;
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(AtkPlayerBoxPos.position, AtkPlayerBoxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            Debug.Log(collider.tag);
            if (collider.tag == "player")
            {
                flag2 = true;
            }
        }

        if (flag2)
        {
            AttackPlayer();
        }
        else
        {
            isAttack = false;
            stopMoving_X = false;
        }
    }
    void AttackPlayer()
    {
        StartCoroutine("AttackCoroutine");
    }
    IEnumerator AttackCoroutine()
    {
        if (canAttack)
        {
            //StopAllCoroutines();
            currentAttackDelay = attackTime;
            //isAttack = true;
            //stopMoving_X = true;
            //currentAttackTime = attackTime;
            animator.SetBool("isWalking", false);
            animator.SetTrigger("isAttack");
            yield return new WaitForSeconds(0.8f);
            bool flag = false;
            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(AtkPlayerBoxPos.position, AtkPlayerBoxSize, 0);
            foreach (Collider2D collider in collider2Ds)
            {
                Debug.Log(collider.tag);
                if (collider.tag == "player")
                {
                    flag = true;
                }
            }
            if (flag)
            {
                //Debug.Log("playerhit");
                PlayerStat.instance.Hit(atk);
            }
            //yield return new WaitForSeconds(1.5f);
            //stopMoving_X = false;
            //isAttack = false;
        }
    }
    void CheckAttackDelay()
    {
        if (currentAttackDelay < 0)
        {
            canAttack = true;
        }
        else
        {
            canAttack = false;
            isAttack = true;
            //stopMoving_X = true;
            currentAttackDelay -= Time.deltaTime;
        }
    }

    void CheckAttackTime()
    {
        
        //if (currentAttackTime > 0)
        //{
        //    currentAttackTime -= Time.deltaTime;
        //}
        //else
        //{
        //    stopMoving_X = false;
        //    isAttack = false;
        //    Debug.Log("imhere");
        //    bool flag = false;
        //    Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(AtkPlayerBoxPos.position, AtkPlayerBoxSize, 0);
        //    foreach (Collider2D collider in collider2Ds)
        //    {
        //        Debug.Log(collider.tag);
        //        if (collider.tag == "player")
        //        {
        //            flag = true;
        //        }
        //    }
        //    if (flag)//flag가 아니라 따로 콜라이더충돌체크해야댐
        //    {
        //        PlayerStat.instance.Hit(5);
        //    }
        //    currentAttackTime = 0;
        //}
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
    public void Hit(int enemyAtk)
    {
        CurrentHp -= enemyAtk;
        Debug.Log("hp : " + CurrentHp);
        animator.SetTrigger("takeDamage");
        if (CurrentHp <= 0)
        {
            Debug.Log("game over");
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(FindPlayerBoxPos.position, FindPlayerBoxSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(AtkPlayerBoxPos.position, AtkPlayerBoxSize);
    }
}





