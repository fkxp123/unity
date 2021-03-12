using UnityEngine;
using System.Collections.Generic;

namespace MomodoraCopy
{
    public class ElevatorController : RaycastController
    {
        public CollisionInfo collisions;

        public LayerMask passengerMask;

        public float speed;
        public float waitTime;

        float nextMoveTime;
        Vector3 startPosition;
        Vector3 endPosition;

        List<PassengerMovement> passengerMovement;
        Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

        Vector3 velocity;

        float direction;
        float moveAmount;

        public override void Start()
        {
            base.Start();

            startPosition = transform.position;
            direction = -1;
        }

        void Update()
        {
            UpdateRaycastOrigins();

            velocity = CalculatePlatformMovement();

            CalculatePassengerMovement(velocity);

            Move(velocity);
        }

        Vector3 CalculatePlatformMovement()
        {
            Vector3 velocity;
            if (Time.time < nextMoveTime)
            {
                return Vector3.zero;
            }
            if(direction == 1)
            {
                Vector3 newPos = Vector3.Lerp(endPosition, startPosition, moveAmount);
                moveAmount += Time.deltaTime * speed / Mathf.Abs(startPosition.y - endPosition.y);
                velocity = newPos - transform.position;
                if(transform.position == startPosition || collisions.above)
                {
                    direction *= -1;
                    nextMoveTime = Time.time + waitTime;
                    moveAmount = 0;
                }
            }
            else
            {
                velocity = new Vector3(0, direction * speed * Time.deltaTime);
                if (collisions.below)
                {
                    direction *= -1;
                    endPosition = transform.position;
                    nextMoveTime = Time.time + waitTime;
                    moveAmount = 0;
                }
            }
            return velocity;
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
                    if (passengerDictionary[passenger.transform] == null)
                    {
                        return;
                    }
                    passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
                }
            }
        }

        public void HorizontalCollisions(ref Vector2 moveAmount)
        {
            float directionX = collisions.hoziontalDirection;
            float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

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

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

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
            float directionY = collisions.verticalDirection;
            float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

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

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

                if (hit)
                {
                    moveAmount.y = (hit.distance - skinWidth) * directionY;
                    rayLength = hit.distance;

                    collisions.below = directionY == -1;
                    collisions.above = directionY == 1;
                }
            }
        }

        public void Move(Vector2 moveAmount)
        {
            MovePassengers(true);

            Vector2 newMoveAmount = moveAmount;
            collisions.Reset();
            collisions.moveAmountOld = moveAmount;

            if (moveAmount.x != 0)
            {
                collisions.hoziontalDirection = (int)Mathf.Sign(moveAmount.x);
            }
            if (moveAmount.y != 0)
            {
                collisions.verticalDirection = (int)Mathf.Sign(moveAmount.y);
            }

            if (moveAmount.x == 0)
            {
                VerticalCollisions(ref newMoveAmount);
            }
            if (moveAmount.y == 0)
            {
                HorizontalCollisions(ref newMoveAmount);
            }

            transform.Translate(newMoveAmount, Space.World);

            MovePassengers(false);
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

        void CalculatePassengerMovement(Vector3 velocity)
        {
            HashSet<Transform> movedPassengers = new HashSet<Transform>();
            passengerMovement = new List<PassengerMovement>();

            float directionX = Mathf.Sign(velocity.x);
            float directionY = Mathf.Sign(velocity.y);

            // Vertically moving platform
            if (velocity.y != 0)
            {
                Vector2 velocityAmount = velocity;

                Collider2D[] collider2Ds = (directionY == 1) ?
                    Physics2D.OverlapAreaAll(eRaycastOrigins.topRight, eRaycastOrigins.topLeft + velocityAmount, passengerMask) :
                    Physics2D.OverlapAreaAll(eRaycastOrigins.bottomRight, eRaycastOrigins.bottomLeft + velocityAmount, passengerMask);

                foreach (Collider2D collider in collider2Ds)
                {
                    if (!movedPassengers.Contains(collider.transform))
                    {
                        movedPassengers.Add(collider.transform);
                        float pushX = (directionY == 1) ? velocity.x : 0;
                        float pushY = velocity.y;

                        passengerMovement.Add(new PassengerMovement(collider.transform, new Vector3(pushX, pushY), directionY == 1, true));
                    }
                }
            }

            //Horizontally moving platform
            if (velocity.x != 0)
            {
                Vector2 velocityAmount = velocity;

                Collider2D[] collider2Ds = (directionX == 1) ?
                    Physics2D.OverlapAreaAll(eRaycastOrigins.bottomRight, eRaycastOrigins.topRight + velocityAmount, passengerMask) :
                    Physics2D.OverlapAreaAll(eRaycastOrigins.bottomLeft, eRaycastOrigins.topLeft + velocityAmount, passengerMask);
                foreach (Collider2D collider in collider2Ds)
                {
                    if (!movedPassengers.Contains(collider.transform))
                    {
                        movedPassengers.Add(collider.transform);
                        float pushX = velocity.x;
                        float pushY = -skinWidth;

                        passengerMovement.Add(new PassengerMovement(collider.transform, new Vector3(pushX, pushY), false, true));
                    }
                }
            }

            // Passenger on top of a horizontally or downward moving platform
            if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
            {
                Vector2 velocityAmount = Vector2.up * skinWidth * 2;

                Collider2D[] collider2Ds =
                    Physics2D.OverlapAreaAll(eRaycastOrigins.topRight, eRaycastOrigins.topLeft + velocityAmount, passengerMask);
                foreach (Collider2D collider in collider2Ds)
                {
                    if (!movedPassengers.Contains(collider.transform))
                    {
                        movedPassengers.Add(collider.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        passengerMovement.Add(new PassengerMovement(collider.transform, new Vector3(pushX, pushY), true, false));
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
    }
}