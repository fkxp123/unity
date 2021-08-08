using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace MomodoraCopy
{
    [Serializable]
    public class CharacterControlAsset : PlayableAsset, ITimelineClipAsset
    {
        //[SerializeField]
        public CharacterControlBehaviour template = new CharacterControlBehaviour();

        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<CharacterControlBehaviour>.Create(graph, template);
        }
    }

}