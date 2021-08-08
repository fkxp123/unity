using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace MomodoraCopy
{
    public class InitTimeline : MonoBehaviour
    {
        Dictionary<string, GameObject> talkerDictionary;

        public string talkerName;
        public GameObject talkerChatInteraction;

        PlayableDirector playableDirector;
        BoxCollider2D boxCollider;

        void Start()
        {
            playableDirector = GetComponent<PlayableDirector>();
            boxCollider = talkerChatInteraction.transform.parent.GetComponent<BoxCollider2D>();
            TimelineInfo info = new TimelineInfo
            {
                talkerPos = new Vector3(talkerChatInteraction.transform.position.x,
                talkerChatInteraction.transform.position.y + boxCollider.size.y / 2, 0)
            };

            TimelineManager.instance.playableDirectorInfoDict.Add(playableDirector, info);
            TimelineManager.instance.playableDirectorNameDict.Add(gameObject.name, playableDirector);

        }
    }
}