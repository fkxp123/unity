using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardManager : MonoBehaviour
{
    public static KeyboardManager instance;

    public KeyCode Jump { get; set; }
    public KeyCode Left { get; set; }
    public KeyCode Right { get; set; }
    public KeyCode Up { get; set; }
    public KeyCode Down { get; set; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (instance != this)
        {
            DestroyImmediate(this);
        }

        Jump = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("JumpKey", "A"));
        Left = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("LeftKey", "LeftArrow"));
        Right = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("RightKey", "RightArrow"));
        Up = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("UpKey", "UpArrow"));
        Down = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("DownKey", "DownArrow")); 
    }
}
