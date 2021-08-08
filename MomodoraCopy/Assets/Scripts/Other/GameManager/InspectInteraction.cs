using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class InspectInteraction : MonoBehaviour
    {
        public GameObject interactPoint;
        LayerMask playerMask;

        bool isNear;
        bool isInteracted;

        Rigidbody2D rigid;

        IInpsectInteraction inspectInteraction;

        void Start()
        {
            playerMask = 1 << 8;

            rigid = transform.GetComponent<Rigidbody2D>();
            rigid.bodyType = RigidbodyType2D.Kinematic;

            inspectInteraction = GetComponent<IInpsectInteraction>();
        }

        void Update()
        {
            if (isInteracted)
            {
                return;
            }
            if (isNear)
            {
                if (Input.GetKeyDown(KeyboardManager.instance.UpKey))
                {
                    inspectInteraction.InspectInteract();
                    isInteracted = true;
                }
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if(other.tag == "Player")
            {
                if (isInteracted)
                {
                    return;
                }
                GameManager.instance.OpenInspectBox(interactPoint.transform.position);
            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                isNear = true;

                if (isInteracted)
                {
                    GameManager.instance.CloseInspectBox();
                    return;
                }

            }
        }
        void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                isNear = false;
                GameManager.instance.CloseInspectBox();
            }
        }
    }
}