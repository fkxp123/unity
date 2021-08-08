using System.Collections.Generic;
using System.Linq;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR

[CustomTimelineEditor(typeof(SignalTrackEx))]
public class FakeTrackEditor : TrackEditor
{
    public override void OnCreate(TrackAsset track, TrackAsset copiedFrom)
    {
        track.CreateCurves("FakeCurves");
        track.curves.SetCurve(string.Empty, typeof(GameObject), "m_FakeCurve", AnimationCurve.Linear(0, 1, 1, 1));
        base.OnCreate(track, copiedFrom);
    }
}

#endif


public class SignalTrackEx : MarkerTrack
{

    //ReceiverExample m_Receiver;

    public override IEnumerable<PlayableBinding> outputs
    {
        get
        {
            var playableBinding = ScriptPlayableBinding.Create(name, null, typeof(GameObject));


            //return this == timelineAsset.markerTrack ? new List<PlayableBinding> {playableBinding} : base.outputs;
            // return base.outputs;
            return new List<PlayableBinding> { playableBinding };
        }
    }



    //public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    //{


    //    if (Application.isPlaying)
    //    {
    //        PlayableOutput playableOutput = graph.GetOutput(0);

    //        if (playableOutput.IsOutputValid())
    //        {
    //            ScriptPlayable<TimeNotificationBehaviour> scriptPlayable =
    //                (ScriptPlayable<TimeNotificationBehaviour>)playableOutput.GetSourcePlayable().GetInput(0);



    //            TimeNotificationBehaviour timeNotificationBehaviour = scriptPlayable.GetBehaviour();

    //            var simpleMarkers = this.GetMarkers().OfType<SimpleMarker>();

    //            m_Receiver = new ReceiverExample();

    //            playableOutput.AddNotificationReceiver(m_Receiver);

    //            foreach (var marker in simpleMarkers)
    //            {

    //                scriptPlayable.GetBehaviour().AddNotification(marker.time, marker);
    //            }
    //        }
    //        else
    //        {

    //            playableOutput = ScriptPlayableOutput.Create(graph, "NotificationOutput");

    //            m_Receiver = new ReceiverExample();

    //            //why also here and in "outputs"
    //            playableOutput.AddNotificationReceiver(m_Receiver);

    //            //Create a TimeNotificationBehaviour
    //            var timeNotificationPlayable = ScriptPlayable<TimeNotificationBehaviour>.Create(graph);

    //            playableOutput.SetSourcePlayable(graph.GetRootPlayable(0));
    //            timeNotificationPlayable.GetBehaviour().timeSource = playableOutput.GetSourcePlayable();
    //            playableOutput.GetSourcePlayable().SetInputCount(playableOutput.GetSourcePlayable().GetInputCount() + 1);
    //            graph.Connect(timeNotificationPlayable, 0, playableOutput.GetSourcePlayable(), playableOutput.GetSourcePlayable().GetInputCount() - 1);

    //            var simpleMarkers = this.GetMarkers().OfType<SimpleMarker>();


    //            foreach (var marker in simpleMarkers)
    //            {

    //                timeNotificationPlayable.GetBehaviour().AddNotification(marker.time, marker);
    //            }
    //        }
    //    }

    //    return base.CreateTrackMixer(graph, go, inputCount);

    //}

}