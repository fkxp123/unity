using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace MomodoraCopy
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class ChatInteraction : MonoBehaviour
    {
        public Vector3 checkPlayerArea;

        LayerMask playerMask;

        bool isNear;

        public string dialogueName;

        Rigidbody2D rigid;

        BoxCollider2D physicsBoxCollider;
        Animator interactBoxAnimator;

        void Start()
        {
            playerMask = 1 << 8;

            rigid = transform.GetComponent<Rigidbody2D>();
            rigid.bodyType = RigidbodyType2D.Kinematic;

            physicsBoxCollider = transform.parent.GetComponent<BoxCollider2D>();
            interactBoxAnimator = DialogueManager.instance.interactionBox.
                transform.GetChild(0).GetComponent<Animator>();
        }

        void Update()
        {
            if(TimelineManager.instance.currentPlayableDirector != null)
            {
                HideInteractionBox();
                return;
            }
            if (isNear)
            {
                if (!DialogueManager.instance.isChatting)
                {
                    if (Input.GetKeyDown(KeyboardManager.instance.UpKey))
                    {
                        DialogueManager.instance.isChatting = true;
                        HideInteractionBox();
                        DialogueManager.instance.ShowChatBox(dialogueName);
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyboardManager.instance.UpKey))
                    {
                        if (DialogueManager.instance.isTyping)
                        {
                            return;
                        }
                        if(DialogueManager.instance.contextCount == DialogueManager.instance.
                            dialogueDictionary[dialogueName.GetHashCode()][LocalizeManager.instance.CurrentLanguage].Count)
                        {
                            DialogueManager.instance.HideChatBox();
                            return;
                        }
                        DialogueManager.instance.ShowChatContext(dialogueName);
                    }
                }
            }
        }


        void ShowInteractionBox()
        {
            DialogueManager.instance.gameObject.transform.position = new Vector3(transform.position.x,
                transform.position.y + physicsBoxCollider.size.y * 0.5f, transform.position.z);
            DialogueManager.instance.interactionBox.SetActive(false);
            DialogueManager.instance.interactionBox.SetActive(true);
        }

        void HideChatBox()
        {
            DialogueManager.instance.isTyping = false;
            DialogueManager.instance.isChatting = false;
            DialogueManager.instance.contextCount = 0;
            DialogueManager.instance.chatContext.text = string.Empty;
            DialogueManager.instance.chatBox.SetActive(false);
        }
        void HideInteractionBox()
        {
            if (!DialogueManager.instance.interactionBox.activeSelf)
            {
                return;
            }
            interactBoxAnimator.Play(Animator.StringToHash("Close"));
        }
       
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                ShowInteractionBox();
            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                isNear = true;
                if (DialogueManager.instance.isChatting)
                {
                    HideInteractionBox();
                    return;
                }
            }
        }
        void OnTriggerExit2D(Collider2D other)
        {
            if(other.tag == "Player")
            {
                isNear = false;
                HideInteractionBox();
                StartCoroutine(WaitPlayerComeBack());
            }
        }
        IEnumerator WaitPlayerComeBack()
        {
            float waitTime = 1f;
            while(waitTime > 0)
            {
                waitTime -= Time.deltaTime;
                yield return null;
            }
            if (!isNear)
            {
                DialogueManager.instance.HideChatBox();
            }
        }
    }
}