using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace MomodoraCopy
{
    public class ChatInteraction : MonoBehaviour
    {
        public Vector3 checkPlayerArea;

        LayerMask playerMask;

        bool isNear;
        bool isChatting;

        public string dialogueName;

        List<string> dialogue = new List<string>();
        [TextArea]
        string dialogueContext;
        int contextCount;

        public float typingCycle = 0.1f;
        WaitForSeconds typingTime;
        bool isTyping;
        public float blinkCycle = 0.2f;
        WaitForSeconds blinkTime;

        BoxCollider2D boxCollider;

        void Start()
        {
            typingTime = new WaitForSeconds(typingCycle);
            blinkTime = new WaitForSeconds(blinkCycle);

            playerMask = 1 << 8;

            if(dialogueName != string.Empty)
            {
                dialogue = DialogueManager.instance.GetDialogue(dialogueName, 0);
            }

            boxCollider = transform.parent.GetComponent<BoxCollider2D>();
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
                transform.position.y + boxCollider.size.y * 0.5f, transform.position.z);
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
            DialogueManager.instance.interactionBox.SetActive(false);
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
                ShowInteractionBox();
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