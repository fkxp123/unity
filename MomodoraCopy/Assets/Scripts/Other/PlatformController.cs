using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace MomodoraCopy
{
    public class PlatformController : RaycastController
    {
        public CollisionInfo collisions;

        public LayerMask passengerMask;

        public Vector3[] localWaypoints;
        Vector3[] globalWaypoints;

        public float speed;
        public bool cyclic;
        public float waitTime;
        [Range(0, 2)]
        public float easeAmount;

        int fromWaypointIndex;
        float percentBetweenWaypoints;
        float nextMoveTime;

        List<PassengerMovement> passengerMovement;
        Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

        Vector3 velocity;

        public bool isOnce;
        bool moveFinish;

        public float startDelay;
        WaitForSeconds waitDelay;
        bool delayFinish;

        Coroutine shakeRoutine;
        public bool isShake;

        public override void Start()
        {
            base.Start();

            globalWaypoints = new Vector3[localWaypoints.Length];
            for (int i = 0; i < localWaypoints.Length; i++)
            {
                globalWaypoints[i] = localWaypoints[i] + transform.position;
            }

            waitDelay = new WaitForSeconds(startDelay);
            StartCoroutine(WaitDelay());
        }

        void OnEnable()
        {
        }

        IEnumerator ShakeCamera()
        {
            while (true)
            {
                CameraManager.instance.ShakeCamera(.25f, .1f);
                yield return null;
            }
        }

        IEnumerator WaitDelay()
        {
            yield return waitDelay;
            delayFinish = true;
            shakeRoutine = StartCoroutine(ShakeCamera());
        }

        void Update()
        {
            if (!delayFinish)
            {
                return;
            }
            UpdateRaycastOrigins();

            if (moveFinish)
            {
                return;
            }
            velocity = CalculatePlatformMovement();

            CalculatePassengerMovement(velocity);
            MovePassengers(true);
            Move(velocity);
            MovePassengers(false);
        }

        float Ease(float x)
        {
            float a = easeAmount + 1;
            return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
        }

        Vector3 CalculatePlatformMovement()
        {
            if (Time.time < nextMoveTime)
            {
                return Vector3.zero;
            }

            fromWaypointIndex %= globalWaypoints.Length;
            int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
            float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
            percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
            percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
            float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

            Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

            if (percentBetweenWaypoints >= 1)
            {
                percentBetweenWaypoints = 0;
                fromWaypointIndex++;

                if (isOnce)
                {
                    moveFinish = true;
                    StopCoroutine(shakeRoutine);
                    return Vector3.zero;
                }

                if (!cyclic)
                {
                    if (fromWaypointIndex >= globalWaypoints.Length - 1)
                    {
                        fromWaypointIndex = 0;
                        System.Array.Reverse(globalWaypoints);
                    }
                }
                nextMoveTime = Time.time + waitTime;
            }

            return newPos - transform.position;
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
                    if(passengerDictionary[passenger.transform] == null)
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
            UpdateRaycastOrigins();

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

            #region physics2d overlap
            //// Vertically moving platform
            //if (velocity.y != 0)
            //{
            //    Vector2 velocityAmount = velocity;

            //    Collider2D[] collider2Ds = (directionY == 1) ?
            //        Physics2D.OverlapAreaAll(uRaycastOrigins.topRight + velocityAmount, uRaycastOrigins.topLeft + velocityAmount, passengerMask) :
            //        Physics2D.OverlapAreaAll(uRaycastOrigins.bottomRight + velocityAmount, uRaycastOrigins.bottomLeft + velocityAmount, passengerMask);
            //    foreach (Collider2D collider in collider2Ds)
            //    {
            //        if (!movedPassengers.Contains(collider.transform))
            //        {
            //            movedPassengers.Add(collider.transform);
            //            float pushX = (directionY == 1) ? velocity.x : 0;
            //            float pushY = velocity.y;

            //            passengerMovement.Add(new PassengerMovement(collider.transform, new Vector3(pushX, pushY), directionY == 1, true));
            //        }
            //    }
            //}

            //// Horizontally moving platform
            //if (velocity.x != 0)
            //{
            //    Vector2 velocityAmount = velocity;

            //    Collider2D[] collider2Ds = (directionX == 1) ?
            //        Physics2D.OverlapAreaAll(uRaycastOrigins.bottomRight + velocityAmount, uRaycastOrigins.topRight + velocityAmount, passengerMask) :
            //        Physics2D.OverlapAreaAll(uRaycastOrigins.bottomLeft + velocityAmount, uRaycastOrigins.topLeft + velocityAmount, passengerMask);
            //    foreach (Collider2D collider in collider2Ds)
            //    {
            //        if (!movedPassengers.Contains(collider.transform))
            //        {
            //            movedPassengers.Add(collider.transform);
            //            float pushX = velocity.x;
            //            float pushY = -skinWidth;

            //            passengerMovement.Add(new PassengerMovement(collider.transform, new Vector3(pushX, pushY), false, true));
            //        }
            //    }
            //}

            //// Passenger on top of a horizontally or downward moving platform
            //if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
            //{
            //    Collider2D[] collider2Ds =
            //        Physics2D.OverlapAreaAll(uRaycastOrigins.topRight, uRaycastOrigins.topLeft, passengerMask);
            //    foreach (Collider2D collider in collider2Ds)
            //    {
            //        if (!movedPassengers.Contains(collider.transform))
            //        {
            //            movedPassengers.Add(collider.transform);
            //            float pushX = velocity.x;
            //            float pushY = velocity.y;

            //            passengerMovement.Add(new PassengerMovement(collider.transform, new Vector3(pushX, pushY), true, false));
            //        }
            //    }
            //}
            #endregion

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

        void OnDrawGizmos()
        {
            if (localWaypoints != null)
            {
                Gizmos.color = Color.red;
                float size = .3f;

                for (int i = 0; i < localWaypoints.Length; i++)
                {
                    Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                    Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                    Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
                }
            }
        }

    }
}