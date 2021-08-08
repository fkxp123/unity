﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MomodoraCopy
{
    public class PoisonBombController : RaycastController
    {
        public CollisionInfo collisions;

        Vector3 velocity;
        public float speed;

        BoxCollider2D boxCollider;

        float gravity;

        Vector3 currentVelocity;
        Vector3 previousVelocity;

        public Vector2 direction;
        float friction = 2f;

        float velocityXSmoothing;
        float accelerationTimeAirborne = .2f;
        float accelerationTimeGrounded = .1f;

        public ParticleSystem poisonBombEffect;
        public ParticleSystem poisonBombCollision;
        Transform sprite;

        PoisonBombSpawner poisonBombSpawner;
        [SerializeField]
        LayerMask playerMask;

        float bombRadius;
        float poisonBombDamage;

        float accel = 1;
        public bool isParabolla;
        Vector3 aimDirection;

        public override void Start()
        {
            base.Start();
            sprite = transform.GetChild(0);
            gravity = 50f;
            //velocity.y = 20;
            //speed = 20;
            boxCollider = GetComponent<BoxCollider2D>();
            playerMask = 1 << 8;
            bombRadius = 0.2f;
            poisonBombDamage = 10f;
            try
            {
                poisonBombSpawner = transform.parent.transform.parent.GetComponent<PoisonBombSpawner>();
            }
            catch
            {
                poisonBombSpawner = null;
            }
        }

        void OnEnable()
        {
            if (GameManager.instance == null)
            {
                return;
            }
            aimDirection = Vector3.zero;
            if (isParabolla)
            {
                //speed = 10;
                speed = Mathf.Abs(transform.parent.transform.position.x - GameManager.instance.playerPhysics.transform.position.x) + 5;
                velocity.y = Mathf.Abs(speed);
                direction.x = transform.parent.transform.rotation.y == 0 ? 1 : -1;
                direction.y = -1;
                return;
            }
            speed = 40;
            velocity.y = 0;
            aimDirection = (GameManager.instance.playerPhysics.transform.position - transform.position).normalized;
        }
        void OnDisable()
        {
            poisonBombEffect.transform.position = transform.position;
            poisonBombCollision.transform.position = transform.position;
            poisonBombEffect.Play();
            poisonBombCollision.Play();
        }

        void Update()
        {
            UpdateRaycastOrigins();
            CalculateVelocity();
            sprite.Rotate(0, 0, -10);
            if (isParabolla)
            {
                Move(velocity * Time.deltaTime);
            }
            else
            {
                Move(velocity * Time.deltaTime);
            }
            CheckPlayerCollisions();
            CheckVerticalCollisions();
        }
        void CalculateVelocity()
        {
            //float targetVelocityX = direction.x * speed;
            //velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
            //speed -= friction * Time.deltaTime;
            //speed -= 0.5f * speed * speed * direction.x * 0.1f * Time.deltaTime;
            //speed = Mathf.Clamp(speed, 0, 50);
            //friction += -1 * direction.x * Time.deltaTime;
            //friction = direction.x == 1 ? Mathf.Clamp(friction, -10, 0) : Mathf.Clamp(friction, 0, 10);
            //velocity.x -= friction * Time.deltaTime;

            if (isParabolla)
            {
                velocity.x = direction.x * speed;
                velocity.y += gravity * direction.y * Time.deltaTime;
                velocity.y = Mathf.Clamp(velocity.y, -50, 50);
                return;
            }
            velocity = aimDirection * speed;
        }

        void CheckPlayerCollisions()
        {
            bool isPlayer = Physics2D.OverlapCircle(transform.position, bombRadius, playerMask);
            if (isPlayer)
            {
                gameObject.SetActive(false);
                GameManager.instance.playerPhysics.transform.GetChild(0)
                    .GetComponent<PlayerStatus>().TakeDamage(poisonBombDamage, DamageType.Poisoned, 
                    transform.parent.transform.rotation);
            }
            bool isCollision = Physics2D.OverlapCircle(transform.position, bombRadius, collisionMask);
            if (isCollision)
            {
                gameObject.SetActive(false);
            }
        }

        void CheckVerticalCollisions()
        {
            if (collisions.left || collisions.right)
            {
                direction.x = 0;
                velocity.x = 0;
                //velocity.x -= (direction.x == 1 ? -1 : 1) * 2f * Time.deltaTime;
                //direction.x *= -1;
                //poisonBombEffect.transform.position = transform.position;
                //poisonBombEffect.Play();
                gameObject.SetActive(false);
            }
            if (collisions.above || collisions.below)
            {
                //friction += speed * 100 * Time.deltaTime;
                //speed -= friction * Time.deltaTime;
                //speed -= 0.5f * speed * speed * direction.x * 0.1f * Time.deltaTime;
                //speed = Mathf.Clamp(speed, 0, 50);
                direction.x = 0;
                velocity.x = 0;
                direction.y = 0;
                velocity.y = 0;
                //poisonBombEffect.transform.position = transform.position;
                //poisonBombEffect.Play();
                gameObject.SetActive(false);
            }
            else
            {
                direction.y = -1;
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

        public void Move(Vector2 moveAmount)
        {
            Vector2 newMoveAmount = moveAmount;

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

            transform.parent.transform.Translate(newMoveAmount, Space.World);
            velocity.x = 0;
            //direction.x = 0;
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

        public void SetAccel(float value)
        {
            accel = value;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, bombRadius);
        }
    }
}