using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MomodoraCopy
{
    public enum EventType
    {
        LanguageChanged,
        GamePause,
        GameResume
    };

    public class EventManager : Singleton<EventManager>
    {
        public delegate void OnEvent();

        private Dictionary<EventType, List<OnEvent>> listenerDict = new Dictionary<EventType, List<OnEvent>>();

        public void AddListener(EventType eventType, OnEvent listenerOnEvent)
        {
            if (listenerDict.ContainsKey(eventType))
            {
                listenerDict[eventType].Add(listenerOnEvent);
            }
            else
            {
                List<OnEvent> ListenerList = new List<OnEvent>();
                ListenerList.Add(listenerOnEvent);
                listenerDict.Add(eventType, ListenerList);
            }
        }
        public void PostNotification(EventType eventType)
        {
            if (!listenerDict.ContainsKey(eventType))
            {
                return;
            }
            for (int i = 0; i < listenerDict[eventType].Count; i++)
            {
                listenerDict[eventType][i]();
            }
        }
        public void RemoveEvent(EventType eventType)
        {
            if (!listenerDict.ContainsKey(eventType))
            {
                return;
            }
            listenerDict.Remove(eventType);
        }
        public void UnsubscribeEvent(EventType eventType, OnEvent listenerOnEvent)
        {
            if (!listenerDict.ContainsKey(eventType))
            {
                return;
            }
            if (!listenerDict[eventType].Contains(listenerOnEvent))
            {
                return;
            }
            listenerDict[eventType].Remove(listenerOnEvent);
        }
        public void RemoveNull()
        {
            foreach (KeyValuePair<EventType, List<OnEvent>> pair in listenerDict)
            {
                foreach (OnEvent item in pair.Value)
                {
                    if (item.Equals(null))
                        pair.Value.Remove(item);
                }
            }
        }
    }

}