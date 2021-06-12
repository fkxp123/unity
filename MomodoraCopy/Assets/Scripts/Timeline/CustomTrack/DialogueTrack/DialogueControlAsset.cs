using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace MomodoraCopy
{
    [Serializable]
    public class DialogueControlAsset : PlayableAsset, ITimelineClipAsset
    {
        //[SerializeField]
        public DialogueControlBehaviour template = new DialogueControlBehaviour();

        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<DialogueControlBehaviour>.Create(graph, template);
        }
    }

}