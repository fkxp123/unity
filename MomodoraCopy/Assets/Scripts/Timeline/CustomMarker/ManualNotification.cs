using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace MomodoraCopy
{
    public class ManualNotification : MonoBehaviour
    {
        PlayableGraph m_Graph;
        MyReceiver m_Receiver;

        void Start()
        {
            m_Graph = PlayableGraph.Create("NotificationGraph");
            var output = ScriptPlayableOutput.Create(m_Graph, "NotificationOutput");

            //Create and register a receiver
            m_Receiver = new MyReceiver();
            output.AddNotificationReceiver(m_Receiver);

            //Push a notification on the output
            output.PushNotification(Playable.Null, new MyNotification());

            m_Graph.Play();
        }
    } 
}