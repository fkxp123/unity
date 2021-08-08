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
    public class CharacterControlBehaviour : PlayableBehaviour
    {
        bool firstFrameHappened;
        GameObject characterPhysics;
        GameObject characterSprite;
        BasicEnemyFsm fsm;
        public EnemyState transitionState;
        public string anim;
        Animator animator;

        public string dialogueName;
        public int startContextLine;
        public int endContextLine;
        public bool isEnd;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            TimelineManager.instance.currentDialogueLine = startContextLine;
            TimelineManager.instance.targetDialogueLine = endContextLine;
            if (isEnd)
            {
                GameManager.instance.playerFsm.stateMachine.SetState(GameManager.instance.playerFsm.idle);
                TimelineManager.instance.Stop();
                return;
            }
        }
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {

            if (!firstFrameHappened)
            {
                characterPhysics = playerData as GameObject;
                characterSprite = characterPhysics.transform.GetChild(0).gameObject;
                fsm = characterSprite.GetComponent<BasicEnemyFsm>();
                animator = characterSprite.GetComponent<Animator>();
                animator.Play(Animator.StringToHash(anim));
                //fsm.currentState = transitionState;

                firstFrameHappened = true;
            }
        }
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            base.OnBehaviourPause(playable, info);

            var duration = playable.GetDuration();

            var time = playable.GetTime();
            var delta = info.deltaTime;

            if (info.evaluationType == FrameData.EvaluationType.Playback)
            {
                var count = time + delta;

                if (count >= duration)
                {
                    TimelineManager.instance.Pause();

                    if (dialogueName != null)
                    {
                        DialogueManager.instance.isChatting = true;
                        DialogueManager.instance.ShowChatBox(dialogueName, startContextLine);
                        DialogueManager.instance.transform.position = 
                            TimelineManager.instance.playableDirectorInfoDict
                            [TimelineManager.instance.currentPlayableDirector].talkerPos;
                    }
                }
            }
        }
    }
}