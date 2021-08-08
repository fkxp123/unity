using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace MomodoraCopy
{
    public class TimelineInfo
    {
        public Vector3 talkerPos;

    }

    public class TimelineManager : Singleton<TimelineManager>
    {
        public Dictionary<PlayableDirector, TimelineInfo> playableDirectorInfoDict 
            = new Dictionary<PlayableDirector, TimelineInfo>();
        public Dictionary<string, PlayableDirector> playableDirectorNameDict
            = new Dictionary<string, PlayableDirector>();

        public PlayableDirector currentPlayableDirector;
        public PlayableDirector tempPlayableDirector;
        public string currentPlayableDirectorName;

        public int currentDialogueLine;
        public int targetDialogueLine;

        void Start()
        {
            EventManager.instance.AddListener(EventType.LanguageChange, OnLanguageChanged);
        }
        
        void OnLanguageChanged()
        {
            tempPlayableDirector = currentPlayableDirector;
            Stop();
            currentPlayableDirector = tempPlayableDirector;
            if(currentPlayableDirector != null)
            {
                currentPlayableDirector.Play();
            }
        }

        public void SetCurrentPlayableDirector(PlayableDirector playableDirector)
        {
            currentPlayableDirector = playableDirector;
            currentPlayableDirector.gameObject.SetActive(true);
            currentPlayableDirector.Play();
        }

        public void Play()
        {

        }

        public void Pause()
        {
            if (currentPlayableDirector == null)
            {
                return;
            }
            if (currentPlayableDirector.state == PlayState.Paused)
            {
                return;
            }
            currentPlayableDirector.Pause();
        }
        public void Stop()
        {
            if(currentPlayableDirector == null)
            {
                return;
            }
            currentPlayableDirector.Stop();
            currentPlayableDirector = null;
        }

        void Update()
        {
            if (currentPlayableDirector == null)
            {
                return;
            }
            if (currentPlayableDirector.state == PlayState.Playing)
            {
                return;
            }

            if (currentDialogueLine < targetDialogueLine)
            {
                if (Input.GetKeyDown(KeyboardManager.instance.UpKey))
                {
                    if (DialogueManager.instance.isTyping)
                    {
                        return;
                    }
                    if (currentDialogueLine == targetDialogueLine - 1)
                    {
                        DialogueManager.instance.HideChatBox();
                        currentPlayableDirector.Play();
                        //currentDialogueLine = 0;
                        return;
                    }
                    currentDialogueLine++;
                    DialogueManager.instance.ShowChatContext(DialogueManager.instance.currentDialogueName, currentDialogueLine);
                    return;
                }
            }



            if (DialogueManager.instance.ableToPassNext && Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentPlayableDirector.gameObject.SetActive(true);
                currentPlayableDirector.Play();
            }
            //if (Input.GetKeyDown(KeyCode.DownArrow))
            //{
            //    //playableDirectorNameDict["Prologue"].Pause();
            //    playableDirectorNameDict["Prologue"].gameObject.SetActive(false);
            //}
        }
    }
}