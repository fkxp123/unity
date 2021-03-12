using UnityEngine;
using System.Collections.Generic;

namespace MomodoraCopy
{
    public class PushBlockController : RaycastController
    {
        public CollisionInfo collisions;

        public LayerMask passengerMask;

        List<PassengerMovement> passengerMovement;
        Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

        public Vector3 velocity;
        //public Vector3 velocity
        //{
        //    get { return _velocity; }
        //    set
        //    {
        //        _velocity = value;

        //        if (collisions.below)
        //        {
        //            if (value.x > 0)
        //            {
        //                dustEffect.transform.position = boxCollider.bounds.min;
        //                dustEffect.transform.rotation = Quaternion.Euler(0, 180, 0);
        //            }
        //            else if (value.x < 0)
        //            {
        //                dustEffect.transform.position = new Vector3(boxCollider.bounds.max.x, boxCollider.bounds.min.y, 0);
        //                dustEffect.transform.rotation = Quaternion.Euler(0, 0, 0);
        //            }
        //            dustEffect.Play();
        //        }
        //    }
        //}
        BoxCollider2D boxCollider;
        public ParticleSystem dustEffect;

        float gravity;

        Vector3 currentVelocity;
        Vector3 previousVelocity;

        public override void Start()
        {
            base.Start();

            gravity = -1f;
            boxCollider = GetComponent<BoxCollider2D>();
        }
        void CalculateCurrentVelocity()
        {
            currentVelocity = (transform.position - previousVelocity) / Time.deltaTime;
            previousVelocity = transform.position;

            //if (collisions.below)
            //{
            //    if (currentVelocity.x > 0)
            //    {
            //        dustEffect.transform.position = boxCollider.bounds.min;
            //        dustEffect.transform.rotation = Quaternion.Euler(0, 180, 0);
            //    }
            //    else if (currentVelocity.x < 0)
            //    {
            //        dustEffect.transform.position = new Vector3(boxCollider.bounds.max.x, boxCollider.bounds.min.y, 0);
            //        dustEffect.transform.rotation = Quaternion.Euler(0, 0, 0);
            //    }
            //    dustEffect.Play();
            //}
        }
        bool playOnce;

        void Update()
        {
            UpdateRaycastOrigins();

            CalculateCurrentVelocity();
            CheckVerticalCollisions();
        }

        void CheckVerticalCollisions()
        {
            if (collisions.below)
            {
                Debug.Log("belcw");
                velocity.y = 0;
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
                velocity.y = Mathf.Clamp(velocity.y, -50, 50);
                Move(velocity);
            }
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
            CalculatePassengerMovement(moveAmount);

            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll
                (boxCollider.bounds.center, new Vector3(boxCollider.bounds.size.x, boxCollider.bounds.size.y * -0.5f, 0), 0);
            foreach (Collider2D collider in collider2Ds)
            {
                if (collider.transform.tag == "Player")
                {
                    if (transform.position.x > collider.transform.position.x)
                    {
                        dustEffect.transform.position = boxCollider.bounds.min + Vector3.up * 0.2f ;
                        dustEffect.transform.rotation = Quaternion.Euler(0, 180, 0);
                        dustEffect.Emit(1);
                    }
                    else if (transform.position.x < collider.transform.position.x)
                    {
                        dustEffect.transform.position = new Vector3(boxCollider.bounds.max.x, boxCollider.bounds.min.y + 0.2f, 0);
                        dustEffect.transform.rotation = Quaternion.Euler(0, 0, 0);
                        dustEffect.Emit(1);
                    }
                }
            }

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