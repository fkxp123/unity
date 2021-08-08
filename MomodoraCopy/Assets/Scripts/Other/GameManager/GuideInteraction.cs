using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class GuideInteraction : MonoBehaviour
    {
        string context;


        void Start()
        {
            Rigidbody2D rigid = GetComponent<Rigidbody2D>();
            rigid.bodyType = RigidbodyType2D.Kinematic;

            context = LocalizeManager.instance.guidesDict[gameObject.name.GetHashCode()]
                [LocalizeManager.instance.CurrentLanguage];
            EventManager.instance.AddListener(EventType.LanguageChange, OnLanguageChanged);
        }

        void OnDestroy()
        {
            EventManager.instance.UnsubscribeEvent(EventType.LanguageChange, OnLanguageChanged);
        }

        void OnLanguageChanged()
        {
            context = LocalizeManager.instance.guidesDict[gameObject.name.GetHashCode()]
                [LocalizeManager.instance.CurrentLanguage];
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                GameManager.instance.ShowGuideText(context);
            }
        }
        void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                GameManager.instance.HideGuideText();

            }
        }
    }

}