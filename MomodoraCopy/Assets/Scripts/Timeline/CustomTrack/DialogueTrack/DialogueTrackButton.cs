using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MomodoraCopy
{
    [CustomEditor(typeof(DialogueTrack))]
    public class DialogueTrackButton : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DialogueTrack dialogueTrack = (DialogueTrack)target;

            if (GUILayout.Button("Load Dialogue"))
            {
                dialogueTrack.AddClipToTrack();
            }
            if (GUILayout.Button("Clear Dialogue"))
            {
                dialogueTrack.ClearAllClips();
            }
        }
    }
}