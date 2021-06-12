using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace MomodoraCopy
{
    //[CustomStyle("myStyle")]
    public class NotificationMarker : Marker, INotification
    {
        public PropertyName id { get; }
    } 
}
