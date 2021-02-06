using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class MovingTrapController : RaycastController
    {
        public CollisionInfo collisions;

        public LayerMask passengerMask;
        public LayerMask findMask;

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
                }
                velocity = value;
            }
        }
        public Vector2 direction;

        public bool findPlayer;

        public Vector3 horizontalAreaSize;
        public Vector3 verticalAreaSize;

        public bool isFirst;

        public ParticleSystem dustCloudEffect;      

        public override void Start()
        {
            base.Start();
            collisions.hoziontalDirection = transform.rotation.y == 0 ? 1 : -1;
            collisions.verticalDirection = -1;
            direction = new Vector2(0, 0);
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
        }

        void CheckVerticalCollision()
        {
            if(collisions.left || collisions.right || collisions.above || collisions.below)
            {
                if (!isFirst)
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
                    Invoke("ResetFindPlayer", waitTime);
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

            float directionX = Mathf.Sign(velocity.x);
            float directionY = Mathf.Sign(velocity.y);
            
            if (velocity.y != 0)
            {
                Vector2 velocityAmount = velocity;

                Collider2D[] collider2Ds = (directionY == 1) ?
                    Physics2D.OverlapAreaAll(raycastOrigins.topRight, uRaycastOrigins.topLeft + velocityAmount, passengerMask) :
                    Physics2D.OverlapAreaAll(raycastOrigins.bottomRight, uRaycastOrigins.bottomLeft + velocityAmount, passengerMask);

                foreach(Collider2D collider in collider2Ds)
                {
                    float hitDistance = (directionY == 1) ?
                        Mathf.Abs(collider.bounds.min.y - raycastOrigins.topRight.y):
                        Mathf.Abs(collider.bounds.max.y - raycastOrigins.bottomRight.y);

                    if (!movedPassengers.Contains(collider.transform))
                    {
                        movedPassengers.Add(collider.transform);
                        float pushX = (directionY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (hitDistance - skinWidth) * directionY;

                        passengerMovement.Add(new PassengerMovement(collider.transform, new Vector3(pushX, pushY), directionY == 1, true));
                    }
                }
            }
            
            if (velocity.x != 0)
            {
                Vector2 velocityAmount = velocity;

                Collider2D[] collider2Ds = (directionX == 1) ?
                    Physics2D.OverlapAreaAll(raycastOrigins.bottomRight, uRaycastOrigins.topRight + velocityAmount, passengerMask) :
                    Physics2D.OverlapAreaAll(raycastOrigins.bottomLeft, uRaycastOrigins.topLeft + velocityAmount, passengerMask);

                foreach (Collider2D collider in collider2Ds)
                {
                    float hitDistance = (directionX == 1) ?
                        Mathf.Abs(collider.bounds.min.x - raycastOrigins.bottomRight.x) :
                        Mathf.Abs(collider.bounds.max.x - raycastOrigins.bottomLeft.x);

                    if (!movedPassengers.Contains(collider.transform))
                    {
                        movedPassengers.Add(collider.transform);
                        float pushX = velocity.x - (hitDistance - skinWidth) * directionX;
                        float pushY = -skinWidth;

                        passengerMovement.Add(new PassengerMovement(collider.transform, new Vector3(pushX, pushY), false, true));
                    }
                }
            }

            if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
            {
                Collider2D[] collider2Ds = 
                    Physics2D.OverlapAreaAll(uRaycastOrigins.topRight, uRaycastOrigins.topLeft, passengerMask);

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
                RaycastHit2D findPlayerLeft = Physics2D.Raycast(leftOrigin, Vector2.left, findPlayerLength, findMask);
                RaycastHit2D findPlayerRight = Physics2D.Raycast(rightOrigin, Vector2.right, findPlayerLength, findMask);

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
                RaycastHit2D findPlayerUp = Physics2D.Raycast(topOrigin, Vector2.up, findPlayerLength, findMask);
                RaycastHit2D findPlayerDown = Physics2D.Raycast(botOrigin, Vector2.down, findPlayerLength, findMask);

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
