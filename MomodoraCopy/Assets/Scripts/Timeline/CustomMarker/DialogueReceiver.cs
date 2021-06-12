using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace MomodoraCopy
{
    public class DialogueReceiver : MonoBehaviour, INotificationReceiver
    {
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is NotificationMarker marker)
            {
                TimelineManager.instance.Stop();
                DialogueManager.instance.HideChatBox();
            }
    }
    }

}