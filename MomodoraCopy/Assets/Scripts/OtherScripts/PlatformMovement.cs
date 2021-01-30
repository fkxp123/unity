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
        Controller2D controller;
        BoxCollider2D boxCollider;

        public GameObject horizontalCheckArea;
        public GameObject verticalCheckArea;
        public Vector3 horizontalAreaSize;
        public Vector3 verticalAreaSize;
        bool findPlayer;
        bool isWaiting;

        public float insideRadius;
        public float outsideRadius;
        bool inOutSide;
        bool inInside;

        void Start()
        {
            gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);

            controller = GetComponent<Controller2D>();
            direction = new Vector2(0, 0);
            boxCollider = GetComponent<BoxCollider2D>();

            //InvokeRepeating("CheckArea", 0, 0.01f);
        }

        void WaitTime()
        {
            findPlayer = false;
            isWaiting = false;
        }

        void CheckArea()
        {
            if (!findPlayer)
            {
                Collider2D[] horizontalColls = Physics2D.OverlapBoxAll(horizontalCheckArea.transform.position, horizontalAreaSize, 0);
                foreach (Collider2D collider in horizontalColls)
                {
                    if (collider.transform.CompareTag("Player"))
                    {
                        direction.x = collider.transform.position.x < transform.position.x ? 1 : -1;
                        direction.y = 0;
                        velocity.y = 0;
                        findPlayer = true;
                    }
                }
                Collider2D[] verticalColls = Physics2D.OverlapBoxAll(verticalCheckArea.transform.position, verticalAreaSize, 0);
                foreach (Collider2D collider in verticalColls)
                {
                    if (collider.transform.CompareTag("Player"))
                    {
                        direction.y = collider.transform.position.y < transform.position.y ? 1 : -1;
                        direction.x = 0;
                        velocity.x = 0;
                        findPlayer = true;
                    }
                }
            }
            else
            {
                if (!isWaiting)
                {
                    isWaiting = true;
                    Invoke("WaitTime", 1f);
                }
            }

        }

        void Update()
        {
            //if (velocity == Vector3.zero)
            //{
            //    if (!isWaiting)
            //    {
            //        isWaiting = true;
            //        Invoke("WaitTime", 1f);
            //    }
            //}
            if (!findPlayer)
            {
                Collider2D[] horizontalColls = Physics2D.OverlapBoxAll(horizontalCheckArea.transform.position, horizontalAreaSize, 0);
                foreach (Collider2D collider in horizontalColls)
                {
                    if (collider.transform.CompareTag("Player"))
                    {
                        direction.x = collider.transform.position.x < transform.position.x ? 1 : -1;
                        direction.y = 0;
                        velocity.y = 0;
                        findPlayer = true;
                    }
                }
                Collider2D[] verticalColls = Physics2D.OverlapBoxAll(verticalCheckArea.transform.position, verticalAreaSize, 0);
                foreach (Collider2D collider in verticalColls)
                {
                    if (collider.transform.CompareTag("Player"))
                    {
                        direction.y = collider.transform.position.y < transform.position.y ? 1 : -1;
                        direction.x = 0;
                        velocity.x = 0;
                        findPlayer = true;
                    }
                }
            }
            else
            {
                if (!isWaiting)
                {
                    isWaiting = true;
                    Invoke("WaitTime", 1f);
                }
            }

            Collider2D[] outsideColls = Physics2D.OverlapCircleAll(transform.position, outsideRadius);
            foreach(Collider2D coll in outsideColls)
            {
                if (coll.transform.CompareTag("Player"))
                {
                    inOutSide = true;
                }
            }
            Collider2D[] insideColls = Physics2D.OverlapCircleAll(transform.position, insideRadius);
            foreach (Collider2D coll in insideColls)
            {
                if (coll.transform.CompareTag("Player"))
                {
                    inInside = true;
                }
                if (inInside && inOutSide)
                {
                    Debug.Log("u died");
                    //coll.GetComponent<PlayerStatus>().InstantDeath();
                    inInside = false;
                    inOutSide = false;
                }
            }

            CalculateVelocity();
            //CalculatePassengerMovement(velocity);

            //MovePassengers(true);
            controller.Move(velocity * Time.deltaTime, direction);
            //MovePassengers(false);
            CheckVerticalCollision();
        }

        void CheckVerticalCollision()
        {
            if(controller.collisions.left || controller.collisions.right)
            {
                direction.x = 0;
                velocity.x = 0;
            }
            if (controller.collisions.above || controller.collisions.below)
            {
                direction.y = 0;
                velocity.y = 0;
            }
        }

        void CalculateVelocity()
        {
            velocity.x += direction.x * gravity * Time.deltaTime;
            velocity.y += direction.y * gravity * Time.deltaTime;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(horizontalCheckArea.transform.position, horizontalAreaSize);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(verticalCheckArea.transform.position, verticalAreaSize);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, insideRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, outsideRadius);
            
        }
    }
}
