using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace MomodoraCopy
{
    public class ScheduledNotification : MonoBehaviour
    {
        PlayableGraph playableGraph;
        MyReceiver myReceiver;

        void Start()
        {
            playableGraph = PlayableGraph.Create("NotificationGraph");
            var output = ScriptPlayableOutput.Create(playableGraph, "NotificationOutput");

            //Create and register a receiver
            myReceiver = new MyReceiver();
            output.AddNotificationReceiver(myReceiver);

            //Create a TimeNotificationBehaviour
            var timeNotificationPlayable = ScriptPlayable<TimeNotificationBehaviour>.Create(playableGraph);
            output.SetSourcePlayable(timeNotificationPlayable);

            //Add a notification on the time notification behaviour
            var notificationBehaviour = timeNotificationPlayable.GetBehaviour();
            //notificationBehaviour.AddNotification(19.1, new NotificationMarker());
            notificationBehaviour.AddNotification(5.0, new MyNotification());

            playableGraph.Play();
        }
    } 
}
