using UnityEngine;

namespace MomodoraCopy
{
    public class EventTest : MonoBehaviour, IListener
    {
        // Start is called before the first frame update
        Istate min = new Istate();

        public void OnEvent(EVENT_TYPE Event_Type, IListener Listener)
        {
            Debug.Log("Add " + Event_Type + " Event + in " + Listener);
            if (Event_Type == EVENT_TYPE.GAME_END)
            {
                //~~~
            }
        }

        void Start()
        {
            EventManagerTest.instance.AddListener(EVENT_TYPE.GAME_INIT, this);
            min.OnEvent(EVENT_TYPE.DEAD, this);
        }


    }
    public class Istate : IListener
    {
        public void OnEvent(EVENT_TYPE Event_Type, IListener Listener)
        {
            Debug.Log("Add " + Event_Type + " Event + in " + Listener);
        }
    }

}