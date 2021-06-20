using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SimpleMarker : Marker, INotification, INotificationOptionProvider
{

    [SerializeField] public bool emitOnce;
    [SerializeField] public bool emitInEditor;



    NotificationFlags INotificationOptionProvider.flags =>
        (emitOnce ? NotificationFlags.TriggerOnce : default) |
        (emitInEditor ? NotificationFlags.TriggerInEditMode : default);

    public PropertyName id { get; }
}
