using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace MomodoraCopy
{
    [Serializable]
    public class DialogueControlBehaviour : PlayableBehaviour
    {
        bool firstFrameHappened;

        public Vector3 chatPosition;
        public string dialogueName;
        public string talkerName;
        [TextArea(5, 10)]
        public string context;
        public int letterCount;
        public int contextLine;
        public int endLine;
        public bool finishWithDialogue;

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
        }
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            if(contextLine > endLine)
            {
                DialogueManager.instance.HideChatBox();
                if (finishWithDialogue)
                {
                    GameManager.instance.playerFsm.stateMachine.SetState(GameManager.instance.playerFsm.idle);
                    TimelineManager.instance.Stop();
                }
                return;
            }
            DialogueManager.instance.isChatting = true;
            DialogueManager.instance.ShowChatBox(dialogueName, contextLine);
            DialogueManager.instance.transform.position = chatPosition;
        }
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            //chatInteraction = playerData as GameObject;

            //if (chatInteraction == null)
            //{
            //    return;
            //}

            if (!firstFrameHappened)
            {
                //defaultPos = chatInteraction.transform.position;

                //chatInteraction.transform.position = pos;
                //chatInteraction.SetActive(true);

                firstFrameHappened = true;
            }
        }
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            base.OnBehaviourPause(playable, info);
            firstFrameHappened = false;

            var duration = playable.GetDuration();

            var time = playable.GetTime();
            var delta = info.deltaTime;

            if (info.evaluationType == FrameData.EvaluationType.Playback)
            {
                var count = time + delta;

                if (count >= duration)
                {
                    TimelineManager.instance.Pause();
                }
            }

            //if (chatInteraction == null)
            //{
            //    return;
            //}
            //chatInteraction.transform.position = defaultPos;

        }
    }
}