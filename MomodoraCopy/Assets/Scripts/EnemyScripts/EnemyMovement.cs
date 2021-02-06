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
        Controller2D controller;
        GameObject playerPos;

        BoxCollider2D boxCollider;

        public Transform FindPlayerBoxPos;
        public Vector2 FindPlayerBoxSize;
        public Transform AtkPlayerBoxPos;
        public Vector2 AtkPlayerBoxSize;
        public GameObject ExclamationMark;

        public bool isAttack;
        public bool isHit;

        Vector2 crushedArea;
        EnemyStatus enemyStatus;

        public Vector3 currentVelocity;
        Vector3 previousVelocity;
        

        void Start()
        {
            gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

            controller = GetComponent<Controller2D>();
            direction = new Vector2(0, 0);
            CurrentHp = Hp;
            playerPos = GameObject.FindGameObjectWithTag("Player");
            boxCollider = GetComponent<BoxCollider2D>();
            enemyStatus = transform.GetChild(0).GetComponent<EnemyStatus>();

            SetNormalCrushedArea();
        }
        public void SetNormalCrushedArea()
        {
            Bounds bounds = boxCollider.bounds;
            transform.localScale = Vector3.one;
            //bounds.Expand(new Vector2(boxCollider.size.x * -0.5f, boxCollider.size.y * -0.66f));
            bounds.Expand(0.015f * -3);
            crushedArea = bounds.size;
        }
        public void SetHurtCrushedArea()
        {
            Bounds bounds = boxCollider.bounds;
            transform.localScale = transform.localScale * 0.8f;
            bounds.Expand(new Vector2(0, bounds.size.y * -0.2f));
            bounds.Expand(0.015f * -3);
            crushedArea = bounds.size;
        }
        void CheckCrushed()//CheckCrushedByArea & CheckCrushedByTime
        {
            Collider2D[] collider2Ds =
                Physics2D.OverlapBoxAll(transform.position + Vector3.up * boxCollider.offset.y * transform.localScale.y, crushedArea, 0);
            foreach (Collider2D collider in collider2Ds)
            {
                if (collider.transform.CompareTag("Platform") ||
                    collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
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
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + Vector3.up * -0.34f * transform.localScale.y, crushedArea);
        }
    }
}