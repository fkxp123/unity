﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class MovingTrapController : RaycastController
    {
        public CollisionInfo collisions;

        public LayerMask passengerMask;
        public float speed;
        public float waitTime;
        List<PassengerMovement> passengerMovement;
        Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

        public Vector3 velocity;
        public Vector2 direction;

        public bool findPlayer;

        public Vector3 horizontalAreaSize;
        public Vector3 verticalAreaSize;
        public Vector3 deathAreaSize;

        public float currentTime;
        bool isFirst;

        public ParticleSystem dustCloudEffect;

        public override void Start()
        {
            base.Start();
            collisions.hoziontalDirection = transform.rotation.y == 0 ? 1 : -1;
            collisions.verticalDirection = -1;
            direction = new Vector2(0, 0);
            deathAreaSize = new Vector3(1.9f, 1.9f);
        }

        void ResetFindPlayer()
        {
            findPlayer = false;
        }

        void FindPlayer()
        {
            Collider2D[] horizontalColls = Physics2D.OverlapBoxAll(transform.position, horizontalAreaSize, 0);
            foreach (Collider2D collider in horizontalColls)
            {
                if (collider.transform.CompareTag("Player") && !findPlayer)
                {
                    direction.x = collider.transform.position.x < transform.position.x ? -1 : 1;
                    direction.y = 0;
                    findPlayer = true;
                }
            }

            Collider2D[] verticalColls = Physics2D.OverlapBoxAll(transform.position, verticalAreaSize, 0);
            foreach (Collider2D collider in verticalColls)
            {
                if (collider.transform.CompareTag("Player") && !findPlayer)
                {
                    direction.y = collider.transform.position.y < transform.position.y ? -1 : 1;
                    direction.x = 0;
                    findPlayer = true;
                }
            }
        }

        void Update()
        {
            UpdateRaycastOrigins();

            CalculateVelocity();

            CalculatePassengerMovement(velocity);

            MovePassengers(true);
            Move(velocity);
            MovePassengers(false);

            CheckVerticalCollision();

            if(velocity != Vector3.zero)
            {
                isFirst = false;
                currentTime = 0;
            }
            //CheckPlayerInstantDeath();
            //FindPlayer();
            Debug.Log("left : "+collisions.left);
            Debug.Log("right : "+collisions.right);
            Debug.Log("above : "+collisions.above);
            Debug.Log("below : "+collisions.below);
        }

        void CheckVerticalCollision()
        {
            //if (collisions.left || collisions.right)
            //{
            //    direction.x = 0;
            //    //velocity.x = 0;
            //}
            //if (collisions.above || collisions.below)
            //{
            //    direction.y = 0;
            //    //velocity.y = 0;
            //}
            if(collisions.left || collisions.right ||
               collisions.above || collisions.below)
            {
                if (currentTime <= 0 && !isFirst)
                {
                    if (collisions.left)
                    {
                        dustCloudEffect.transform.position = transform.position + Vector3.left * 1.25f;
                        dustCloudEffect.transform.rotation = Quaternion.Euler(0, 90, 0);
                        direction.x = 0;
                        velocity.x = 0;
                    }
                    if (collisions.right)
                    {
                        dustCloudEffect.transform.position = transform.position + Vector3.right * 1.25f;
                        dustCloudEffect.transform.rotation = Quaternion.Euler(0, -90, 0);
                        direction.x = 0;
                        velocity.x = 0;
                    }
                    if (collisions.above)
                    {
                        dustCloudEffect.transform.position = transform.position + Vector3.up * 1.25f;
                        dustCloudEffect.transform.rotation = Quaternion.Euler(90, 0, 0);
                        direction.y = 0;
                        velocity.y = 0;
                    }
                    if (collisions.below)
                    {
                        dustCloudEffect.transform.position = transform.position + Vector3.down * 1.25f;
                        dustCloudEffect.transform.rotation = Quaternion.Euler(-90, 0, 0);
                        direction.y = 0;
                        velocity.y = 0;
                    }
                    dustCloudEffect.Play();
                    isFirst = true;
                    currentTime = waitTime;
                }
                currentTime -= Time.deltaTime;
                if (currentTime <= 0)
                {
                    ResetFindPlayer();
                }
            }
        }

        void CalculateVelocity()
        {
            velocity.x = direction.x * speed * Time.deltaTime;
            velocity.y = direction.y * speed * Time.deltaTime;
            Debug.Log("dx : "+direction.x);
            Debug.Log("dy : "+direction.y);
            //if (direction.x != 0 && direction.y == 0)
            //{
            //    velocity.x = direction.x * speed * Time.deltaTime;
            //}
            //if (direction.x == 0 && direction.y != 0)
            //{
            //    velocity.y = direction.y * speed * Time.deltaTime;
            //}
        }

        void MovePassengers(bool beforeMovePlatform)
        {
            foreach (PassengerMovement passenger in passengerMovement)
            {
                if (!passengerDictionary.ContainsKey(passenger.transform))
                {
                    passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<Controller2D>());
                }

                if (passenger.moveBeforePlatform == beforeMovePlatform)
                {
                    passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
                }
            }
        }

        void CalculatePassengerMovement(Vector3 velocity)
        {
            HashSet<Transform> movedPassengers = new HashSet<Transform>();
            passengerMovement = new List<PassengerMovement>();

            float directionX = Mathf.Sign(velocity.x);
            float directionY = Mathf.Sign(velocity.y);

            // Vertically moving platform
            if (velocity.y != 0)
            {
                float rayLength = Mathf.Abs(velocity.y) + skinWidth;

                for (int i = 0; i < verticalRayCount; i++)
                {
                    Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                    rayOrigin += Vector2.right * (verticalRaySpacing * i);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

                    if (hit && hit.distance != 0)
                    {
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            movedPassengers.Add(hit.transform);
                            float pushX = (directionY == 1) ? velocity.x : 0;
                            float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

                            passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                        }
                    }
                }
            }

            // Horizontally moving platform
            if (velocity.x != 0)
            {
                float rayLength = Mathf.Abs(velocity.x) + skinWidth;

                for (int i = 0; i < horizontalRayCount; i++)
                {
                    Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                    rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

                    if (hit && hit.distance != 0)
                    {
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            movedPassengers.Add(hit.transform);
                            float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                            float pushY = -skinWidth;
                            passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                        }
                    }
                }
            }

            // Passenger on top of a horizontally or downward moving platform
            if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
            {
                float rayLength = skinWidth * 2;

                for (int i = 0; i < verticalRayCount; i++)
                {
                    Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                    if (hit && hit.distance != 0)
                    {
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            movedPassengers.Add(hit.transform);
                            float pushX = velocity.x;
                            float pushY = velocity.y;
                            passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                        }
                    }
                }
            }
        }

        struct PassengerMovement
        {
            public Transform transform;
            public Vector3 velocity;
            public bool standingOnPlatform;
            public bool moveBeforePlatform;

            public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform)
            {
                transform = _transform;
                velocity = _velocity;
                standingOnPlatform = _standingOnPlatform;
                moveBeforePlatform = _moveBeforePlatform;
            }
        }

        public void Move(Vector2 moveAmount)
        {
            UpdateRaycastOrigins();

            Vector2 newMoveAmount = moveAmount;
            collisions.Reset();
            collisions.moveAmountOld = moveAmount;

            if (moveAmount.x != 0)
            {
                collisions.hoziontalDirection = (int)Mathf.Sign(moveAmount.x);
            }
            if(moveAmount.y != 0)
            {
                collisions.verticalDirection = (int)Mathf.Sign(moveAmount.y);
            }

            if(moveAmount.x == 0)
            {
                VerticalCollisions(ref newMoveAmount);
            }
            if(moveAmount.y == 0)
            {
                HorizontalCollisions(ref newMoveAmount);
            }


            transform.Translate(newMoveAmount, Space.World);
        }

        public void HorizontalCollisions(ref Vector2 moveAmount)
        {
            float directionX = collisions.hoziontalDirection;
            float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
            float checkPlatformLength = 1.0f;
            float findPlayerLength = 10.0f;

            if (Mathf.Abs(moveAmount.x) < skinWidth)
            {
                rayLength = 2 * skinWidth;
            }

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                Vector2 leftOrigin = raycastOrigins.bottomLeft;
                Vector2 rightOrigin = raycastOrigins.bottomRight;

                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                leftOrigin += Vector2.up * (horizontalRaySpacing * i);
                rightOrigin += Vector2.up * (horizontalRaySpacing * i);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
                RaycastHit2D hitPlayer = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);
                RaycastHit2D hitPlatform = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, checkPlatformLength, collisionMask);
                RaycastHit2D findPlayerLeft = Physics2D.Raycast(leftOrigin, Vector2.left, findPlayerLength, passengerMask);
                RaycastHit2D findPlayerRight = Physics2D.Raycast(rightOrigin, Vector2.right, findPlayerLength, passengerMask);

                //Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.blue);
                //Debug.DrawRay(rayOrigin, Vector2.right * checkPlatformLength * directionX, Color.green);
                //Debug.DrawRay(leftOrigin, Vector2.left * findPlayerLength, Color.green);
                //Debug.DrawRay(rightOrigin, Vector2.right * findPlayerLength, Color.green);

                if (hit)
                {
                    if (hit.distance == 0)
                    {
                        continue;
                    }

                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;
                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
                if (!findPlayer)
                {
                    if (findPlayerLeft)
                    {
                        direction.x = -1;
                        findPlayer = true;
                    }
                    else if (findPlayerRight)
                    {
                        direction.x = 1;
                        findPlayer = true;
                    }
                }
                if (hitPlayer && hitPlatform)
                {
                    Debug.Log("dead by raycast");
                    hitPlayer.transform.GetComponent<PlayerStatus>().InstantDeath();
                }
            }
        }

        void VerticalCollisions(ref Vector2 moveAmount)
        {
            float directionY = collisions.verticalDirection;
            float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;
            float checkPlatformLength = 1.0f;
            float findPlayerLength = 10.0f;
            if (Mathf.Abs(moveAmount.y) < skinWidth)
            {
                rayLength = 2 * skinWidth;
            }

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                Vector2 topOrigin = raycastOrigins.topLeft;
                Vector2 botOrigin = raycastOrigins.bottomLeft;

                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                topOrigin += Vector2.right * (verticalRaySpacing * i);
                botOrigin += Vector2.right * (verticalRaySpacing * i);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
                RaycastHit2D hitPlayer = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);
                RaycastHit2D hitPlatform = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, checkPlatformLength, collisionMask);
                RaycastHit2D findPlayerUp = Physics2D.Raycast(topOrigin, Vector2.up, findPlayerLength, passengerMask);
                RaycastHit2D findPlayerDown = Physics2D.Raycast(botOrigin, Vector2.down, findPlayerLength, passengerMask);

                //Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);
                //Debug.DrawRay(rayOrigin, Vector2.up * checkPlatformLength * directionY, Color.green);
                //Debug.DrawRay(topOrigin, Vector2.up * findPlayerLength, Color.green);
                //Debug.DrawRay(botOrigin, Vector2.down * findPlayerLength, Color.green);

                if (hit)
                {
                    moveAmount.y = (hit.distance - skinWidth) * directionY;
                    rayLength = hit.distance;

                    collisions.below = directionY == -1;
                    collisions.above = directionY == 1;
                }
                if (!findPlayer)
                {
                    if (findPlayerUp)
                    {
                        direction.y = 1;
                        findPlayer = true;
                    }
                    else if (findPlayerDown)
                    {
                        direction.y = -1;
                        findPlayer = true;
                    }
                }
                if(hitPlayer && hitPlatform)
                {
                    hitPlayer.transform.GetComponent<PlayerStatus>().InstantDeath();
                }
            }

        }



        public struct CollisionInfo
        {
            public bool above, below;
            public bool left, right;

            public Vector2 moveAmountOld;
            public int hoziontalDirection;
            public int verticalDirection;

            public void Reset()
            {
                above = below = false;
                left = right = false;
            }
        }
    }
}
