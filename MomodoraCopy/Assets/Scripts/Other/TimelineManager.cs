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

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            RefreshDialogueTracks();
        }
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void Start()
        {
            RefreshDialogueTracks();
            EventManager.instance.AddListener(EventType.LanguageChanged, OnLanguageChanged);
        }
        
        void OnLanguageChanged()
        {
            RefreshDialogueTracks();
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

        public void RefreshDialogueTracks()
        {
            foreach(PlayableDirector playableDirector in playableDirectorInfoDict.Keys)
            {
                playableDirector.time = 0;
                playableDirector.Stop();
                playableDirector.Evaluate();
                
                TimelineAsset timelineAsset = (TimelineAsset)playableDirector.playableAsset;
                foreach(var track in timelineAsset.GetOutputTracks())
                {
                    if(track is DialogueTrack dialogueTrack)
                    {
                        dialogueTrack = (DialogueTrack)track;
                        dialogueTrack.AddClipToTrack();
                        //var clips = dialogueTrack.GetClips();
                        //foreach(var clip in clips)
                        //{
                        //    save dialoguetrack.dialoguename / clip.start / clip.end?
                        //}
                    }
                }
            }
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