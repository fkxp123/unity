using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test5 : MonoBehaviour
{
    //// https://docs.unity3d.com/ScriptReference/RuntimeInitializeLoadType.html
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //static void OnRuntimeMethodLoad()
    //{
    //    Debug.Log("BeforeSceneLoad");
    //}
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    //static void OnSecondRuntimeMethodLoad()
    //{
    //    Debug.Log("AfterSceneLoad");
    //}
    void Start()
    {
        Debug.Log("start");
    }
    void Awake()
    {
        Debug.Log("awake");
    }
}
