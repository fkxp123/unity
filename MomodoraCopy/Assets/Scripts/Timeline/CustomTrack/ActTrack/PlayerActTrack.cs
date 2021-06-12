using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace MomodoraCopy
{
    [TrackColor(111f / 255f, 211f / 255f, 111f / 255f)]
    [TrackClipType(typeof(DialogueControlAsset))]
    public class PlayerActTrack : TrackAsset
    {
        GameObject playerPhysics;
        GameObject playerSprite;


    }

}