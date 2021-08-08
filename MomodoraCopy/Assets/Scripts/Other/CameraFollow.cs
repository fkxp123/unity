using UnityEngine;

namespace MomodoraCopy
{
    public class CameraFollow : MonoBehaviour
    {
        static CameraFollow instance;
        public Controller2D target;
        public float verticalOffset;
        public float lookAheadDstX;
        public float lookSmoothTimeX;
        public float verticalSmoothTime;
        public Vector2 focusAreaSize;

        FocusArea focusArea;

        [HideInInspector]
        public float currentLookAheadX;
        [HideInInspector]
        public float targetLookAheadX;
        [HideInInspector]
        public float lookAheadDirX;
        [HideInInspector]
        public float smoothLookVelocityX;
        [HideInInspector]
        public float smoothVelocityY;
        [HideInInspector]
        public Vector2 focusPosition;

        bool lookAheadStopped;

        [SerializeField]
        bool stopCameraFollow;
        [SerializeField]
        bool isCrouchToNormal;
        float crouchHangTime = 1.0f;
        float currentHangTime;
        float cameraMoveAmount = 0.5f;
        float cameraSmoothTime = 0.25f;
        float currentSmoothTime;
        float crouchCameraDistance = 4.0f;
        float cameraSmoothVelocity;
        float startPosY;
        Vector3 crouchCameraPos;
        Player player;

        BoxCollider2D targetCollider;
        Transform cameraTargetBounds;

        void Awake()
        {
            if (instance == null)
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            cameraTargetBounds = target.transform.GetChild(1);
            targetCollider = cameraTargetBounds.GetComponent<BoxCollider2D>();
            focusArea = new FocusArea(targetCollider.bounds, focusAreaSize);
            player = target.GetComponent<Player>();
            DontDestroyOnLoad(gameObject);
        }

        //if you want player set crouchCameraMovement
        //void Update()
        //{
        //    CheckPlayerCrouch();
        //    CheckCrouchHangTime();
        //}
        //void CheckPlayerCrouch()
        //{
        //    if (player.stateMachine.CurrentState == player.crouch)
        //    {
        //        if (currentHangTime > 0)
        //        {
        //            startPosY = transform.position.y;
        //        }
        //        stopCameraFollow = true;
        //        currentHangTime -= Time.deltaTime;
        //        isCrouchToNormal = true;
        //        currentSmoothTime = cameraSmoothTime;
        //    }
        //    else
        //    {
        //        currentHangTime = crouchHangTime;
        //        stopCameraFollow = false;
        //    }
        //}
        //void OperateCameraCrouch()
        //{
        //    if (currentHangTime < 0)
        //    {
        //        crouchCameraPos.y = Mathf.SmoothDamp(transform.position.y, startPosY - crouchCameraDistance,
        //            ref cameraMoveAmount, cameraSmoothTime);
        //        transform.position = new Vector3(transform.position.x, crouchCameraPos.y, transform.position.z);
        //    }

        //}
        //void CheckCrouchHangTime()
        //{
        //    if (isCrouchToNormal)
        //    {
        //        currentSmoothTime = Mathf.SmoothDamp(currentSmoothTime, 0, ref cameraSmoothVelocity, 0.5f);
        //        verticalSmoothTime = currentSmoothTime;
        //        if (verticalSmoothTime < 0.0001)
        //        {
        //            isCrouchToNormal = false;
        //        }
        //    }
        //}

        void LateUpdate()
        {
            //if (stopCameraFollow)
            //{
            //    //OperateCameraCrouch();
            //    return;
            //}

            focusArea.Update(targetCollider.bounds);

            focusPosition = focusArea.center + Vector2.up * verticalOffset;

            if (focusArea.velocity.x != 0)
            {
                lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
                if (Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0)
                {
                    lookAheadStopped = false;
                    targetLookAheadX = lookAheadDirX * lookAheadDstX;
                }
                else
                {
                    if (!lookAheadStopped)
                    {
                        lookAheadStopped = true;
                        targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4f;
                    }
                }
            }

            currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

            focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
            focusPosition += Vector2.right * currentLookAheadX;
            transform.position = (Vector3)focusPosition + Vector3.forward * -10;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, .5f);
            Gizmos.DrawCube(focusArea.center, focusAreaSize);
        }

        struct FocusArea
        {
            public Vector2 center;
            public Vector2 velocity;
            float left, right;
            float top, bottom;


            public FocusArea(Bounds targetBounds, Vector2 size)
            {
                left = targetBounds.center.x - size.x / 2;
                right = targetBounds.center.x + size.x / 2;
                bottom = targetBounds.min.y;
                top = targetBounds.min.y + size.y;

                velocity = Vector2.zero;
                center = new Vector2((left + right) / 2, (top + bottom) / 2);
            }

            public void Update(Bounds targetBounds)
            {
                float shiftX = 0;
                if (targetBounds.min.x < left)
                {
                    shiftX = targetBounds.min.x - left;
                }
                else if (targetBounds.max.x > right)
                {
                    shiftX = targetBounds.max.x - right;
                }
                left += shiftX;
                right += shiftX;

                float shiftY = 0;
                if (targetBounds.min.y < bottom)
                {
                    shiftY = targetBounds.min.y - bottom;
                }
                else if (targetBounds.max.y > top)
                {
                    shiftY = targetBounds.max.y - top;
                }
                top += shiftY;
                bottom += shiftY;
                center = new Vector2((left + right) / 2, (top + bottom) / 2);
                velocity = new Vector2(shiftX, shiftY);
            }
        }

    }
}