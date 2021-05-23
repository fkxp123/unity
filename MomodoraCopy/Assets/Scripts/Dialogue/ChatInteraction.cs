using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class ChatInteraction : MonoBehaviour
    {
        GameObject chatBox;
        GameObject interactionBox;

        public Vector3 checkPlayerArea;

        LayerMask playerMask;

        bool isNear;
        bool isChatting;

        public string dialogueName;

        List<string> dialogue = new List<string>();
        [TextArea]
        string dialogueContext;
        int contextCount;

        float typingCycle = 0.1f;
        WaitForSeconds typingTime;
        bool isTyping;
        float blinkCycle = 0.2f;
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
            if (isNear)
            {
                if (!isChatting)
                {
                    if (Input.GetKeyDown(KeyboardManager.instance.UpKey))
                    {
                        isChatting = true;
                        HideInteractionBox();
                        ShowChatBox();
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyboardManager.instance.UpKey))
                    {
                        if (isTyping)
                        {
                            return;
                        }
                        if(contextCount == dialogue.Count)
                        {
                            HideChatBox();
                            return;
                        }
                        ShowChatContext();
                    }
                }
            }
        }
        void ShowChatContext()
        {
            dialogueContext = dialogue[contextCount];
            StartCoroutine(TypeContexts(dialogueContext));
            DialogueManager.instance.SetChatBox(dialogue[contextCount]);
            Vector3 backgroundSize = DialogueManager.instance.GetBackgroundSize();
            DialogueManager.instance.gameObject.transform.position =
                new Vector3(transform.position.x, transform.position.y + backgroundSize.y * 0.0315f, transform.position.y);
            contextCount++;
        }

        void ShowChatBox()
        {
            DialogueManager.instance.gameObject.transform.position = transform.position;
            DialogueManager.instance.chatBox.SetActive(true);
            if (dialogue.Count == 0)
            {
                return;
            }
            ShowChatContext();
        }
        void ShowInteractionBox()
        {
            DialogueManager.instance.gameObject.transform.position = new Vector3(transform.position.x,
                transform.position.y + boxCollider.size.y * 0.5f, transform.position.z);
            DialogueManager.instance.interactionBox.SetActive(true);
        }
        void HideChatBox()
        {
            isTyping = false;
            isChatting = false;
            contextCount = 0;
            DialogueManager.instance.chatContext.text = string.Empty;
            DialogueManager.instance.chatBox.SetActive(false);
        }
        void HideInteractionBox()
        {
            DialogueManager.instance.interactionBox.SetActive(false);
        }
        
        IEnumerator TypeContexts(string context)
        {
            yield return null;
            isTyping = true;
            for(int i = 0; i < context.Length; i++)
            {
                DialogueManager.instance.chatContext.text = context.Substring(0, i);
                yield return typingTime;
            }
            if (!isChatting)
            {
                DialogueManager.instance.chatContext.text = string.Empty;
                yield break;
            }
            yield return typingTime;
            isTyping = false;
            StartCoroutine(BlinkNextButton());
        }

        IEnumerator BlinkNextButton()
        {
            float i = 0;
            RectTransform rectTransform = DialogueManager.instance.nextButtonRectTransform;
            while (!isTyping)
            {
                i++;
                if (i % 2 == 0)
                { 
                    rectTransform.anchoredPosition = 
                        new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + 1, 0);
                }
                else
                {
                    rectTransform.anchoredPosition =
                        new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y - 1, 0);

                }
                yield return blinkTime;
            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                isNear = true;
                if (isChatting)
                {
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
                HideChatBox();
            }
        }
    }
}