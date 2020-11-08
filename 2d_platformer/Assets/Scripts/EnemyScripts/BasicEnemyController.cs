using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour
{
    public enum State
    {
        Idle,
        Patrol,
        Knockback,
        Attack,
        Chase,
        Dead
    }

    public State currentState;

    public float
        groundCheckDistance,
        wallCheckDistance,
        movementSpeed,
        maxHp,
        knockbackDuration;
    public Transform
        groundCheck,
        wallCheck;
    public LayerMask whatIsGround;
    int
        facingDirection,
        damageDirection;
    float
        currentHp,
        knockbackStartTime,
        
        patrolTime;
    bool
        groundDetected,
        wallDetected,
        findPlayer,
        activated;
    public GameObject
        hitParticle,
        deathChunkParticle,
        deathBloodParticle;
    GameObject alive;
    Rigidbody2D aliveRb;
    public Vector2 movement;
    Vector2 knockbackSpeed;
    Animator aliveAnim;
    // Start is called before the first frame update
    void Start()
    {
        alive = transform.Find("Alive").gameObject;
        aliveRb = alive.GetComponent<Rigidbody2D>();
        facingDirection = 1;
        currentHp = maxHp;
        aliveAnim = alive.GetComponent<Animator>();
        SwitchState(State.Patrol);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(currentState);
        if (!activated)
        {
            switch (currentState)
            {
                case State.Idle:
                    UpdateIdleState();
                    break;
                case State.Patrol:
                    UpdatePatrolState();
                    break;
                case State.Knockback:
                    UpdateKnockbackState();
                    break;
                case State.Attack:
                    UpdateAttackState();
                    break;
                case State.Chase:
                    UpdateChaseState();
                    break;
                case State.Dead:
                    UpdateDeadState();
                    break;
                default:
                    break;
            }
        }
    }
    #region IdleState
    public void EnterIdleState()
    {
        //currentState = State.Idle;
    }
    public void UpdateIdleState()
    {
        //FindPlayer();
        if (findPlayer)
        {
            SwitchState(State.Patrol);
        }
        else
        {
            //StartCoroutine("Wait");
        }

    }
    public void ExitIdleState()
    {

    }
    #endregion
    #region PatrolState
    public void EnterPatrolState()
    {
        //currentState = State.Patrol;
        patrolTime = Time.time;
        aliveAnim.SetBool("Moving", true);
    }
    public void UpdatePatrolState()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, whatIsGround);
        if (!groundDetected || wallDetected)
        {
            Flip();
        }
        else
        {
            movement.Set(movementSpeed * facingDirection, aliveRb.velocity.y);
            aliveRb.velocity = movement;
        }
        //if (Time.time - patrolTime >= 3)
        //{
        //    Debug.Log("patrol -> idle");
        //    SwitchState(State.Idle);
        //}
        //else
        //{
        //    if (!groundDetected || wallDetected)
        //    {
        //        Flip();
        //    }
        //    else
        //    {
        //        movement.Set(movementSpeed * facingDirection, aliveRb.velocity.y);
        //        aliveRb.velocity = movement;
        //    }
        //}        
    }
    public void ExitPatrolState()
    {
        aliveAnim.SetBool("Moving", false);
    }
    #endregion
    #region KnockbackState
    public void EnterKnockbackState()
    {
        currentState = State.Knockback;
        knockbackStartTime = Time.time;
        movement.Set(knockbackSpeed.x * damageDirection, knockbackSpeed.y);
        aliveRb.velocity = movement;
        aliveAnim.SetBool("Knockback", true);
    }
    public void UpdateKnockbackState()
    {
        if (Time.time >= knockbackStartTime + knockbackDuration)
        {
            SwitchState(State.Idle);
        }
    }
    public void ExitKnockbackState()
    {
        aliveAnim.SetBool("Knockback", false);
    }
    #endregion
    #region AttackState
    public void EnterAttackState()
    {
        currentState = State.Attack;
    }
    public void UpdateAttackState()
    {

    }
    public void ExitAttackState()
    {

    }
    #endregion
    #region ChaseState
    public void EnterChaseState()
    {
        currentState = State.Chase;
    }
    public void UpdateChaseState()
    {

    }
    public void ExitChaseState()
    {

    }
    #endregion
    #region DeadState
    public void EnterDeadState()
    {
        currentState = State.Dead;
        Destroy(gameObject);
    }
    public void UpdateDeadState()
    {

    }
    public void ExitDeadState()
    {

    }
    #endregion

    public void Damage(float[] attackDetails)
    {
        currentHp -= attackDetails[0];

        Instantiate(hitParticle, alive.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));

        if (attackDetails[1] > alive.transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }

        if (currentHp > 0)
        {
            SwitchState(State.Knockback);
        }
        else if (currentHp <= 0)
        {
            SwitchState(State.Dead);
        }
    }
    public void Flip()
    {
        facingDirection *= -1;
        alive.transform.Rotate(0, 180, 0);
    }
    public void SwitchState(State _state)
    {
        switch (currentState)
        {
            case State.Idle:
                ExitIdleState();
                break;
            case State.Patrol:
                ExitPatrolState();
                break;
            case State.Knockback:
                ExitKnockbackState();
                break;
            case State.Attack:
                ExitAttackState();
                break;
            case State.Chase:
                ExitChaseState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
            default:
                break;
        }
        currentState = _state;
        switch (_state)
        {
            case State.Idle:
                EnterIdleState();
                break;
            case State.Patrol:
                EnterPatrolState();
                break;
            case State.Knockback:
                EnterKnockbackState();
                break;
            case State.Attack:
                EnterAttackState();
                break;
            case State.Chase:
                EnterChaseState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
            default:
                break;
        }
    }
    IEnumerator Wait()
    {
        activated = true;
        int random = Random.Range(0, 3);
        Debug.Log(random);
        if (random == 0)
        {
            SwitchState(State.Patrol);
        }
        else if (random == 1)
        {
            Flip();
        }
        yield return new WaitForSeconds(3f);
        activated = false;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }
}
