using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    [RequireComponent(typeof(Controller2D))]
    public class PlatformMovement : MonoBehaviour
    {


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
        PlayerStatus ps;

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

        void Start()
        {
            ps = PlayerStatus.instance;
            gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

            controller = GetComponent<Controller2D>();
            animator = GetComponent<Animator>();
            direction = new Vector2(0, 0);
            playerPos = GameObject.FindGameObjectWithTag("Player");
            boxCollider = GetComponent<BoxCollider2D>();
        }

        void Update()
        {
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
