using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Enemy instance;
    public int Hp = 100;
    int CurrentHp;
    public int atk = 5;
    public float HitDistance = 3;
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
    GameObject playerPos;
    Player player;
    PlayerStat ps;
    Rigidbody2D rigid;

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
    //public GameObject PlayerPos;

    bool FindPlayer;
    //bool AIBehavior;
    bool stopAllMove;
    //bool stopAi;
    //bool AiIdle;
    bool TimeToBehavior;
    bool stopMoving_X;
    bool canAttack;
    public bool isAttack;
    public bool isHit;

    int random = 0;
    int dmg = 5;

    void Start()
    {
        ps = PlayerStat.instance;
    }
    void Awake()
    {
        instance = this;
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        controller = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        directionalInput = new Vector2(0, 0);
        player = Player.instance;
        CurrentHp = Hp;
        playerPos = GameObject.FindGameObjectWithTag("player");
        //ps = GetComponent<PlayerStat>();
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
        bool flag = false;
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(FindPlayerBoxPos.position, FindPlayerBoxSize, 0);
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
            FindPlayer = true;
            ExclamationMark.SetActive(true);
            FollowPlayer();
        }
        else
            FindPlayer = false;
    }
    void CanAttackPlayer()
    {
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
        //StopAllCoroutines();
        StartCoroutine("AttackCoroutine");
    }
    IEnumerator AttackCoroutine()
    {
        if (canAttack)
        {
            currentAttackDelay = attackTime;
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
                ps.Hit(atk);
            }
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
            currentAttackDelay -= Time.deltaTime;
        }
    }

    void FollowPlayer()
    {
        if(playerPos.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector2(1, 1);
        }
        else
            transform.localScale = new Vector2(-1, 1);
        currentMoveTime = 0;
        SetAiMove();
    }
    public void HitbyPlayer(int playerAtk)
    {
        StopAllCoroutines();
        StartCoroutine("HitCoroutine", playerAtk);
    }
    IEnumerator HitCoroutine(int playerAtk)
    {
        Debug.Log("in enemy hit");
        CurrentHp -= playerAtk;
        Debug.Log("enemy hp : " + CurrentHp);
        animator.SetTrigger("takeDamage");
        stopMoving_X = true;
        if (CurrentHp <= 0)
        {
            Debug.Log("enemy die");
        }
        if (playerPos.transform.position.x <= transform.position.x)
        {
            rigid.velocity = new Vector2(HitDistance, rigid.velocity.y);
        }
        else
        {
            rigid.velocity = new Vector2(-1 * HitDistance, rigid.velocity.y);
        }
        yield return new WaitForSeconds(0.6f);
        rigid.velocity = new Vector2(0, rigid.velocity.y);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(FindPlayerBoxPos.position, FindPlayerBoxSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(AtkPlayerBoxPos.position, AtkPlayerBoxSize);
    }
}





