using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EVENT_TYPE
{
    GAME_INIT,
    GAME_END,
    AMMO_CHANGE,
    HEALTH_CHANGE,
    DEAD
};

public interface IListener
{
    void OnEvent(EVENT_TYPE Event_Type, IListener Listener);
}

public class EventManager : MonoBehaviour
{
    #region Singleton
    public static EventManager Instance
    {
        get { return instance; }
        set { }
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            DestroyImmediate(gameObject);
    }
    private static EventManager instance = null;
    #endregion
    private Dictionary<EVENT_TYPE, List<IListener>> Listeners = new Dictionary<EVENT_TYPE, List<IListener>>();

    //구독자는 start에서 리스너로 등록해야한다
    public void AddListener(EVENT_TYPE Event_Type, IListener Listener)
    {
        List<IListener> ListenerList = new List<IListener> { Listener };
        //만약 Listeners딕셔너리에 넘겨받은 EVENT_TYPE키가 존재할 때 해당 키의 리스트에 Listener 추가
        if (Listeners.ContainsKey(Event_Type))
            Listeners[Event_Type].Add(Listener);
        //만약 Listeners딕셔너리에 넘겨받은 EVENT_TYPE키가 존재하지 않고 새로운 이벤트 타입일 때 딕셔너리에 해당 키와 리스트 추가
        else
            Listeners.Add(Event_Type, ListenerList);
        #if SHOW_DEBUG_MESSAGE
        foreach (IListener L in Listeners[Event_Type])
        {
            Debug.Log(Event_Type + " Event Listener : " + L);
        }
        #endif
    }
    public void PostNotification(EVENT_TYPE Event_Type)
    {
        List<IListener> PostList = new List<IListener>();
        //특정 이벤트가 발생하면 이 이벤트를 구독하는 구독자만 이벤트 발생을 알림
        PostList = Listeners[Event_Type];
        foreach(IListener L in PostList)
        {
            L.OnEvent(Event_Type, L);
        }
    }
    public void RemoveEvent(EVENT_TYPE Event_Type)
    {
        Listeners.Remove(Event_Type);
    }
    public void RemoveNull()
    {
        foreach(KeyValuePair<EVENT_TYPE, List<IListener>> pair in Listeners)
        {
            foreach(IListener item in pair.Value)
            {
                if (item.Equals(null))
                    pair.Value.Remove(item);
            }
        }
    }
}