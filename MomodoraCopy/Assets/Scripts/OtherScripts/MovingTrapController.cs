using System.Collections;
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
        Vector3 Velocity
        {
            get { return velocity; }
            set
            {
                if (value.x < 0)
                {
                    dustCloudEffect.transform.position = transform.position + Vector3.left * 1.25f;
                    dustCloudEffect.transform.rotation = Quaternion.Euler(0, 90, 0);   
                }
                else if (value.x > 0)
                {
                    dustCloudEffect.transform.position = transform.position + Vector3.right * 1.25f;
                    dustCloudEffect.transform.rotation = Quaternion.Euler(0, -90, 0);
                }
                else if (value.y > 0)
                {
                    dustCloudEffect.transform.position = transform.position + Vector3.up * 1.25f;
                    dustCloudEffect.transform.rotation = Quaternion.Euler(90, 0, 0);
                }
                else if (value.y < 0)
                {
                    dustCloudEffect.transform.position = transform.position + Vector3.down * 1.25f;
                    dustCloudEffect.transform.rotation = Quaternion.Euler(-90, 0, 0);
                }
                if (value != Vector3.zero)
                {
                    isFirst = false;
                    currentTime = 0;
                }
                velocity = value;
            }
        }
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

        void Update()
        {
            UpdateRaycastOrigins();

            CalculateVelocity();

            CalculatePassengerMovement(Velocity);

            MovePassengers(true);
            Move(Velocity);
            MovePassengers(false);

            CheckVerticalCollision();

            if(Velocity != Vector3.zero)
            {
                isFirst = false;
                currentTime = 0;
            }
        }

        void CheckVerticalCollision()
        {
            if(collisions.left || collisions.right ||
               collisions.above || collisions.below)
            {
                if (currentTime <= 0 && !isFirst)
                {
                    if (collisions.left || collisions.right)
                    {
                        direction.x = 0;
                        Velocity = new Vector3(0, Velocity.y);
                    }
                    if (collisions.above || collisions.below)
                    {
                        direction.y = 0;
                        Velocity = new Vector3(Velocity.x, 0);
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
            Velocity = new Vector3(direction.x * speed * Time.deltaTime, direction.y * speed * Time.deltaTime);  
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

            float directionX = Mathf.Sign(Velocity.x);
            float directionY = Mathf.Sign(Velocity.y);
            
            if (Velocity.y != 0)
            {
                float rayLength = Mathf.Abs(Velocity.y) + skinWidth;

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
                            float pushX = (directionY == 1) ? Velocity.x : 0;
                            float pushY = Velocity.y - (hit.distance - skinWidth) * directionY;

                            passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                        }
                    }
                }
            }
            
            if (Velocity.x != 0)
            {
                float rayLength = Mathf.Abs(Velocity.x) + skinWidth;

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
                            float pushX = Velocity.x - (hit.distance - skinWidth) * directionX;
                            float pushY = -skinWidth;
                            passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                        }
                    }
                }
            }
            
            if (directionY == -1 || Velocity.y == 0 && Velocity.x != 0)
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
                            float pushX = Velocity.x;
                            float pushY = Velocity.y;
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
                rightOrigin += Vector2.up * (horizontalRaySpacing * (i + 1));

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask); 
                RaycastHit2D findPlayerLeft = Physics2D.Raycast(leftOrigin, Vector2.left, findPlayerLength, passengerMask);
                RaycastHit2D findPlayerRight = Physics2D.Raycast(rightOrigin, Vector2.right, findPlayerLength, passengerMask);

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
            }
        }

        void VerticalCollisions(ref Vector2 moveAmount)
        {
            float directionY = collisions.verticalDirection;
            float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;
            
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
                RaycastHit2D findPlayerUp = Physics2D.Raycast(topOrigin, Vector2.up, findPlayerLength, passengerMask);
                RaycastHit2D findPlayerDown = Physics2D.Raycast(botOrigin, Vector2.down, findPlayerLength, passengerMask);

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
