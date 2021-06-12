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
        public float dialogueStartTime = 0.1f;
        public bool finishWithDialogue;

        public Dictionary<Language, List<string>> dialogueDictionary = new Dictionary<Language, List<string>>();

        Language currentLanguage;

        public void AddClipToTrack()
        {
            ClearAllClips();

            currentLanguage = DialogueManager.instance.currentLanguage;
            List<string> dialogueContext = GetDialogue(dialogueName, (int)currentLanguage);
            PlayableDirector playableDirector = TimelineManager.instance.playableDirectorNameDict[dialogueName];

            float clipStartTime = dialogueStartTime;
            for (int i = 0; i <= dialogueContext.Count; i++)
            {
                TimelineClip clip = CreateDefaultClip();
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
                    clipStartTime += dialogueContext[i].Length * DialogueManager.instance.typingCycle + DialogueManager.instance.typingCycle;
                    dialogueClip.template.chatPosition =
                           TimelineManager.instance.playableDirectorInfoDict[playableDirector].talkerPos;
                    dialogueClip.template.dialogueName = dialogueName;
                    dialogueClip.template.talkerName = dialogueName;
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
                    dialogueClip.template.talkerName = dialogueName;
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
                return;
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