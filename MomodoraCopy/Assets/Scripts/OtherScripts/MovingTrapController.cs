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
        [Range(0, 2)]
        public float easeAmount;
        float nextMoveTime;
        List<PassengerMovement> passengerMovement;
        Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

        public Vector3 velocity;
        public Vector2 direction;

        public bool findPlayer;

        public Vector3 horizontalAreaSize;
        public Vector3 verticalAreaSize;

        public float insideRadius;
        public float outsideRadius;
        bool inOutSide;
        bool inInside;

        public float currentTime;

        public override void Start()
        {
            base.Start();
            collisions.faceDir = transform.rotation.y == 0 ? 1 : -1;
            direction = new Vector2(0, 0);
            //InvokeRepeating("FindPlayer", 0, 0.01f);
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
                    currentTime = 0;
                    findPlayer = true;
                    break;
                }
            }

            Collider2D[] verticalColls = Physics2D.OverlapBoxAll(transform.position, verticalAreaSize, 0);
            foreach (Collider2D collider in verticalColls)
            {
                if (collider.transform.CompareTag("Player") && !findPlayer)
                {
                    direction.y = collider.transform.position.y < transform.position.y ? -1 : 1;
                    direction.x = 0;
                    currentTime = 0;
                    findPlayer = true;
                    break;
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

            FindPlayer();
            Debug.Log(collisions.left);
            Debug.Log(collisions.right);
            Debug.Log(collisions.above); //above랑 below가 벽에 닿아도 계속 true로 바뀌게해야댐
            Debug.Log(collisions.below);
        }

        void CheckVerticalCollision()
        {
            if (collisions.left || collisions.right)
            {
                direction.x = 0;
                velocity.x = 0;
                if(currentTime <= 0)
                {
                    currentTime = waitTime;
                }
                currentTime -= Time.deltaTime;
                if (currentTime <= 0)
                {
                    ResetFindPlayer();
                }
            }
            if (collisions.above || collisions.below)
            {
                direction.y = 0;
                velocity.y = 0;
                if (currentTime <= 0)
                {
                    currentTime = waitTime;
                }
                currentTime -= Time.deltaTime;
                if (currentTime <= 0)
                {
                    ResetFindPlayer();
                }
            }
        }


        float Ease(float x)
        {
            float a = easeAmount + 1;
            return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
        }

        void CalculateVelocity()
        {
            velocity.x = direction.x * speed * Time.deltaTime;
            velocity.y = direction.y * speed * Time.deltaTime;
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
                        if (collisions.above || collisions.below)
                        {
                            return;
                        }
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
                collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
            }

            HorizontalCollisions(ref newMoveAmount);
            if (moveAmount.y != 0)
            {
                VerticalCollisions(ref newMoveAmount);
            }

            transform.Translate(newMoveAmount, Space.World);
        }

        public void HorizontalCollisions(ref Vector2 moveAmount)
        {
            float directionX = collisions.faceDir;
            float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

            if (Mathf.Abs(moveAmount.x) < skinWidth)
            {
                rayLength = 2 * skinWidth;
            }

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;

                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

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
            }
        }

        void VerticalCollisions(ref Vector2 moveAmount)
        {
            float directionY = Mathf.Sign(moveAmount.y);
            float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {

                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

                if (hit)
                {
                    moveAmount.y = (hit.distance - skinWidth) * directionY;
                    rayLength = hit.distance;

                    collisions.below = directionY == -1;
                    collisions.above = directionY == 1;
                }
            }

        }



        public struct CollisionInfo
        {
            public bool above, below;
            public bool left, right;

            public Vector2 moveAmountOld;
            public int faceDir;

            public void Reset()
            {
                above = below = false;
                left = right = false;
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, horizontalAreaSize);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, verticalAreaSize);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, insideRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, outsideRadius);
        }
    }
}
