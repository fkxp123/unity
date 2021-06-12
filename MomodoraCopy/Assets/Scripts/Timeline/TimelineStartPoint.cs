using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace MomodoraCopy
{
    public class TimelineStartPoint : MonoBehaviour
    {
        public PlayableDirector playableDirector;

        bool played;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                if (played)
                {
                    return;
                }
                played = true;
                TimelineManager.instance.SetCurrentPlayableDirector(playableDirector);    
            }
        }
    }

}