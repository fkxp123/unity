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

        public bool isAttack;
        public bool isHit;

        Vector2 crushedArea;
        EnemyStatus enemyStatus;

        public Vector3 currentVelocity;
        Vector3 previousVelocity;

        float coroutineCycle;
        WaitForSeconds waitTime;

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

            SetCrushedArea();

            coroutineCycle = 0.1f;
            waitTime = new WaitForSeconds(coroutineCycle);
            StartCoroutine(CheckCrushed());
        }

        public void SetCrushedArea()
        {
            Bounds bounds = boxCollider.bounds;
            //bounds.Expand(new Vector2(boxCollider.size.x * -0.5f, boxCollider.size.y * -0.66f));
            bounds.Expand(0.015f * -4);
            crushedArea = bounds.size;
        }
        IEnumerator CheckCrushed()
        {
            while (true)
            {
                Collider2D[] collider2Ds =
                    Physics2D.OverlapBoxAll(boxCollider.bounds.center, crushedArea, 0);
                foreach (Collider2D collider in collider2Ds)
                {
                    if (collider.transform.CompareTag("Platform") ||
                        collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
                    {
                        enemyStatus.CrushedDeath();
                    }
                }
                yield return waitTime; 
            }
        }
        //void CheckCrushed()
        //{
        //    Collider2D[] collider2Ds =
        //        Physics2D.OverlapBoxAll(transform.position + Vector3.up * boxCollider.offset.y * transform.localScale.y, crushedArea, 0);
        //    foreach (Collider2D collider in collider2Ds)
        //    {
        //        if (collider.transform.CompareTag("Platform") ||
        //            collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
        //        {
        //            enemyStatus.CrushedDeath();
        //        }
        //    }
        //}

        void CalculateCurrentVelocity()
        {
            currentVelocity = (transform.position - previousVelocity) / Time.deltaTime;
            previousVelocity = transform.position;
        }

        void Update()
        {
            //CheckCrushed();
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
        //void OnJumpMovement()
        //{
        //    velocity.y = maxJumpHeight;
        //}
        //void OffJumpMovement()
        //{
        //    if (velocity.y > minJumpVelocity)
        //    {
        //        velocity.y = minJumpVelocity;
        //    }
        //}
        void OnDrawGizmos()
        {
            //Gizmos.color = Color.red;
            //Gizmos.DrawWireCube(boxCollider.bounds.center, crushedArea);
        }
    }
}