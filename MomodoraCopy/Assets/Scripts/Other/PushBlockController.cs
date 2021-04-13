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

        public Vector2 direction;
        float speed = 3;

        public float targetVelocityX;

        public override void Start()
        {
            base.Start();
            gravity = -50f;
            collisions.below = true;
            //start()에서 true로 설정한 이유 : 
            //level generator에서 collisions 체크가 한 프레임 늦게되서 벽밑으로 빨려들어감(왜인지는 모름)
            boxCollider = GetComponent<BoxCollider2D>();
        }
        void CalculateCurrentVelocity()
        {
            currentVelocity = (transform.position - previousVelocity) / Time.deltaTime;
            previousVelocity = transform.position;
            //Debug.Log(currentVelocity);

        }

        void Update()
        {
            UpdateRaycastOrigins();
            CalculateVelocity();
            //CalculateCurrentVelocity();
            CalculatePassengerMovement(velocity * Time.deltaTime);
            Move(velocity * Time.deltaTime, true);
            CheckVerticalCollisions();
        }

        //public Vector2 direction;
        //float speed = 10;
        //void CalculateVelocity()
        //{
        //    velocity.x = direction.x * speed * Time.deltaTime;
        //    velocity.y += direction.y * speed * Time.deltaTime;

        //    //velocity = new Vector3(direction.x * speed * Time.deltaTime, direction.y * speed * Time.deltaTime);
        //}
        void CalculateVelocity()
        {
            //velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
            ////velocity.x = direction.x * speed * Time.deltaTime;

            //Vector3 v = velocity;
            //v.y += gravity * direction.y * Time.deltaTime;
            //v.y = Mathf.Clamp(v.y, -50, 50);
            //v.x = direction.x * speed;
            //return v;

            velocity.y += gravity * Time.deltaTime;
            velocity.y = Mathf.Clamp(velocity.y, -50, 50);
            velocity.x = direction.x * speed;
        }
        void CheckVerticalCollisions()
        {
            if (collisions.left || collisions.right)
            {
                direction.x = 0;
                velocity.x = 0;
            }
            if (collisions.above || collisions.below)
            {
                direction.y = 0;
                velocity.y = 0;
                //direction.x = -1;

                //targetVelocityX = -3f;
            }


            //if (collisions.below)
            //{
            //    velocity.y = 0;
            //}
            //else
            //{
            //    velocity.y += gravity * Time.deltaTime;
            //    velocity.y = Mathf.Clamp(velocity.y, -50, 50);
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

                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength);

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

                Debug.DrawRay(rayOrigin, Vector2.up * directionY);

                if (hit)
                {
                    moveAmount.y = (hit.distance - skinWidth) * directionY;
                    rayLength = hit.distance;

                    collisions.below = directionY == -1;
                    collisions.above = directionY == 1;
                }
            }
        }

        public void Move(Vector2 moveAmount, bool moveSelf)
        {
            //UpdateRaycastOrigins();
            //if (collisions.left || collisions.right)
            //{
            //    return;
            //}
            //CalculatePassengerMovement(velocity * Time.deltaTime);
            //if (moveSelf)
            //{
            //    CalculatePassengerMovement(velocity * Time.deltaTime);
            //}
            //else
            //{
            //    CalculatePassengerMovement(ref newMoveAmount);
            //}
            if(moveAmount.x > 0)
            {
                dustEffect.transform.position = boxCollider.bounds.min + Vector3.up * 0.2f;
                dustEffect.transform.rotation = Quaternion.Euler(0, 180, 0);
                dustEffect.Emit(1);
            }
            else if(moveAmount.x < 0)
            {
                dustEffect.transform.position = new Vector3(boxCollider.bounds.max.x, boxCollider.bounds.min.y + 0.2f, 0);
                dustEffect.transform.rotation = Quaternion.Euler(0, 0, 0);
                dustEffect.Emit(1);
            }
            //Collider2D[] collider2Ds = Physics2D.OverlapBoxAll
            //    (boxCollider.bounds.center, new Vector3(boxCollider.bounds.size.x, boxCollider.bounds.size.y * -0.5f, 0), 0);
            //foreach (Collider2D collider in collider2Ds)
            //{
            //    if (collider.transform.tag == "Player")
            //    {
            //        if (!moveSelf && transform.position.x > collider.transform.position.x)
            //        {
            //            dustEffect.transform.position = boxCollider.bounds.min + Vector3.up * 0.2f;
            //            dustEffect.transform.rotation = Quaternion.Euler(0, 180, 0);
            //            dustEffect.Emit(1);
            //        }
            //        else if (!moveSelf && transform.position.x < collider.transform.position.x)
            //        {
            //            dustEffect.transform.position = new Vector3(boxCollider.bounds.max.x, boxCollider.bounds.min.y + 0.2f, 0);
            //            dustEffect.transform.rotation = Quaternion.Euler(0, 0, 0);
            //            dustEffect.Emit(1);
            //        }
            //        if (moveAmount.x < 0 && transform.position.x > collider.transform.position.x)
            //        {
            //            return;
            //        }
            //        if (moveAmount.x > 0 && transform.position.x < collider.transform.position.x)
            //        {
            //            return;
            //        }
            //    }
            //}
            //if (!moveSelf)
            //{
            //    CalculatePassengerMovement(moveAmount);
            //}
            Vector2 newMoveAmount = moveAmount;

            MovePassengers(true);

            collisions.Reset();
            collisions.moveAmountOld = moveAmount;

            if (moveAmount.x != 0)
            {
                collisions.hoziontalDirection = (int)Mathf.Sign(moveAmount.x);
            }
            HorizontalCollisions(ref newMoveAmount);
            if (moveAmount.y != 0)
            {
                collisions.verticalDirection = (int)Mathf.Sign(moveAmount.y);
                VerticalCollisions(ref newMoveAmount);
            }

            if (moveAmount.x == 0)
            {
            }
            if (moveAmount.y == 0)
            {
            }

            transform.Translate(newMoveAmount, Space.World);
            velocity.x = 0;
            direction.x = 0;
            MovePassengers(false);
        }
        public void Move(Vector2 moveAmount)
        {
            UpdateRaycastOrigins();
            if (collisions.left || collisions.right)
            {
                return;
            }
            Vector2 newMoveAmount = moveAmount;
            CalculatePassengerMovement(moveAmount);
            //CalculatePassengerMovement(velocity * Time.deltaTime);
            //if (moveSelf)
            //{
            //    CalculatePassengerMovement(velocity * Time.deltaTime);
            //}
            //else
            //{
            //    CalculatePassengerMovement(ref newMoveAmount);
            //}

            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll
                (boxCollider.bounds.center, new Vector3(boxCollider.bounds.size.x, boxCollider.bounds.size.y * -0.5f, 0), 0);
            foreach (Collider2D collider in collider2Ds)
            {
                if (collider.transform.tag == "Player")
                {
                    if (transform.position.x > collider.transform.position.x)
                    {
                        dustEffect.transform.position = boxCollider.bounds.min + Vector3.up * 0.2f;
                        dustEffect.transform.rotation = Quaternion.Euler(0, 180, 0);
                        dustEffect.Emit(1);
                    }
                    else if (transform.position.x < collider.transform.position.x)
                    {
                        dustEffect.transform.position = new Vector3(boxCollider.bounds.max.x, boxCollider.bounds.min.y + 0.2f, 0);
                        dustEffect.transform.rotation = Quaternion.Euler(0, 0, 0);
                        dustEffect.Emit(1);
                    }
                    if (moveAmount.x < 0 && transform.position.x > collider.transform.position.x)
                    {
                        return;
                    }
                    if (moveAmount.x > 0 && transform.position.x < collider.transform.position.x)
                    {
                        return;
                    }
                }
            }
            MovePassengers(true);

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

            //velocity.x = newMoveAmount.x;
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

            if (velocity.y != 0 && !collisions.below)
            {
                Vector2 velocityAmount = velocity;

                Collider2D[] collider2Ds = (directionY == 1) ?
                    Physics2D.OverlapAreaAll(eRaycastOrigins.topRight, eRaycastOrigins.topLeft + velocityAmount, passengerMask) :
                    Physics2D.OverlapAreaAll(eRaycastOrigins.bottomRight, eRaycastOrigins.bottomLeft + velocityAmount, passengerMask);

                foreach (Collider2D collider in collider2Ds)
                {
                    float hitDistance = (directionY == 1) ?
                        Mathf.Abs(collider.bounds.min.y - raycastOrigins.topRight.y) :
                        Mathf.Abs(collider.bounds.max.y - raycastOrigins.bottomRight.y);

                    movedPassengers.Add(collider.transform);
                    float pushX = (directionY == 1) ? velocity.x : 0;
                    float pushY = velocity.y - (hitDistance - skinWidth) * directionY;

                    passengerMovement.Add(new PassengerMovement(collider.transform, new Vector3(pushX, pushY), directionY == 1, true));
                }
            }

            if (velocity.x != 0)
            {
                Vector2 velocityAmount = velocity;

                Collider2D[] collider2Ds = (directionX == 1) ?
                    Physics2D.OverlapAreaAll(eRaycastOrigins.bottomRight, eRaycastOrigins.topRight + velocityAmount, passengerMask) :
                    Physics2D.OverlapAreaAll(eRaycastOrigins.bottomLeft, eRaycastOrigins.topLeft + velocityAmount, passengerMask);

                foreach (Collider2D collider in collider2Ds)
                {
                    float hitDistance = (directionX == 1) ?
                        Mathf.Abs(collider.bounds.min.x - raycastOrigins.bottomRight.x) :
                        Mathf.Abs(collider.bounds.max.x - raycastOrigins.bottomLeft.x);

                    movedPassengers.Add(collider.transform);
                    float pushX = velocity.x - (hitDistance - skinWidth) * directionX;
                    float pushY = -skinWidth;

                    passengerMovement.Add(new PassengerMovement(collider.transform, new Vector3(pushX, pushY), false, true));
                }
            }

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