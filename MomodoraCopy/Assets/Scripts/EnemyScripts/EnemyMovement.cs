using System.Collections;
using UnityEngine;

namespace MomodoraCopy
{
    [RequireComponent(typeof(Controller2D))]
    public class EnemyMovement : MonoBehaviour
    {
        public int Hp = 100;
        int CurrentHp;
        public int atk = 5;
        public float HitDistance = 3;

        float maxJumpHeight = 4;
        float minJumpHeight = 1;
        float timeToJumpApex = .4f;
        float maxJumpVelocity;
        float minJumpVelocity;

        float accelerationTimeAirborne = .2f;
        float accelerationTimeGrounded = .1f;
        float gravity;
        float moveSpeed = 3.0f;
        float velocityXSmoothing;

        public Vector3 velocity;
        public Vector2 direction;
        Animator animator;
        Controller2D controller;
        GameObject playerPos;

        Player player;

        BoxCollider2D boxCollider;

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

        bool FindPlayer;

        public bool isAttack;
        public bool isHit;


        Vector2 checkCrushedArea;
        EnemyStatus enemyStatus;

        public Vector3 currentVelocity;
        Vector3 previousVelocity;

        void Start()
        {
            gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

            controller = GetComponent<Controller2D>();
            animator = GetComponent<Animator>();
            direction = new Vector2(0, 0);
            CurrentHp = Hp;
            playerPos = GameObject.FindGameObjectWithTag("Player");
            boxCollider = GetComponent<BoxCollider2D>();
            enemyStatus = GetComponent<EnemyStatus>();

            Bounds bounds = boxCollider.bounds;

            bounds.Expand(new Vector2(-0.5f, -1.5f));
            //bounds.Expand(0.015f * -3);
            checkCrushedArea = bounds.size;
        }

        void CheckCrushed()//CheckCrushedByArea & CheckCrushedByTime
        {
            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(transform.position + Vector3.up * 0.22f, checkCrushedArea, 0);
            foreach (Collider2D collider in collider2Ds)
            {
                if (collider.transform.CompareTag("Platform"))
                {
                    enemyStatus.CrushedDeath();
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
            SetEnemyMovement();
            CheckVerticalCollision();
        }
        void SetEnemyMovement()
        {
            CalculateVelocity();
            controller.Move(velocity * Time.deltaTime, direction);
        }
        void CheckVerticalCollision()
        {
            if (controller.collisions.above || controller.collisions.below)
            {
                velocity.y = 0;
            }
        }
        public void SetDirection(Vector2 direction)
        {
            this.direction = direction;
        }
        void CalculateVelocity()
        {
            float targetVelocityX = direction.x * moveSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
            velocity.y += gravity * Time.deltaTime;
        }
        void OnJumpMovement()
        {
            velocity.y = maxJumpHeight;
        }
        void OffJumpMovement()
        {
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
        }

        //void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireCube(FindPlayerBoxPos.position, FindPlayerBoxSize);
        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawWireCube(AtkPlayerBoxPos.position, AtkPlayerBoxSize);
        //}
    }
}