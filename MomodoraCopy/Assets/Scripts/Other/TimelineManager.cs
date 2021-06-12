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
            currentPlayableDirector = playableDirectorNameDict["Prologue"];
            RefreshDialogueTracks();
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
                playableDirector.gameObject.SetActive(false);
                TimelineAsset timelineAsset = (TimelineAsset)playableDirector.playableAsset;
                foreach(var track in timelineAsset.GetOutputTracks())
                {
                    if(track.GetType().Name == "DialogueTrack")
                    {
                        DialogueTrack dialogueTrack = (DialogueTrack)track;
                        dialogueTrack.AddClipToTrack();
                    }
                }
            }
        }

        public void Play()
        {

        }

        public void Pause()
        {
            if (currentPlayableDirector.state == PlayState.Paused)
            {
                return;
            }
            currentPlayableDirector.Pause();
        }
        public void Stop()
        {
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
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentPlayableDirector.gameObject.SetActive(true);
                currentPlayableDirector.Play();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                //playableDirectorNameDict["Prologue"].Pause();
                playableDirectorNameDict["Prologue"].gameObject.SetActive(false);
            }
        }
    }
}