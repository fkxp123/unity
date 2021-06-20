using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace MomodoraCopy
{
    public class TimelineStartPoint : MonoBehaviour
    {
        public PlayableDirector playableDirector;

        bool entered;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player" && !entered)
            {
                entered = true;
                TimelineManager.instance.SetCurrentPlayableDirector(playableDirector);
                StartCoroutine(WaitPlayerStateFinish());
            }
        }

        IEnumerator WaitPlayerStateFinish()
        {
            while(GameManager.instance.playerFsm.stateMachine.CurrentState != GameManager.instance.playerFsm.idle &&
                GameManager.instance.playerFsm.stateMachine.CurrentState != GameManager.instance.playerFsm.run &&
                GameManager.instance.playerFsm.stateMachine.CurrentState != GameManager.instance.playerFsm.land)
            {
                yield return null;
            }
            GameManager.instance.playerFsm.stateMachine.SetState(GameManager.instance.playerFsm.talking);
            gameObject.SetActive(false);
        }
    }

}