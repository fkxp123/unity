using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace MomodoraCopy
{
    [TrackColor(111f / 255f, 211f / 255f, 111f / 255f)]
    [TrackBindingType(typeof(GameObject))]
    [TrackClipType(typeof(CharacterControlAsset))]
    public class CharacterControlTrack : TrackAsset
    {
        BasicEnemyFsm fsm;
        Player playerfsm;

        public List<TimelineClip> clipList = new List<TimelineClip>();

        //public void InitCharacter()
        //{
        //    var asset = TimelineEditor.inspectedAsset;

        //    foreach (var track in asset.GetRootTracks())
        //    {
        //        if (track is CharacterControlTrack)
        //        {
        //            var clips = track.GetClips();
        //            foreach (var clip in clips)
        //            {
        //                clipList.Add(clip);
        //            }
        //        }
        //    }
        //}
    }

}