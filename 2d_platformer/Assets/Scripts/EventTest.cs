using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTest : MonoBehaviour, IListener
{
    // Start is called before the first frame update
    Istate min = new Istate();
    void Start()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.GAME_INIT, this);
        min.OnEvent(EVENT_TYPE.DEAD, this);
    }


    // Update is called once per frame
    void Update()
    {
    }
    public void OnEvent(EVENT_TYPE Event_Type, IListener Listener)
    {
        Debug.Log("Add " + Event_Type + " Event + in " + Listener);
    }
}
public class Istate : IListener
{
    public void OnEvent(EVENT_TYPE Event_Type, IListener Listener)
    {
        Debug.Log("Add " + Event_Type + " Event + in " + Listener);
    }
}
