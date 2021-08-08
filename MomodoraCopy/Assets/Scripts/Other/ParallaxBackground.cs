using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class ParallaxBackground : MonoBehaviour
    {
        public Vector2 parallaxMultiplier;
        public bool infiniteHorizontal;
        public bool infiniteVertical;

        [SerializeField]
        Transform cameraTransform;
        Vector3 lastCameraPosition;
        float textureUnitSizeX;
        float textureUnitSizeY;
        [SerializeField]
        GameObject mainCamera;
        CameraManager cameraManager = CameraManager.instance;

        Transform originTransform;
        bool isMapChanged;

        void Awake()
        {
            originTransform = transform.parent;
        }

        void Start()
        {
            mainCamera = GameManager.instance.mainCameraObject;
            cameraTransform = mainCamera.transform;

            lastCameraPosition = cameraTransform.position;
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            Texture2D texture = sprite.texture;
            textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
            textureUnitSizeY = texture.height / sprite.pixelsPerUnit;

            EventManager.instance.AddListener(EventType.MapChange, OnMapChange);
        }

        void OnDisable()
        {
            EventManager.instance.UnsubscribeEvent(EventType.MapChange, OnMapChange);
        }

        void OnMapChange()
        {
            transform.SetParent(mainCamera.transform);
            transform.position = new Vector3(mainCamera.transform.position.x, transform.position.y, transform.position.z);
            isMapChanged = true;
        }

        void LateUpdate()
        {
            if (cameraTransform == null)
            {
                mainCamera = GameManager.instance.mainCameraObject;
                cameraTransform = mainCamera.transform;
                return;
            }
            Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

            transform.position += new Vector3(deltaMovement.x * parallaxMultiplier.x,
                deltaMovement.y * parallaxMultiplier.y, 0);
            if (isMapChanged)
            {
                if (lastCameraPosition != cameraTransform.position)
                {
                    transform.SetParent(originTransform);
                    isMapChanged = false;
                }
            }
            lastCameraPosition = cameraTransform.position;

            if (infiniteHorizontal)
            {
                if(Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
                {
                    //i dont know why but this was fit to me
                    float offsetPositionX = (cameraTransform.position.x - transform.position.x) < 0 ?
                        -transform.localScale.x : transform.localScale.x;
                    //float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
                    //why %??
                    transform.position = new Vector3(cameraTransform.position.x + offsetPositionX,
                        transform.position.y, transform.position.z);
                }
            }

            if (infiniteVertical)
            {
                if (Mathf.Abs(cameraTransform.position.y - transform.position.y) >= textureUnitSizeY)
                {
                    //float offsetPositionY = (cameraTransform.position.y - transform.position.y) % textureUnitSizeY;
                    float offsetPositionY = (cameraTransform.position.y - transform.position.y) < 0 ?
                        -transform.localScale.y : transform.localScale.y;
                    transform.position = new Vector3(transform.position.x, cameraTransform.position.y + offsetPositionY,
                        transform.position.z);
                }
            }

        }
    }

}