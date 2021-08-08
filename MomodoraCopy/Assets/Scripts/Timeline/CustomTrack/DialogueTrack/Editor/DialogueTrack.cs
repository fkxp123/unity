using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace MomodoraCopy
{
    [TrackColor(241f / 255f, 249f / 255f, 99f / 255f)]
    //[TrackBindingType(typeof(GameObject))]
    [TrackClipType(typeof(DialogueControlAsset))]
    public class DialogueTrack : TrackAsset
    {
        public string dialogueName;
        public float dialogueStartTime = 0f;
        public bool finishWithDialogue;

        public Dictionary<Language, List<string>> dialogueDictionary = new Dictionary<Language, List<string>>();

        public List<TimelineClip> clipList = new List<TimelineClip>();

        public void AddClipToTrack()
        {
            ClearAllClips();

            List<string> dialogueContext = GetDialogue(dialogueName, (int)LocalizeManager.instance.CurrentLanguage);
            PlayableDirector playableDirector = TimelineManager.instance.playableDirectorNameDict[dialogueName];
            clipList.Clear();
            float clipStartTime = dialogueStartTime;
            for (int i = 0; i <= dialogueContext.Count; i++)
            {
                TimelineClip clip = CreateDefaultClip();
                clipList.Add(clip);
                DialogueControlAsset dialogueClip = clip.asset as DialogueControlAsset;
                if (i == 0)
                {
                    clip.displayName = "Start";
                }
                else if(i == dialogueContext.Count)
                {
                    clip.displayName = "End";
                }
                else
                {
                    clip.displayName = i.ToString();
                }
                if(i < dialogueContext.Count)
                {
                    clip.start = clipStartTime;
                    clip.duration = dialogueContext[i].Length * DialogueManager.instance.typingCycle;
                    clipStartTime += dialogueContext[i].Length * DialogueManager.instance.typingCycle 
                        + DialogueManager.instance.typingCycle;
                    dialogueClip.template.chatPosition =
                           TimelineManager.instance.playableDirectorInfoDict[playableDirector].talkerPos;
                    dialogueClip.template.dialogueName = dialogueName;
                    dialogueClip.template.letterCount = dialogueContext[i].Length;
                    dialogueClip.template.context = dialogueContext[i];
                    dialogueClip.template.endLine = dialogueContext.Count - 1;
                    dialogueClip.template.contextLine = i;
                }
                else
                {
                    clip.start = clipStartTime;
                    clip.duration = 1.5f;

                    dialogueClip.template.finishWithDialogue = finishWithDialogue;
                    dialogueClip.template.chatPosition =
                           TimelineManager.instance.playableDirectorInfoDict[playableDirector].talkerPos;
                    dialogueClip.template.dialogueName = dialogueName;
                    dialogueClip.template.context = "end";
                    dialogueClip.template.endLine = dialogueContext.Count - 1;
                    dialogueClip.template.contextLine = dialogueContext.Count;

                }
            }
        }

        public void ClearAllClips()
        {
            var asset = TimelineEditor.inspectedAsset;
            
            if (asset == null)
            {
                var trackAsset = clipList[0].parentTrack;
                var timelineAsset = trackAsset.timelineAsset;
                for(int i = 0; i < clipList.Count; i++)
                {
                    timelineAsset.DeleteClip(clipList[i]);
                }
                clipList.Clear();
                return;
            }

            foreach (var track in asset.GetRootTracks())
            {
                if (track is DialogueTrack)
                {
                    var clips = track.GetClips();
                    foreach (var clip in clips)
                    {
                        asset.DeleteClip(clip);
                    }
                    //var markers = track.GetMarkers();
                    //foreach (var m in markers)
                    //{
                    //    track.DeleteMarker(m);
                    //}

                }
            }
            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
            TimelineEditor.Refresh(RefreshReason.ContentsModified);
        }

        List<string> GetDialogue(string _CSVFileName, int localNumber)
        {
            List<string> dialogueList = new List<string>();
            TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName);

            string[] data = csvData.text.Split(new char[] { '\n' });

            for (int i = 1; i < data.Length - 1; i++)
            {
                string[] row = data[i].Split(new char[] { ',' });
                string context = row[localNumber];
                context = context.Replace("<br>", "\n");
                context = context.Replace("<c>", ",");
                context = context.Replace("<..>", "..");

                dialogueList.Add(context);
            }

            return dialogueList;
        }
    }
}